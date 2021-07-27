using AnalyticsJobsService.Interface;
using CrouterStorage.Interface;
using CrouterStorage.Models;
using CrouterStorage.Service.Logic;
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

namespace CrouterStorage.Service
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CrouterStorageService : StatefulService, ICrouterStorageService
    {
        private static string StorageName = "CroutersStorageQueue";
        private readonly IAnalyticsJobsService _analyticsJobsService;
        private readonly ICrouterStorageServiceLogic _countsStorageServiceLogic;

        // logging // 
        private string EnqueueMessage(long subjectId, int itemsEnqueued) => $"Crouter Cutpoints Enqueued for Subject: {subjectId} item Count: {itemsEnqueued}";
        private string StorageMessage(long subjectId, int itemsEnqueued, TimeSpan timeSpan) => $"Crouter Cutpoints Stored for Subject: {subjectId} item Count: {itemsEnqueued} ProcessingTimeInSeconds: {timeSpan.TotalSeconds}";

        public CrouterStorageService(StatefulServiceContext context)
            : base(context)
        {
            var configurationPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var connectionStringParameter = configurationPackage.Settings.Sections["ExternalConnections"].Parameters["dbstorageconnection"];

            _analyticsJobsService = FabricServices.GetAnalyticsJobsService();
            _countsStorageServiceLogic = new CrouterStorageServiceLogic(connectionStringParameter.Value);
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

        public async Task EnqueueMessage(CrouterStorageDto crouterStorage)
        {
            IReliableConcurrentQueue<CrouterStorageDto> queue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<CrouterStorageDto>>(StorageName);
            using (var txn = this.StateManager.CreateTransaction())
            {
                await queue.EnqueueAsync(txn, crouterStorage);
                await txn.CommitAsync();
            }

            ServiceEventSource.Current.Message(EnqueueMessage(crouterStorage.SubjectId, crouterStorage.CrouterDto.Length));
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            IReliableConcurrentQueue<CrouterStorageDto> queue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<CrouterStorageDto>>(StorageName);

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
                            var itemsStored = await _countsStorageServiceLogic.InsertNewCrouterCutpoints(result.Value);
                            //update algorithm task // 
                            var completedDateTimeUtc = DateTime.UtcNow;
                            await _analyticsJobsService.UpdateAlgorithmTaskStorageItemsComplete(result.Value.TaskId, startedDateTimeUtc, completedDateTimeUtc, itemsStored, "");
                            // commit trans and remove item from the queue // 
                            await tx.CommitAsync();
                            ServiceEventSource.Current.Message(StorageMessage(result.Value.SubjectId, result.Value.CrouterDto.Length, (completedDateTimeUtc - startedDateTimeUtc)));
                        }
                        catch (Exception ex)
                        {
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
