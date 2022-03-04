using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSSPAPI.Core
{
    public class PhysicalResponse
    {
        public string Name { get; set; }
        [JsonProperty(PropertyName ="exe_production")]
        public string ExecutiveProduction { get; set; }
        public string Details { get; set; }
        public string Subject { get; set; }
        public string Department { get; set; }
        public string Bailiff { get; set; }
        [JsonProperty(PropertyName ="ip_end")]
        public string IPEnd { get; set; }
    }
}
