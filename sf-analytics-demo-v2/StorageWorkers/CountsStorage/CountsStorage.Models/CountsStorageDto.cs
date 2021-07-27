using System;

namespace CountsStorage.Models
{
    public class CountsStorageDto
    {
        public Guid TaskId { get; set; }
        public long SubjectId { get; set; }
        public Guid SettingsId { get; set; }
        public string DeviceId { get; set; }
        public CountsDto[] CountsDto { get; set; }
    }

    public class CountsDto 
    {
        public long TimestampUnixUtc { get; set; }
        public int XAxis { get; set; }
        public int YAxis { get; set; }
        public int ZAxis { get; set; }
    }
}
