using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imagegram.Api.JSON
{
    public class Request
    {
        [JsonProperty(PropertyName = "image_id")]
        public int ImageId { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public String Comment { get; set; }
    }
}
