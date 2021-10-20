using System;
using Newtonsoft.Json;

namespace Imagegram.Api.Models
{
    public class Request
    {
        public Request(int imageId, string comment)
        {
            ImageId = imageId;
            Comment = comment;
        }

        [JsonProperty(PropertyName = "image_id")]
        public int ImageId { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public String Comment { get; set; }
    }
}
