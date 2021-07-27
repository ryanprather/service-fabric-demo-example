using System;

namespace CountsWorker.Models
{
    public class CountsWorkerTaskResult
    {
        public Guid TaskId { get; set; }
        public DateTime ProcessingStartedUtc { get; set; }
        public DateTime ProcessingEndedUtc { get; set; }
        public int ItemsComputed { get; set; }
        public string ErrorMessage { get; set; }
    }
}
