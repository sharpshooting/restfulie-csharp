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
        private readonly RequestHeaders _requestHeaders;

        public Restfulie(IHttpClient httpClient, RequestHeaders requestHeaders)
        {
            _httpClient = httpClient;
            _requestHeaders = requestHeaders;
        }

        public dynamic At(Uri uri)
        {
            _httpClient.Send(HttpMethod.GET, uri, _requestHeaders);

            return null;
        }
    }
}