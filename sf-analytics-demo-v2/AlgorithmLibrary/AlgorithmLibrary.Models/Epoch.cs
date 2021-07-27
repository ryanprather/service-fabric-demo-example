using System;

namespace AlgorithmLibrary.Models
{
    public class Epoch
    {
        public long TimestampUnixUtc { get; set; }
        public int XAxisCounts { get; set; }
        public int YAxisCounts { get; set; }
        public int ZAxisCounts { get; set; }
        public double VectorMagintude { get { return Math.Sqrt(Math.Pow(XAxisCounts, 2) + Math.Pow(YAxisCounts, 2) + Math.Pow(ZAxisCounts, 2));} }
        public DateTime TimestampUtc { get { return DateTimeOffset.FromUnixTimeSeconds(TimestampUnixUtc).DateTime; } }
    }
}
