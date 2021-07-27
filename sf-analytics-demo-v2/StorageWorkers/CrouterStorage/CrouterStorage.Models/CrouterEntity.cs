using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CrouterStorage.Models
{
    public class CrouterEntity
    {
        public long SubjectId { get; set; }
        public string DeviceId { get; set; }
        public Guid SettingsId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string CutPointBucketVectorMagnitude { get; set; }
        public string CutPointBucketVerticalAxis { get; set; }
    }
}
