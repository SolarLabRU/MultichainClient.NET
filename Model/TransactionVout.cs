using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class TransactionVout
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("scriptPubKey")]
        public ScriptPubKeyResponse ScriptPubKey { get; set; }

        [JsonProperty("assets")]
        public List<object> Assets { get; set; }

        [JsonProperty("permissions")]
        public List<PermissionsResponse> Permissions { get; set; }

        public TransactionVout()
        {
            this.Assets = new List<object>();
            this.Permissions = new List<PermissionsResponse>();
        }
    }
}
