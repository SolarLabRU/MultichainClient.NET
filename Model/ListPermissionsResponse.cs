﻿using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ListPermissionsResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("startblock")]
        public long StartBlock { get; set; }

        [JsonProperty("endblock")]
        public long EndBlock { get; set; }
    }
}
