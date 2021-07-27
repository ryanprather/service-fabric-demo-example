using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class ProcessingJobEpochRetrievalEntity
    {
        public Guid Id { get; set; }
        public Guid UploadProcessingJobId { get; set; }
        public DateTime ProcessingStartedUtc { get; set; }
        public DateTime ProcessingCompletedDateTimeUtc { get; set; }
        public DateTime AdjustedBeginTimestampUtc { get; set; }
        public DateTime AdjustedEndTimestampUtc { get; set; }
        public bool IsError { get; set; }
        public string ErrorReason { get; set; }
    }
}
