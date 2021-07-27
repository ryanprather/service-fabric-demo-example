using System;

namespace AlgorithmWorker.Models
{
    public class AlgorithmWorkerTaskResult
    {
        public Guid TaskId { get; set; }
        public DateTime ProcessingStartedUtc { get; set; }
        public DateTime ProcessingEndedUtc { get; set; }
        public int ItemsComputed { get; set; }
        public string ErrorMessage { get; set; }
    }
}
