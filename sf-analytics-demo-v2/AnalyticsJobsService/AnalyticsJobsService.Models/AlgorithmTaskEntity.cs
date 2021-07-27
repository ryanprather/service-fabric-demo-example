using System;

namespace AnalyticsJobsService.Models
{
    public class AlgorithmTaskEntity
    {
        public Guid Id { get; set; }
        public Guid UploadProcessingJobId { get; set; }
        public Guid AlgorithmSettingId { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime ProcessingStartedUtc { get; set; }
        public DateTime CompletedDateTimeUtc { get; set; }
        public DateTime AdjustedBeginTimestampUtc { get; set; }
        public DateTime AdjustedEndTimestampUtc { get; set; }
        public bool IsError { get; set; }
        public string ErrorReason { get; set; }
    }
}
