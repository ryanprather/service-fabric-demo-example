using AnalyticsJobsService.Models;
using System;
using System.Collections.Generic;

namespace TaskOrchestrator.Models
{
    public class OrchestratorJob: IOrchestratorJob
    {
        public Guid JobId { get; set; }
        public long StudyId { get; set; }
        public long SubjectId { get; set; }
        public List<AlgorithmTaskEntity> AlgorithmTasks { get; set; }
    }

    public interface IOrchestratorJob
    {
        Guid JobId { get; set; }
        List<AlgorithmTaskEntity> AlgorithmTasks { get; set; }
    }
}
