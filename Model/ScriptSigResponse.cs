using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    public class ScriptSigResponse
    {
        [JsonProperty("asm")]
        public string Asm { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }
    }
}
