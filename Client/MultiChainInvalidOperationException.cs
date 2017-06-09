using System;
using Platform.DataAccess.MultiChain.Model;
using Newtonsoft.Json;

namespace Platform.DataAccess.MultiChain.Client
{
    /// <summary>
    /// Ошибка выполнения запроса в MultiChain
    /// </summary>
    public class MultiChainInvalidOperationException : InvalidOperationException
    {
        public string Details { get; set; }
        public string Url { get; set; }
        public string Json { get; set; }
        public JsonErrorResponse ErrorResponse { get; set; }

        public MultiChainInvalidOperationException(string message, string details, string url, string json) : this(message, details)
        {
            Url = url;
            Json = json;
        }


        public MultiChainInvalidOperationException(string message, string details) : this(ModifyMessage(message, details))
        {
            // Попробуем привести полученную информацию к структурированной ошибке
            try
            {
                var response = JsonConvert.DeserializeObject<JsonRpcResponse<object>>(details);
                ErrorResponse = response.Error;
            }
            catch
            {
                Details = details;
            }
        }

        public MultiChainInvalidOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MultiChainInvalidOperationException(JsonErrorResponse errorResponse)
            : base(errorResponse.Message)
        {
        }

        public MultiChainInvalidOperationException(string message) : base(message)
        {

        }

        private static string ModifyMessage(string message, string details)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<JsonRpcResponse<object>>(details);
                return response.Error.Message;
            }
            catch
            {
                return message;
            }
        }
    }
}
