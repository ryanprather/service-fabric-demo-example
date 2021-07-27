using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Api.Models
{
    public class UploadMetadataDto
    {
        [JsonProperty(Required = Required.Always)]
        public long StudyId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long SubjectId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DeviceSerial { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime BeginTimeStampUtc { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime EndTimeStampUtc { get; set; }
    }
}
