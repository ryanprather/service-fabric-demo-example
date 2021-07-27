using System;
using System.Collections.Generic;
using System.Text;

namespace CountsStorage.Models
{
    public class CountsEntity
    {
        public long SubjectId { get; set; }
        public string DeviceId { get; set; }
        public Guid SettingsId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public int XAxis { get; set; }
        public int YAxis { get; set; }
        public int ZAxis { get; set; }
        
    }
}
