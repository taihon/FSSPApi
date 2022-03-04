using Newtonsoft.Json;

namespace FSSPAPI.Core
{
    internal class SingleRequest
    {
        public int type { get; set; } = 1;
        [JsonProperty(PropertyName = "params")]
        public RequestParams Params { get; set; }
    }
}