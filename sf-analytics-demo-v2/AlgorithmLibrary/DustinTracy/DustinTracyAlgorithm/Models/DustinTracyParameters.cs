using System;
using System.Collections.Generic;
using System.Text;

namespace DustinTracyAlgorithm.Models
{
    public class DustinTracyParameters
    {
        public int BlockLengthInMinutes { get; set; }
        public int ThresholdCountsPerMinute { get; set; }
        public int BedRestStartTriggerCountsPerMinute { get; set; }
        public int BedRestEndTriggerCountsPerMinute { get; set; }
        public int MinimumBedRestLengthInMinutes { get; set; }
        public int MaxSleepPeriodLengthInMinutes { get; set; }
        public int? MinimumNonZeroEpochs { get; set; }
    }
}
