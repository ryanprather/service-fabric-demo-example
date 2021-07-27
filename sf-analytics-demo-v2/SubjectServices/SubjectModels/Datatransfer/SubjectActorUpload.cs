using System;
using System.Runtime.Serialization;

namespace SubjectModels
{
    [DataContract]
    public class SubjectActorUpload
    {
        [DataMember]
        public Guid UploadId { get; set; }
        [DataMember]
        public string DeviceSerial { get; set; }
        [DataMember]
        public DateTime BeginTimestampUtc { get; set; }
        [DataMember]
        public DateTime EndTimestampUtc { get; set; }
        public bool IsComplete { get; set; }
    }
}
