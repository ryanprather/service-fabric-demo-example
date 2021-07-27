using System;
using System.Collections.Generic;
using System.Text;

namespace DustinTracyAlgorithm.Models
{
    public class DustinTracyBucket
    {
        public long TimeStamp { get; set; }
        public double Axis1Avg { get; set; }
        public DustinTracySleepEpochType? BlockType { get; set; }
        public IEnumerable<DustinTracyMinuteEpoch> Epochs { get; set; }
    }
}
