using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using EpochRetrievalActor.Interfaces;
using AnalyticsJobsService.Models;
using EpochRetrievalActor.Logic;
using EpochRetrievalActor.EpochSqlService;
using EpochRetrieval.Models;

namespace EpochRetrievalActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class EpochRetrievalActor : Actor, IEpochRetrievalActor
    {
        private EpochRetrievalLogic _epochLogic;
        /// <summary>
        /// Initializes a new instance of EpochRetrievalActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public EpochRetrievalActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            _epochLogic = new EpochRetrievalLogic(new EpochSqlService.EpochSqlService(actorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config")
                                    .Settings.Sections["ConnectionStrings"].Parameters["EpochsDbConnectionString"].Value));
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return base.OnActivateAsync();
        }

        public async Task<EpochRetrievalTaskResult> ProcessEpochRequest(long subjectId, AlgorithmTaskDto[] epochAlgorithmTasks)
        {
            var result = new EpochRetrievalTaskResult() { ProcessingStartedUtc = DateTime.UtcNow };
            try 
            {
                var timeRangeResults = await _epochLogic.ProcessEpochRetrievalTask(subjectId, epochAlgorithmTasks);
                result.AdjustedBeginTimestampUtc = timeRangeResults.Item1;
                result.AdjustedEndTimestampUtc = timeRangeResults.Item2;
            }
            catch (Exception ex) 
            {
                result.ErrorMessage = ex.Message;
                
            }
            result.ProcessingEndedUtc = DateTime.UtcNow;

            return result;
        }
    }
}
