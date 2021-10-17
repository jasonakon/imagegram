using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Imagegram.Api.Models;
using Newtonsoft.Json;

namespace Imagegram.Api.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private readonly HttpClient client = new HttpClient();
        public async Task<Response> SendHttpRequest(string Url, Dictionary<string, string> Params)
        {
            
            var data = await client.GetAsync(QueryHelpers.AddQueryString(Url, Params));

            var responseString = await data.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<Response>(responseString);

            return response;
        }
    }
}
