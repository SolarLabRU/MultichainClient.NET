using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ListStreamItemsResponseT<T>
    {
        [JsonProperty("publishers")]
        public List<string> Publishers { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("confirmactions")]
        public int ConfirmActions { get; set; }

        [JsonProperty("blocktime")]
        public int BlockTime { get; set; }

        [JsonProperty("txid")]
        public string TxId { get; set; }
    }

    public class DataTxOut
    {
        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("vout")]
        public int Vout { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}