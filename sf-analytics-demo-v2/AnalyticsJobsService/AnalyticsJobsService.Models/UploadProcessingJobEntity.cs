using System;

namespace AnalyticsJobsService.Models
{
    public class UploadProcessingJobEntity
    {
        public Guid Id { get; set; }
        public Guid SubjectDeviceUploadId { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime ProcessingStartedUtc { get; set; }
        public DateTime CompletedDateTimeUtc { get; set; }
        public bool IsError { get; set; }
        public string ErrorReason { get; set; }
    }
}
