using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ListSinceLastBlockResponse 
    {
        [JsonProperty("transactions")]
        public List<TransactionResponse> Transactions { get; set; }

        public ListSinceLastBlockResponse()
        {
            this.Transactions = new List<TransactionResponse>();
        }
    }
}
