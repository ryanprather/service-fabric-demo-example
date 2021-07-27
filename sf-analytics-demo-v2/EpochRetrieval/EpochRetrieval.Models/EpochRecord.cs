using System;

namespace EpochRetrieval.Models
{
    public class EpochRecord
    {
        public long TimestampUnixUtc { get; set; }
        public int XAxisCounts { get; set; }
        public int YAxisCounts { get; set; }
        public int ZAxisCounts { get; set; }
        public DateTime TimestampUtc { get { return DateTimeOffset.FromUnixTimeSeconds(TimestampUnixUtc).DateTime; } }
    }
}
