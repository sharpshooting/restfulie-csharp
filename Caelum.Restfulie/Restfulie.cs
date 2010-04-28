using System;
using System.Dynamic;
using Microsoft.Http;
using Microsoft.Http.Headers;
using SharpShooting.Http;

namespace Caelum.Restfulie
{
    public class Restfulie : DynamicObject
    {
        private readonly IHttpClient _httpClient;

        public Restfulie(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public dynamic At(Uri uri)
        {
            _httpClient.Send(HttpMethod.GET, uri, new RequestHeaders());

            return null;
        }
    }
}