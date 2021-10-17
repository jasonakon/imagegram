using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.Api.Models
{
    public interface IHttpHelper
    {
        public Task<Response> SendHttpRequest(string Url, Dictionary<string, string> Params);
    }
}
