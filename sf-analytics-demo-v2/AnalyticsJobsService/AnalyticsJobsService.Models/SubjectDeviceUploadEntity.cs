using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class SubjectDeviceUploadEntity
    {
        public Guid Id { get; set; }
        public Guid SubjectDeviceId {get;set;}
        public DateTime BeginTimestampUtc { get; set; }
        public DateTime EndTimestampUtc { get; set; }
    }
}
