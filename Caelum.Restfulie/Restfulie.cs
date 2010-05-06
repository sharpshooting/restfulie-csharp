using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    public class Restfulie : IRestfulieProxyFactory
    {
        private readonly Uri _uri;
        private readonly IHttpClient _httpClient;
        private readonly IDynamicContentParserFactory _dynamicContentParserFactory;
        private readonly IHttpMethodDiscoverer _httpMethodDiscoverer;
        
        private string _contentType;

        public Restfulie(Uri uri, IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory, IHttpMethodDiscoverer httpMethodDiscoverer)
        {
            _uri = uri;
            _httpClient = httpClient;
            _dynamicContentParserFactory = dynamicContentParserFactory;
            _httpMethodDiscoverer = httpMethodDiscoverer;
        }

        public RestfulieProxy Get()
        {
            var httpResponseMessage = _httpClient.Send(HttpMethod.GET, _uri);

            return NewRestfulieProxy(httpResponseMessage);
        }

        public RestfulieProxy Create(object content)
        {
            var requestHeaders = new RequestHeaders { ContentType = _contentType };

            var httpResponseMessage = _httpClient.Send(HttpMethod.POST, _uri, requestHeaders, HttpContent.Create(content.ToString()));

            return NewRestfulieProxy(httpResponseMessage);
        }

        public IRestfulieProxyFactory As(string contentType)
        {
            _contentType = contentType;
            return this;
        }

        private RestfulieProxy NewRestfulieProxy(HttpResponseMessage httpResponseMessage)
        {
            return new RestfulieProxy(_httpClient, _dynamicContentParserFactory, _httpMethodDiscoverer)
            {
                LatestHttpResponseMessage = httpResponseMessage,
                DynamicContentParser = _dynamicContentParserFactory.New(httpResponseMessage.Content)
            };
        }

        public static IRestfulieProxyFactory At(string uri)
        {
            throw new NotImplementedException();
        }
    }
}