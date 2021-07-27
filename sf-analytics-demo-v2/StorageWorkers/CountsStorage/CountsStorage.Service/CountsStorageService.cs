using AnalyticsJobsService.Interface;
using CountsStorage.Interface;
using CountsStorage.Models;
using CountsStorage.Service.Logic;
using Global.Services;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace CountsStorage.Service
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CountsStorageService : StatefulService, ICountsStorageService
    {
        private static string StorageName = "CountsStorageQueue";
        private readonly IAnalyticsJobsService _analyticsJobsService;
        private readonly ICountsStorageServiceLogic _countsStorageServiceLogic;

        // logging // 
        private string EnqueueMessage(long subjectId, int itemsEnqueued) => $"Counts Enqueued for Subject: {subjectId} item Count: {itemsEnqueued}";
        private string StorageMessage(long subjectId, int itemsEnqueued, TimeSpan timeSpan) => $"Counts Stored for Subject: {subjectId} item Count: {itemsEnqueued} ProcessingTimeInSeconds: {timeSpan.TotalSeconds}";


        public CountsStorageService(StatefulServiceContext context)
            : base(context)
        {
            var configurationPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var connectionStringParameter = configurationPackage.Settings.Sections["ExternalConnections"].Parameters["dbstorageconnection"];

            _analyticsJobsService = FabricServices.GetAnalyticsJobsService();
            _countsStorageServiceLogic = new CountsStorageServiceLogic(connectionStringParameter.Value);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context=> new FabricTransportServiceRemotingListener(context, this))
            };
        }

        public async Task EnqueueMessage(CountsStorageDto countsStorageDto)
        {
            IReliableConcurrentQueue<CountsStorageDto> queue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<CountsStorageDto>>(StorageName);
            using (var txn = this.StateManager.CreateTransaction())
            {
                await queue.EnqueueAsync(txn, countsStorageDto);
                await txn.CommitAsync();
            }

            ServiceEventSource.Current.Message(EnqueueMessage(countsStorageDto.SubjectId, countsStorageDto.CountsDto.Length));
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            IReliableConcurrentQueue<CountsStorageDto> queue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<CountsStorageDto>>(StorageName);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await queue.TryDequeueAsync(tx, cancellationToken);
                    if (result.HasValue)
                    {
                        var startedDateTimeUtc = DateTime.UtcNow;
                        try
                        {
                            //store items //
                            var itemsStored = await _countsStorageServiceLogic.InsertNewCounts(result.Value);
                            //update algorithm task // 
                            var completedDateTimeUtc = DateTime.UtcNow;
                            await _analyticsJobsService.UpdateAlgorithmTaskStorageItemsComplete(result.Value.TaskId, startedDateTimeUtc, completedDateTimeUtc, itemsStored, "");
                            // commit trans and remove item from the queue // 
                            await tx.CommitAsync();
                            ServiceEventSource.Current.Message(StorageMessage(result.Value.SubjectId, result.Value.CountsDto.Length, (completedDateTimeUtc - startedDateTimeUtc)));
                        }
                        catch (Exception ex)
                        {
                            // update analytics job database if failure //
                            await _analyticsJobsService.UpdateAlgorithmTaskStorageItemsComplete(result.Value.TaskId, startedDateTimeUtc, DateTime.UtcNow, 0, ex.Message);
                            tx.Abort();
                        }
                    }
                    
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
