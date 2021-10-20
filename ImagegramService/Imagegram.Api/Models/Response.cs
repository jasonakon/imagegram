using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Imagegram.Api.Models
{
    public class Response
    {
        public Response(string status, string message)
        {
            Status = status;
            Message = message;
        }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public static implicit operator Task<object>(Response v)
        {
            throw new NotImplementedException();
        }
    }
}
