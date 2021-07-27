using AlgorithmWorker.Models;
using AnalyticsJobsService.Interface;
using AnalyticsJobsService.Models;
using ChoiWorkerActor.Interfaces;
using CountsWorkerActor.Interfaces;
using CrouterWorkerActor.Interfaces;
using DustinTracyWorkerActor.Interfaces;
using Global.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskOrchestratorActor.Logic
{
    public class TaskOrchestratorLogic: ITaskOrchestratorLogic
    {
        private readonly IAnalyticsJobsService _analyticsJobsService;
        public TaskOrchestratorLogic(IAnalyticsJobsService analyticsJobsService) 
        {
            _analyticsJobsService = analyticsJobsService;
        }

        public async Task ProcessTasks(Guid jobId, long subjectId, string deviceSerial, List<AlgorithmTaskDto> algorithmTasks)
        {
            // update upload processing job started //
            await _analyticsJobsService.SetProcessingJobStartedDateTime(DateTime.UtcNow, jobId);

            // if any tasks require epochs create workers and stream epochs to the workers //
            if (algorithmTasks.Any(x => x.AlgorithmInputDataType == Global.Constants.AlgorithmInputDataTypes.EPOCHDATA))
            {
                List<ICountsWorkerActor> countsWorkers = new List<ICountsWorkerActor>();
                List<IChoiWorkerActor> choiWorkers = new List<IChoiWorkerActor>();
                List<ICrouterWorkerActor> crouterWorkers = new List<ICrouterWorkerActor>();
                List<IDustinTracyWorkerActor> dustinTracyWorkers = new List<IDustinTracyWorkerActor>();

                var epochAlgorithmTasks = algorithmTasks.Where(x => x.AlgorithmInputDataType == Global.Constants.AlgorithmInputDataTypes.EPOCHDATA).ToArray();
                
                // init and create epoch workers // 
                foreach (var epochAlgTask in epochAlgorithmTasks) 
                {
                    switch (epochAlgTask.AnalyticsTypeId) 
                    {
                        case Global.Constants.AnalyticsTypes.EpochSummary:
                            //create worker //
                            var countsWorker = FabricServices.GetCountsWorkerActor(epochAlgTask.Id);
                            await countsWorker.InitAlgorithmWorker(epochAlgTask.AlgorithmSettingId, epochAlgTask.Id, subjectId, deviceSerial, epochAlgTask.Settings);
                            countsWorkers.Add(countsWorker);
                            break;
                        case Global.Constants.AnalyticsTypes.ChoiWearPeriods:
                            //create worker //
                            var choiWorker = FabricServices.GetChoiWorkerActor(epochAlgTask.Id);
                            await choiWorker.InitAlgorithmWorker(epochAlgTask.AlgorithmSettingId, epochAlgTask.Id, subjectId, deviceSerial, epochAlgTask.Settings);
                            choiWorkers.Add(choiWorker);
                            break;
                        case Global.Constants.AnalyticsTypes.CrouterChildCutpoints:
                            //create worker //
                            var crouterWorker = FabricServices.GetCrouterWorkerActor(epochAlgTask.Id);
                            await crouterWorker.InitAlgorithmWorker(epochAlgTask.AlgorithmSettingId, epochAlgTask.Id, subjectId, deviceSerial, epochAlgTask.Settings);
                            crouterWorkers.Add(crouterWorker);
                            break;
                        case Global.Constants.AnalyticsTypes.DustinTracySleepPeriods:
                            //create worker //
                            var dustinTracyWorker = FabricServices.GetDustinTracyWorkerActor(epochAlgTask.Id);
                            await dustinTracyWorker.InitAlgorithmWorker(epochAlgTask.AlgorithmSettingId, epochAlgTask.Id, subjectId, deviceSerial, epochAlgTask.Settings);
                            dustinTracyWorkers.Add(dustinTracyWorker);
                            break;
                    }
                }

                // create epoch worker and stream epochs to the workers // 
                var epochRetrievalActor = FabricServices.GetEpochRetrievalActor(subjectId);
                var epochRetrievalResult = await epochRetrievalActor.ProcessEpochRequest(subjectId, epochAlgorithmTasks);
                // update the database //
                await _analyticsJobsService
                    .CreateEpochRetrievalForProcessingJob(
                    jobId, 
                    epochRetrievalResult.ProcessingStartedUtc,
                    epochRetrievalResult.ProcessingEndedUtc,
                    epochRetrievalResult.AdjustedBeginTimestampUtc,
                    epochRetrievalResult.AdjustedEndTimestampUtc,
                    epochRetrievalResult.ErrorMessage);
                //////////////////////////////////////////////
                
                // send all task workers to go and process // 
                var algorithmProcessingTasks = new List<Task<AlgorithmWorkerTaskResult>>();
                countsWorkers.ForEach(x => { algorithmProcessingTasks.Add(x.ProcessAlgorithm()); });
                choiWorkers.ForEach(x => { algorithmProcessingTasks.Add(x.ProcessAlgorithm()); });
                crouterWorkers.ForEach(x => { algorithmProcessingTasks.Add(x.ProcessAlgorithm()); });
                dustinTracyWorkers.ForEach(x => { algorithmProcessingTasks.Add(x.ProcessAlgorithm()); });
                await Task.WhenAll(algorithmProcessingTasks);
                //////////////////////////////////////////////

                // update upload processing job started //
                await _analyticsJobsService.SetProcessingJobCompletedDateTime(DateTime.UtcNow, jobId);

            }

        }
    }
}
