using System;
using System.Collections.Generic;
using System.Text;

namespace ChoiAlgorithm.Models
{
    public class ChoiWearTimeParameters
    {
        public int SmallWindowLengthInMinutes { get; set; }
        public int SpikeToleranceInMinutes { get; set; }
        public int MinimumLengthInMinutes { get; set; }
        public bool UseVM { get; set; }
        public DateTime? StartOfNonWearPeriod { get; set; }
    }
}
