using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ScriptPubKeyResponse
    {
        [JsonProperty("asm")]
        public string Asm { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }

        [JsonProperty("reqSigs")]
        public int ReqSigs { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("addresses")]
        public List<string> Addresses { get; set; }

        public ScriptPubKeyResponse()
        {
            this.Addresses = new List<string>();
        }
    }
}
