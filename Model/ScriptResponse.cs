using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ScriptResponse
    {
        [JsonProperty("asm")]
        public string Asm { get; set; }

        [JsonProperty("reqSigs")]
        public int ReqSigs { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("addresses")]
        public List<string> Addresses { get; set; }

        [JsonProperty("p2sh")]
        public string P2Sh { get; set; }

        public ScriptResponse()
        {
            this.Addresses = new List<string>();
        }
    }
}
