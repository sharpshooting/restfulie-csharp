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
        private readonly IDynamicContentParserFactory _dynamicContentParserFactory;
        private readonly IHttpMethodDiscovery _httpMethodDiscovery;

        public IDynamicContentParser DynamicContentParser { get; set; }
        public HttpResponseMessage LatestHttpResponseMessage { get; set; }

        public Restfulie(IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory, IHttpMethodDiscovery httpMethodDiscovery)
        {
            _httpClient = httpClient;
            _dynamicContentParserFactory = dynamicContentParserFactory;
            _httpMethodDiscovery = httpMethodDiscovery;
        }

        public dynamic At(Uri uri)
        {
            LatestHttpResponseMessage = _httpClient.Send(HttpMethod.GET, uri);

            return new Restfulie(_httpClient, _dynamicContentParserFactory, _httpMethodDiscovery)
                       {
                           LatestHttpResponseMessage = LatestHttpResponseMessage,
                           DynamicContentParser = _dynamicContentParserFactory.New(LatestHttpResponseMessage.Content)
                       };
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name.Equals("statuscode", StringComparison.InvariantCultureIgnoreCase))
                result = ((int)LatestHttpResponseMessage.StatusCode).ToString();
            else
                return DynamicContentParser.TryGetMember(binder, out result);

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            var uri = DynamicContentParser.UriFor(binder.Name);

            if (uri != null)
            {
                var httpMethod = _httpMethodDiscovery.MethodFor(binder.Name);

                var latestHttpResponseMessage = _httpClient.Send(httpMethod, uri);

                result = new Restfulie(_httpClient, _dynamicContentParserFactory, _httpMethodDiscovery)
                {
                    LatestHttpResponseMessage = latestHttpResponseMessage,
                    DynamicContentParser = _dynamicContentParserFactory.New(latestHttpResponseMessage.Content)
                };
            }

            return true;
        }
    }
}