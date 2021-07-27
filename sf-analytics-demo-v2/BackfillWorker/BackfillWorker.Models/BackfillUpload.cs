using System;

namespace BackfillWorker.Models
{
    public class BackfillUpload
    {
        public long SubjectId { get; set; }
        public string DeviceSerial { get; set; }
        public long UploadBeginTimestampUtc { get; set; }
        public long UploadEndTimestampUtc { get; set; }
    }
}
