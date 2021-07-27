using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Api.Models
{
    public class BackfillDto
    {
        [JsonProperty(Required = Required.Always)]
        public int StudyId { get; set; }
    }
}
