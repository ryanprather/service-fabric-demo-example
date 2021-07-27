using AnalyticsJobsService.Models;
using Global.Constants;
using Global.Services;
using JobModels;
using JobsActorExternalService;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SubjectActor.Interfaces;
using SubjectActor.Logic;
using SubjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubjectActor
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
    internal class SubjectActor : Actor, ISubjectActor
    {
        private const string SubjectKey = "SubjectStateId";
        private const string StudyKey = "StudyStateId";
        private const string UploadsKey = "UploadsStateId";
        private const string ReadyToProcessKey = "ReadyToProcessStateId";
        
        private IActorTimer _processUploadTimer;
        private readonly IExternalJobsService _externalJobsService;
        private ISubjectActorLogic _workerLogic;

        // logging example // 
        private string GetUpoadProcessingMessage(long subjectId, Guid uploadId) => $"Subject Actor is Processing Upload for Subject: {subjectId} SubjectDeviceUploadId: {uploadId}";
        private string GetInitProcessingMessage(long subjectId) => $"Subject Actor Created for SubjectId: {subjectId}";
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of SubjectActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SubjectActor(ActorService actorService, ActorId actorId, IExternalJobsService externalJobsService) 
            : base(actorService, actorId)
        {
            _externalJobsService = externalJobsService;
            _telemetryClient = TelemetryClientHelper.GetActorTelemetryClient(actorService);
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Subject Actor Activated.");

            _workerLogic = new SubjectActorLogic(FabricServices.GetAnalyticsJobsService());
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

        public async Task InitSubjectActor(SubjectMdo subjectMdo)
        {
            // logging //
            ActorEventSource.Current.Message(GetInitProcessingMessage(subjectMdo.SubjectId));
            _telemetryClient.TrackTrace(GetInitProcessingMessage(subjectMdo.SubjectId));

            var subjectDevice = await _workerLogic.CreateSubjectIfNotExists(subjectMdo);
            var subjectDeviceEntity = await _workerLogic.CreateSubjectDeviceIfNotExist(subjectDevice, subjectMdo);
            var subjectUpload = await _workerLogic.CreateSubjectUploadIfNotExists(subjectDeviceEntity, subjectMdo.SubjectUpload);

            
            var tasks = new List<Task>();
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(StudyKey, subjectMdo.StudyId, (key, value) => subjectMdo.StudyId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(SubjectKey, subjectMdo.SubjectId, (key, value) => subjectMdo.SubjectId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(UploadsKey, subjectUpload, (key, value) => subjectUpload));
            tasks.Add(this.StateManager.SetStateAsync(ReadyToProcessKey, true));
            await Task.WhenAll(tasks);
        }

        private async Task ProcessUpload(object state) 
        {
            var test = await this.StateManager.GetStateAsync<bool>(ReadyToProcessKey);
            if (await this.StateManager.GetStateAsync<bool>(ReadyToProcessKey)) 
            {
                await this.StateManager.SetStateAsync(ReadyToProcessKey, false);
                var currentUpload = await this.StateManager.GetStateAsync<SubjectDeviceUploadEntity>(UploadsKey);
                var subjectId = await this.StateManager.GetStateAsync<long>(SubjectKey);
                
                // create job actor and pass info to job actor to process the job //
                var job = new JobDto() {
                    StudyId = await this.StateManager.GetStateAsync<long>(StudyKey),
                    SubjectId = subjectId,
                    UploadId = currentUpload.Id,
                    BeginTimestampUtc = currentUpload.BeginTimestampUtc,
                    EndTimestampUtc = currentUpload.EndTimestampUtc
                };

                await _externalJobsService.InitNewJob(job);
                
                // logging //
                ActorEventSource.Current.Message(GetUpoadProcessingMessage(subjectId, currentUpload.Id));
                _telemetryClient.TrackTrace(GetUpoadProcessingMessage(subjectId, currentUpload.Id));
            }
        }
    }
}
