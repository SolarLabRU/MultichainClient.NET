using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class AddressTransactionResponse
    {
        [JsonProperty("balance")]
        public BalanceTransactionResponse Balance { get; set; }

        [JsonProperty("myaddresses")]
        public List<string> MyAddresses { get; set; }

        [JsonProperty("addresses")]
        public List<string> Addresses { get; set; }

        [JsonProperty("permissions")]
        public List<PermissionsResponse> Permissions { get; set; }

        [JsonProperty("items")]
        public List<object> Items { get; set; }

        [JsonProperty("data")]
        public List<string> Data { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("blockhash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockindex")]
        public int BlockIndex { get; set; }

        [JsonProperty("blocktime")]
        public long BlockTime { get; set; }

        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("timereceived")]
        public long TimeReceived { get; set; }
    }
}
