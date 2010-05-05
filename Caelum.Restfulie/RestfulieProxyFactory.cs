using System;
using Microsoft.Http;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    public class RestfulieProxyFactory : IRestfulieProxyFactory
    {
        private readonly Uri _uri;
        private readonly IHttpClient _httpClient;
        private readonly IDynamicContentParserFactory _dynamicContentParserFactory;
        private readonly IHttpMethodDiscoverer _httpMethodDiscoverer;

        public RestfulieProxyFactory(Uri uri, IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory, IHttpMethodDiscoverer httpMethodDiscoverer)
        {
            _uri = uri;
            _httpClient = httpClient;
            _dynamicContentParserFactory = dynamicContentParserFactory;
            _httpMethodDiscoverer = httpMethodDiscoverer;
        }

        public RestfulieProxy Get()
        {
            var httpResponseMessage = _httpClient.Send(HttpMethod.GET, _uri);

            return new RestfulieProxy(_httpClient, _dynamicContentParserFactory, _httpMethodDiscoverer)
                       {
                           LatestHttpResponseMessage = httpResponseMessage,
                           DynamicContentParser = _dynamicContentParserFactory.New(httpResponseMessage.Content)
                       };
        }

        public RestfulieProxy Create(object content)
        {
            throw new NotImplementedException();
        }
    }
}