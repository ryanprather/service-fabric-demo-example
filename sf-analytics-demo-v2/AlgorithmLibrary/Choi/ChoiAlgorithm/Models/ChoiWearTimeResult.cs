using System;
using System.Collections.Generic;
using System.Text;

namespace ChoiAlgorithm.Models
{
    public class ChoiWearTimeResult
    {
        public ChoiWearPeriod[] WearPeriods { get; set; }
        public DateTime[] NextStartTime { get; set; }
    }

    public class ChoiWearPeriod 
    {
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
    }

}
