using Global.Services;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskOrchestratorActor.Interfaces;
using TaskOrchestratorActor.Logic;

namespace TaskOrchestratorActor
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
    internal class TaskOrchestratorActor : Actor, ITaskOrchestratorActor
    {
        private const string JobKey = "JobIdKey";
        private const string StudyKey = "StudyIdKey";
        private const string SubjectKey = "StudyIdKey";
        private const string ReadyToProcessKey = "ReadyToProcessStateId";
        private IActorTimer _processTasksTimer;
        private ITaskOrchestratorLogic _taskOrchestratorLogic;


        // logging example // 
        private string GetOrchProcessingMessage(long subjectId, Guid uploadId, TimeSpan timeSpan) => $"Task Orchestrator is Processing Upload for Subject: {subjectId} SubjectDeviceUploadId: {uploadId} ProcessingTimeInSeconds: {timeSpan.TotalSeconds}";       
        private string GetInitProcessingMessage(long subjectId) => $"Task Orchestrator Actor Created for SubjectId: {subjectId}";

        /// <summary>
        /// Initializes a new instance of TaskOrchestratorActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public TaskOrchestratorActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            _taskOrchestratorLogic = new TaskOrchestratorLogic(FabricServices.GetAnalyticsJobsService());
            
                _processTasksTimer = RegisterTimer(
            ProcessTasks,                     // Callback method
            null,                           // Parameter to pass to the callback method
            TimeSpan.FromMilliseconds(20),  // Amount of time to delay before the callback is invokeds
            TimeSpan.FromSeconds(10)); // Time interval between invocations of the callback method

            return this.StateManager.AddStateAsync(ReadyToProcessKey, false);
        }

        protected override Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Subject Actor Deactivate.");
            DeactivateProcessTimer();
            return base.OnDeactivateAsync();
        }

        private void DeactivateProcessTimer()
        {
            if (_processTasksTimer != null)
            {
                UnregisterTimer(_processTasksTimer);
            }
        }

        public Task InitTaskOrchestratorActor(long studyId, long subjectId, Guid jobId)
        {
            // logging //
            ActorEventSource.Current.Message(GetInitProcessingMessage(subjectId));

            var tasks = new List<Task>();
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(StudyKey, studyId, (key, value) => studyId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(SubjectKey, subjectId, (key, value) => subjectId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(JobKey, jobId, (key, value) => jobId));
            tasks.Add(this.StateManager.SetStateAsync(ReadyToProcessKey, true));
            return Task.WhenAll(tasks);
        }

        private async Task ProcessTasks(object state)
        {
            if (await this.StateManager.GetStateAsync<bool>(ReadyToProcessKey))
            {
                try 
                {
                    await this.StateManager.SetStateAsync(ReadyToProcessKey, false);
                    // get job algorithm tasks //
                    var uploadJobId = await this.StateManager.GetStateAsync<Guid>(JobKey);
                    var subjectId = await this.StateManager.GetStateAsync<long>(SubjectKey);
                    var analyticsJobsService = FabricServices.GetAnalyticsJobsService();

                    // process algorithm tasks // 
                    var analyticsTasks = await analyticsJobsService.GetAlgorithmTaskDtos(uploadJobId);
                    var uploadProcessingJob = await analyticsJobsService.GetUploadProcessingJob(uploadJobId);
                    var subjectDevice = await analyticsJobsService.GetSubjectUploadWithDevice(uploadProcessingJob.SubjectDeviceUploadId);
                    
                    await _taskOrchestratorLogic.ProcessTasks(uploadJobId, subjectId, subjectDevice.DeviceSerial, analyticsTasks.ToList());
                    
                }
                catch (Exception ex) 
                {
                    var test = ex;
                }
                
            }
        }


    }
}
