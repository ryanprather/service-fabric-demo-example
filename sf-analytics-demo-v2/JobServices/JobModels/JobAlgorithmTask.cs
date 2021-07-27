using Global.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobModels
{
    public class JobAlgorithmTask
    {
        public Guid Id { get; set; }
        public Guid UploadProcessingJobId { get; set; }
        public Guid AlgorithmSettingId { get; set; }
        public AnalyticsTypes AnalyticsType { get; set; }
        public string AnalyticsSetting { get; set; }
        public DateTime AdjustedBeginTimestampUtc { get; set; }
        public DateTime AdjustedEndTimestampUtc { get; set; }
    }
}
