using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class SubjectDeviceEntity
    {
        public Guid Id { get; set; }
        public Guid StudySubjectId {get;set;}
        public string DeviceSerial { get; set; }
    }
}
