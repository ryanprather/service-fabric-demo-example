using Global.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class AlgorithmTaskDto
    {
        public Guid Id { get; set; }
        public AnalyticsTypes AnalyticsTypeId { get; set; }
        public AlgorithmInputDataTypes AlgorithmInputDataType { get; set; }
        public Guid AlgorithmSettingId { get; set; }
        public DateTime AdjustedBeginTimestampUtc { get; set; }
        public DateTime AdjustedEndTimestampUtc { get; set; }
        public string Settings { get; set; }
        public string ErrorReason { get; set; }
        public bool IsError { get; set; }
    }
}
