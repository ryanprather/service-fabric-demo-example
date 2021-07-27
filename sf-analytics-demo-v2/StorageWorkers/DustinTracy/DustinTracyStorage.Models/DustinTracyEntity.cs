using System;
using System.Collections.Generic;
using System.Text;

namespace DustinTracyStorage.Models
{
    public class DustinTracyEntity
    {
        public long SubjectId { get; set; }
        public string DeviceId { get; set; }
        public Guid SettingsId { get; set; }
        public DateTime SleepPeriodStartUtc { get; set; }
        public DateTime SleepPeriodEndUtc { get; set; }
    }
}
