using System;
using System.Dynamic;
using Microsoft.Http;
using SharpShooting.Http;

namespace Caelum.Restfulie
{
    public class RestfulieProxy : DynamicObject
    {
        private readonly IHttpClient _httpClient;
        private readonly IDynamicContentParserFactory _dynamicContentParserFactory;
        private readonly IHttpMethodDiscoverer _httpMethodDiscoverer;

        public IDynamicContentParser DynamicContentParser { get; set; }
        public HttpResponseMessage LatestHttpResponseMessage { get; set; }

        public RestfulieProxy(IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory, IHttpMethodDiscoverer httpMethodDiscoverer)
        {
            _httpClient = httpClient;
            _dynamicContentParserFactory = dynamicContentParserFactory;
            _httpMethodDiscoverer = httpMethodDiscoverer;
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
                var httpMethod = _httpMethodDiscoverer.MethodFor(binder.Name);

                var latestHttpResponseMessage = _httpClient.Send(httpMethod, uri, null, null);

                result = new RestfulieProxy(_httpClient, _dynamicContentParserFactory, _httpMethodDiscoverer)
                {
                    LatestHttpResponseMessage = latestHttpResponseMessage,
                    DynamicContentParser = _dynamicContentParserFactory.New(latestHttpResponseMessage.Content)
                };
            }

            return true;
        }
    }
}