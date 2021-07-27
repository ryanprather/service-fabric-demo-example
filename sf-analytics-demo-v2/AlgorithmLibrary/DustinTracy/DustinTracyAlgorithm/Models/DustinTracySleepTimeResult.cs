using System;

namespace DustinTracyAlgorithm.Models
{

    public class DustinTracySleepTimeResult
    {
        public SleepPeriod[] SleepPeriods { get; set; }
        public SleepPeriodState[] SleepPeriodStates { get; set; }
    }

    public class DustinTracySleepTimeOutput
    {
        public DateTimeOffset SleepPeriodBegin { get; set; }
        public DateTimeOffset SleepPeriodEnd { get; set; }
        public bool IsComplete { get; set; }
        public DateTimeOffset DataStartTimestamp { get; set; }
        public DateTimeOffset DataEndTimestamp { get; set; }
    }

    public class SleepPeriod 
    {
        public DateTime SleepPeriodBegin { get; set; }
        public DateTime SleepPeriodEnd { get; set; }
        public bool IsComplete { get; set; }
    }

    public class SleepPeriodState 
    {
        public DateTime DataStartTimestamp { get; set; }
        public DateTime DataEndTimestamp { get; set; }
    }
}
