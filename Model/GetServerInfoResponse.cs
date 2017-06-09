using System.Collections.Generic;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class GetServerInfoResponse
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("availableMethods")]
        public List<string> AvailableMethods { get; set; }

        public GetServerInfoResponse()
        {
            this.AvailableMethods = new List<string>();
        }
    }
}
