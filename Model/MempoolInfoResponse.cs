using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class MempoolInfoResponse
    {
        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }
    }
}
