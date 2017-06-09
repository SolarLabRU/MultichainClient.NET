using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class UnspentAssetResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("assetref")]
        public string AssetRef { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }
    }
}
