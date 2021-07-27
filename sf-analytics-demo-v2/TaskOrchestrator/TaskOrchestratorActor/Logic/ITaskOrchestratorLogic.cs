using AnalyticsJobsService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskOrchestratorActor.Logic
{
    public interface ITaskOrchestratorLogic
    {
        Task ProcessTasks(Guid jobId, long subjectId, string deviceSerial, List<AlgorithmTaskDto> algorithmTasks);
    }
}
