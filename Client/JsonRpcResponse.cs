using System;
using Newtonsoft.Json;
using Platform.DataAccess.MultiChain.Model;

namespace Platform.DataAccess.MultiChain.Client
{
    public class JsonRpcResponse<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("error")]
        public JsonErrorResponse Error { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonIgnore]
        public string RawJson { get; internal set; }

        public void AssertOk()
        {
            if (Error != null && Error.Code != 0)
                throw new MultiChainInvalidOperationException(Error);
        }
    }
}
