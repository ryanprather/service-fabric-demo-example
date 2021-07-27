using System;

namespace AnalyticsJobsService.Models
{
    public class SubjectSleepPeriodStateEntity
    {
        public Guid Id { get; set; }
        public Guid SleepPeriodAlgorithmId {get;set;}
        public DateTime LastSleepPeriodStart { get; set; }
    }
}
