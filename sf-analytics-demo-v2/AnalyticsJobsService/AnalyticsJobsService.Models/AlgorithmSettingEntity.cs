using Global.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class AlgorithmSettingEntity
    {
        public Guid Id { get; set; }
        public AnalyticsTypes AnalyticsTypeId { get; set; }
        public string Name { get; set; }
        public string Settings { get; set; }
        public bool Deleted { get; set; }
        public bool IsStatelessAlgorithm { get; set; }
        public AlgorithmInputDataTypes AlgorithmInputDataType { get; set; }

    }
}
