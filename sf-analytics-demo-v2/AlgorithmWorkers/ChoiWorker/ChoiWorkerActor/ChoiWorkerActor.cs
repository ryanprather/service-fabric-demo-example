using AlgorithmWorker.Models;
using AnalyticsJobsService.Interface;
using ChoiAlgorithm.Service;
using ChoiWorkerActor.Interfaces;
using ChoiWorkerActor.Logic;
using EpochRetrieval.Models;
using Global.Services;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChoiWorkerActor
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
    internal class ChoiWorkerActor : Actor, IChoiWorkerActor
    {
        public const string StudyKey = "StudyId";
        public const string SubjectKey = "SubjectId";
        public const string DeviceKey = "DeviceId";
        public const string SettingsKey = "SettingsIdLocation";
        public const string CountsLocation = "CountsArry";
        public const string ActorInitLocation = "ActorInit";
        public const string SettingsLocation = "SettingsId";
        public const string TaskLocation = "TaskId";

        private IChoiWorkerLogic _workerLogic;
        private IAnalyticsJobsService _analyticsJobsService;

        /// <summary>
        /// Initializes a new instance of ChoiWorkerActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ChoiWorkerActor(ActorService actorService, ActorId actorId) 
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
            _workerLogic = new ChoiWorkerLogic(new ChoiWearTimeService(), FabricServices.GetAlgorithmStateService(), FabricServices.GetChoiStorageService());
            _analyticsJobsService = FabricServices.GetAnalyticsJobsService(); 
            var tasks = new List<Task>();
            var countsArry = new List<EpochRecord>();
            tasks.Add(this.StateManager.AddStateAsync(CountsLocation, countsArry.ToArray()));

            return Task.WhenAll(tasks);
        }

        public Task InitAlgorithmWorker(Guid settingsId, Guid taskId, long subjectId, string deviceId, string algorithmSettings)
        {
            var tasks = new List<Task>();
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(TaskLocation, taskId, (key, value) => taskId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(SettingsKey, settingsId, (key, value) => settingsId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(SubjectKey, subjectId, (key, value) => subjectId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(DeviceKey, deviceId, (key, value) => deviceId));
            tasks.Add(this.StateManager.AddOrUpdateStateAsync(SettingsLocation, algorithmSettings, (key, value) => algorithmSettings));
            return Task.WhenAll(tasks);
        }

        public async Task AddEpochProcessingRecords(EpochRecord[] epochRecords)
        {
            var epochlist = new List<EpochRecord>();
            var countsArry = await this.StateManager.GetStateAsync<EpochRecord[]>(CountsLocation);
            epochlist.AddRange(countsArry);
            epochlist.AddRange(epochRecords);
            await this.StateManager.SetStateAsync(CountsLocation, epochlist.ToArray());
        }

        public async Task<AlgorithmWorkerTaskResult> ProcessAlgorithm()
        {
            var workerResult = new AlgorithmWorkerTaskResult() { ProcessingStartedUtc = DateTime.UtcNow };
            var taskId = await this.StateManager.GetStateAsync<Guid>(TaskLocation);
            try
            {
                await _analyticsJobsService.UpdateAlgorithmTaskStarted(taskId, DateTime.UtcNow);
                var countsArry = await this.StateManager.GetStateAsync<EpochRecord[]>(CountsLocation);
                var subjectId = await this.StateManager.GetStateAsync<long>(SubjectKey);
                var deviceId = await this.StateManager.GetStateAsync<string>(DeviceKey);
                var settingsId = await this.StateManager.GetStateAsync<Guid>(SettingsKey);                
                var settings = await this.StateManager.GetStateAsync<string>(SettingsLocation);
                
                var itemsCompleted = await _workerLogic.ProcessRecords(countsArry, subjectId, deviceId, settings, taskId, settingsId);
                await _analyticsJobsService.UpdateAlgorithmTaskCompleted(taskId, DateTime.UtcNow, itemsCompleted);
            }
            catch (Exception ex)
            {
                await _analyticsJobsService.UpdateAlgorithmTaskErrored(taskId, DateTime.UtcNow, ex.Message);
            }

            return workerResult;
        }
    }
}
