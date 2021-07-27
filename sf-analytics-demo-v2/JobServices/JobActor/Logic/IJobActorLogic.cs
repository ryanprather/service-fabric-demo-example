using AnalyticsJobsService.Interface;
using AnalyticsJobsService.Models;
using JobModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JobActor.Logic
{
    public interface IJobActorLogic
    {
        List<JobAlgorithmTask> GetAlgorithmTasksForStudy(AlgorithmSettingEntity[] algorithmSettingEntities, long studyId, Guid uploadJobId, DateTime beginTimestampUtc, DateTime endTimestampUtc);
        List<JobAlgorithmTask> UpdateAlgorithmTasksTimeRanges(List<JobAlgorithmTask> jobAlgorithmTasks);
        Task<List<JobAlgorithmTask>> StoreAlgorithmTasks(List<JobAlgorithmTask> jobAlgorithmTasks, IAnalyticsJobsService analyticsJobsService);
    }
}
