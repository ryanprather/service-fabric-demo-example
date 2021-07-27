using System;

namespace DustinTracyStorage.Models
{
    public class DustinTracyStorageDto
    {
        public Guid TaskId { get; set; }
        public long SubjectId { get; set; }
        public Guid SettingsId { get; set; }
        public string DeviceId { get; set; }
        public DustinTracySleepPeriodDto[] SleepPeriods { get; set; }
    }
}
