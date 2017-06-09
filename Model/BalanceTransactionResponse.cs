using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class BalanceTransactionResponse
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("assets")]
        public List<AssetBalanceResponse> Assets { get; set; }
    }
}
