using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class NetTotalsResponse
    {
        [JsonProperty("totalbytesrecv")]
        public long TotalsBytesRecv { get; set; }

        [JsonProperty("totalbytessent")]
        public long TotalsBytesSent { get; set; }

        [JsonProperty("timemillis")]
        public long TimeMillis { get; set; }
    }
}
