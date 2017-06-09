using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class TxOutResponse
    {
        [JsonProperty("bestblock")]
        public string BestBlock { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("scriptPubKey")]
        public ScriptPubKeyResponse ScriptPubKey { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("coinbase")]
        public bool Coinbase { get; set; }

        [JsonProperty("assets")]
        public List<TxAssetResponse> Assets { get; set; }

        [JsonProperty("permissions")]
        public List<object> Permissions { get; set; }

        public TxOutResponse()
        {
            this.Assets = new List<TxAssetResponse>();
            this.Permissions = new List<object>();
        }
    }
}
