using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class AlgorithmTaskStorageUpdateDto
    {
        public Guid Id { get; set; }
        public int ItemsComputed { get; set; }
        public int ItemsStored { get; set; }
        public DateTime? StorageStartedUtc { get; set; }
        public DateTime? StorageCompletedUtc { get; set; }
    }
}
