using System;

namespace JobModels
{
    public class JobDto: IJobDto
    {
        public long StudyId { get; set; }
        public long SubjectId { get; set; }
        public Guid UploadId { get; set; }
        public DateTime BeginTimestampUtc { get; set; }
        public DateTime EndTimestampUtc { get; set; }
    }

    public interface IJobDto 
    {
        public long StudyId { get; set; }
        public long SubjectId { get; set; }
        public Guid UploadId { get; set; }
        public DateTime BeginTimestampUtc { get; set; }
        public DateTime EndTimestampUtc { get; set; }
    }
}
