using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Api.Models;

namespace Imagegram.Api.Helpers
{
    public interface IHttpHelper
    {
        public Task<Response> SendHttpRequest(string Url, Dictionary<string, string> Params);
    }
}
