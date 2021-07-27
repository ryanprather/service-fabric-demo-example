using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class SubjectUploadDto
    {
        public Guid Id { get; set; }
        public string DeviceSerial { get; set; }
        public DateTime BeginTimestampUtc { get; set; }
        public DateTime EndTimestampUtc { get; set; }
    }
}
