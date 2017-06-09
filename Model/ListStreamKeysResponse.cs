using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ListStreamKeysResponse
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("items")]
        public int Items { get; set; }

        [JsonProperty("confirmed")]
        public int Confirmed { get; set; }

    }
}