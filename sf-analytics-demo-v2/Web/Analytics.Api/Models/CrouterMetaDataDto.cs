using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Api.Models
{
    public class CrouterMetaDataDto
    {
        public long SubjectId { get; set; }
        public long DeviceId { get; set; }
        public string AnalyticsSettings { get; set; }
    }
}
