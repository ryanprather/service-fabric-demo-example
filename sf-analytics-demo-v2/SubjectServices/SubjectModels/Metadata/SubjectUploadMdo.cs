using System;

namespace SubjectModels
{
    public class SubjectUploadMdo : ISubjectUploadMdo
    {
        public DateTime BeginTimestampUtc { get; set; }
        public DateTime EndTimestampUtc { get; set; }
    }

    public interface ISubjectUploadMdo
    {
        public DateTime BeginTimestampUtc { get; set; }
        public DateTime EndTimestampUtc { get; set; }
    }
}
