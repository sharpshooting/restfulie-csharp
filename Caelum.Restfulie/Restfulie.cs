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
        private readonly IDynamicContentParserFactory _dynamicContentParserFactory;

        private readonly dynamic _dynamicContentParser;

        public Restfulie(IHttpClient httpClient, RequestHeaders requestHeaders, IDynamicContentParserFactory dynamicContentParserFactory, dynamic dynamicContentParser = null)
        {
            _httpClient = httpClient;
            _requestHeaders = requestHeaders;
            _dynamicContentParserFactory = dynamicContentParserFactory;
            _dynamicContentParser = dynamicContentParser;
        }

        public dynamic At(Uri uri)
        {
            var httpResponseMessage = _httpClient.Send(HttpMethod.GET, uri, _requestHeaders);

            return new Restfulie(_httpClient, _requestHeaders, _dynamicContentParserFactory, _dynamicContentParserFactory.New(httpResponseMessage.Content));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _dynamicContentParser.TryGetMember(binder, out result);

            return true;
        }
    }
}