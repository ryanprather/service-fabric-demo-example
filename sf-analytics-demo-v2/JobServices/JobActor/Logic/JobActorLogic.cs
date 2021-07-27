using AnalyticsJobsService.Interface;
using AnalyticsJobsService.Models;
using JobModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobActor.Logic
{
    class JobActorLogic: IJobActorLogic
    {
        public List<JobAlgorithmTask> GetAlgorithmTasksForStudy(AlgorithmSettingEntity[] algorithmSettingEntities, long studyId, Guid uploadJobId, DateTime beginTimestampUtc, DateTime endTimestampUtc) 
        {
            return algorithmSettingEntities.ToList()
                .Select(x => new JobAlgorithmTask()
                {
                    UploadProcessingJobId = uploadJobId,
                    AnalyticsType = x.AnalyticsTypeId,
                    AlgorithmSettingId = x.Id,
                    AnalyticsSetting = x.Settings,
                    AdjustedBeginTimestampUtc = beginTimestampUtc,
                    AdjustedEndTimestampUtc = endTimestampUtc,
                }).ToList();
        }

        public List<JobAlgorithmTask> UpdateAlgorithmTasksTimeRanges(List<JobAlgorithmTask> jobAlgorithmTasks) 
        {
            return jobAlgorithmTasks;
        }

        public async Task<List<JobAlgorithmTask>> StoreAlgorithmTasks(List<JobAlgorithmTask> jobAlgorithmTasks, IAnalyticsJobsService analyticsJobsService) 
        {
            var algorithmTaskList = new List<JobAlgorithmTask>();

            foreach (var algorithmTask in jobAlgorithmTasks)
            {
                var algorithmTaskEntity = await analyticsJobsService.CreateNewAlgorithmTaskEntityAsync(algorithmTask.UploadProcessingJobId, algorithmTask.AlgorithmSettingId, algorithmTask.AdjustedBeginTimestampUtc, algorithmTask.AdjustedEndTimestampUtc);
                algorithmTask.Id = algorithmTaskEntity.Id;
                algorithmTaskList.Add(algorithmTask);
            }

            return algorithmTaskList;
        }

    }
}