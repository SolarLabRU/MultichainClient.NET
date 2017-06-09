using Newtonsoft.Json;
using Platform.DataAccess.MultiChain.Model;

namespace Platform.DataAccess.MultiChain
{
    public class TransactionVin
    {
        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("vout")]
        public int Vout { get; set; }

        [JsonProperty("scriptsig")]
        public ScriptSigResponse ScriptSig { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }
    }
}
