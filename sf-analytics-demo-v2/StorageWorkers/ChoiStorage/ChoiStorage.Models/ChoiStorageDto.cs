using System;

namespace ChoiStorage.Models
{
    public class ChoiStorageDto
    {
        public Guid TaskId { get; set; }
        public long SubjectId { get; set; }
        public Guid SettingsId { get; set; }
        public string DeviceId { get; set; }
        public ChoiWearPeriodDto[] WearPeriods { get; set; }
    }
}
