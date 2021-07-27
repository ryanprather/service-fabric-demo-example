using System;
using System.Collections.Generic;
using System.Text;

namespace EpochRetrieval.Models
{
    public class EpochRetrievalTaskResult
    {
        public DateTime ProcessingStartedUtc { get; set; }
        public DateTime ProcessingEndedUtc { get; set; }
        public DateTime AdjustedBeginTimestampUtc { get; set; }
        public DateTime AdjustedEndTimestampUtc { get; set; }
        public string ErrorMessage { get; set; }
    }
}
