using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class NetworkInfoResponse
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("subversion")]
        public string Subversion { get; set; }

        [JsonProperty("protocolversion")]
        public int ProtocolVersion { get; set; }

        [JsonProperty("localservices")]
        public string LocalServices { get; set; }

        [JsonProperty("timeoffset")]
        public int TimeOffset { get; set; }

        [JsonProperty("connections")]
        public int Connections { get; set; }

        [JsonProperty("networks")]
        public List<NetworkResponse> Networks { get; set; }

        [JsonProperty("relayfee")]
        public decimal RelayFee { get; set; }

        [JsonProperty("localaddresses")]
        public List<string> LocalAddresses { get; set; }

        public NetworkInfoResponse()
        {
            this.Networks = new List<NetworkResponse>();
            this.LocalAddresses = new List<string>();
        }
    }
}
