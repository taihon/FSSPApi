using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSSPAPI.Core
{
    public class GroupRequestResultResponse
    {
        public string status { get; set; }
        public int code { get; set; }
        public string exception { get; set; }
        public GroupRequestSubResponse Response { get; set; }
    }
    public class GroupRequestSubResponse
    {
        public string Status { get; set; }
        [JsonProperty(PropertyName = "task_start")]
        public string TaskStart { get; set; }
        [JsonProperty(PropertyName = "task_end")]
        public string TaskEnd { get; set; }
        public List<SingleRequestResultResponse> Result { get; set; }
    }

    public class SingleRequestResultResponse
    {
        public string Status { get; set; }
        public QueryParams Query { get; set; }
        public List<PhysicalResponse> Result { get; set; }
    }

    public class QueryParams
    {
        public string Type { get; set; }
        public PhysicalParams Params { get; set; }
    }
}
