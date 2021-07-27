using System;


namespace CrouterStorage.Models
{
    public class CrouterStorageDto
    {
        public Guid TaskId { get; set; }
        public long SubjectId { get; set; }
        public Guid SettingsId { get; set; }
        public string DeviceId { get; set; }
        public CrouterDto[] CrouterDto { get; set; }
    }

    public class CrouterDto 
    {
        public long TimestampUtc { get; set; }
        public string VerticalAxisResult { get; set; }
        public string VectorMagnitudeAxisResult { get; set; }
    }
}
