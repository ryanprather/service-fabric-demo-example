using AnalyticsJobsService.Interface;
using Global.Services;
using JobActor.Interfaces;
using JobActor.Logic;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Volatile)]
    internal class JobActor : Actor, IJobActor
    {

        private const string SubjectKey = "SubjectStateId";
        private const string StudyKey = "StudyStateId";
        private const string UploadKey = "UploadStateId";
        private const string BeginKey = "BeginTimestampStateId";
        private const string EndKey = "EndTimestampStateId";
        private const string UploadProcessingKey = "uploadProcessingStateId";
        private const string ReadyToProcessKey = "ReadyToProcessStateId";

        private IActorTimer _processUploadTimer;
        //private readonly IAnalyticsJobsService _analyticsJobsService;
        /// <summary>
        /// Initializes a new instance of JobActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public JobActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Job Actor activated.");

            
            _processUploadTimer = RegisterTimer(
            ProcessUpload,                     // Callback method
            null,                           // Parameter to pass to the callback method
            TimeSpan.FromMilliseconds(20),  // Amount of time to delay before the callback is invokeds
            TimeSpan.FromSeconds(10)); // Time interval between invocations of the callback method

            this.StateManager.AddStateAsync(ReadyToProcessKey, false);

            return base.OnActivateAsync();
        }

        protected override Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Subject Actor Deactivate.");
            DeactivateProcessTimer();
            return base.OnDeactivateAsync();
        }

        private void DeactivateProcessTimer()
        {
            if (_processUploadTimer != null)
            {
                UnregisterTimer(_processUploadTimer);
            }
        }


        public Task InitJobActor(long studyId, long subjectId, Guid uploadId, Guid uploadProcessingId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {


            var tasks = new List<Task>();
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(StudyKey, studyId, (key, value) => studyId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(SubjectKey, subjectId, (key, value) => subjectId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(UploadKey, uploadId, (key, value) => uploadId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(UploadProcessingKey, uploadProcessingId, (key, value) => uploadProcessingId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(BeginKey, beginTimestampUtc, (key, value) => beginTimestampUtc));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(EndKey, endTimestampUtc, (key, value) => endTimestampUtc));
            tasks.Add(this.StateManager.SetStateAsync(ReadyToProcessKey, true));
            return Task.WhenAll(tasks);
        }

        private async Task ProcessUpload(object state)
        {
            if (await this.StateManager.GetStateAsync<bool>(ReadyToProcessKey)) 
            {
                // set ready to process to false to allow as to not enter this code twice //
                await this.StateManager.SetStateAsync(ReadyToProcessKey, false);
                var analyticsJobsService = FabricServices.GetAnalyticsJobsService();
                var studyId = await this.StateManager.GetStateAsync<long>(StudyKey);
                var subjectId = await this.StateManager.GetStateAsync<long>(SubjectKey);
                var beginTimestampUtc = await this.StateManager.GetStateAsync<DateTime>(BeginKey);
                var endTimestampUtc = await this.StateManager.GetStateAsync<DateTime>(EndKey);
                var jobActorLogic = new JobActorLogic();
                var uploadJobId = await this.StateManager.GetStateAsync<Guid>(UploadProcessingKey);
                var algorithmSettingEntities = await analyticsJobsService.GetAlgorithmSettingForStudyAsync(studyId);
                
                // get all algorithm tasks // 
                var jobAlgorithmTasks = jobActorLogic.GetAlgorithmTasksForStudy(algorithmSettingEntities, studyId, uploadJobId, beginTimestampUtc, endTimestampUtc);

                //update timerange for wear and sleep period calculations //
                jobAlgorithmTasks = jobActorLogic.UpdateAlgorithmTasksTimeRanges(jobAlgorithmTasks);

                // store all algorithm tasks //
                var algorithmTasks = await jobActorLogic.StoreAlgorithmTasks(jobAlgorithmTasks, analyticsJobsService);
                // start the task orchestrator //
                var taskOrchestrator = FabricServices.GetTaskOrchestratorActor(subjectId);
                await taskOrchestrator.InitTaskOrchestratorActor(studyId, subjectId, uploadJobId);
            }
        }

    }
}
