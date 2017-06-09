using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class MultiSigResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("redeemScript")]
        public string RedeemScript { get; set; }
    }
}
