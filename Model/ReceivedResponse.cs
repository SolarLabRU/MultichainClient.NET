﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ReceivedResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("txids")]
        public List<string> TxIds { get; set; }

        public ReceivedResponse()
        {
            this.TxIds = new List<string>();
        }
    }
}
