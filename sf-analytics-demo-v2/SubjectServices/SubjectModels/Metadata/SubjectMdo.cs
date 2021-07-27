namespace SubjectModels
{
    public class SubjectMdo : ISubjectMdo
    {
        public long StudyId { get; set; }
        public long SubjectId { get; set; }
        public string DeviceSerial { get; set; }
        public SubjectUploadMdo SubjectUpload { get; set; }
    }

    public interface ISubjectMdo
    {
        public long StudyId { get; set; }
        public long SubjectId { get; set; }
        public string DeviceSerial { get; set; }
        public SubjectUploadMdo SubjectUpload { get; set; }
    }
}
