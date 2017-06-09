using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Model
{
    /// <summary>
    /// Описывает структуру возвращаемой ошибки в json формате
    /// </summary>
    public class JsonErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
