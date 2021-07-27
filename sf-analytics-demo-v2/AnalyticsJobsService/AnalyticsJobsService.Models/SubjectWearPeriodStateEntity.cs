using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class SubjectWearPeriodStateEntity
    {
        public Guid Id { get; set; }
        public Guid WearPeriodAlgorithmId { get; set; }
        public DateTime LastWearPeriodStart { get; set; }
    }
}
