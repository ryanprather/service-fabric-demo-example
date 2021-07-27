using System;
using System.Collections.Generic;
using System.Text;

namespace ChoiStorage.Models
{
    public class ChoiEntity
    {
        public long SubjectId { get; set; }
        public string DeviceId { get; set; }
        public DateTime WearPeriodStartUtc { get; set; }
        public DateTime WearPeriodEndUtc { get; set; }
        public Guid SettingsId { get; set; }
    }
}
