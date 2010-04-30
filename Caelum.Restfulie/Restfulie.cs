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

        private DynamicObject DynamicContentParser { get; set; }
        private HttpResponseMessage LatestHttpResponseMessage { get; set; }

        public Restfulie(IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory)
        {
            _httpClient = httpClient;
            _dynamicContentParserFactory = dynamicContentParserFactory;
        }

        public dynamic At(Uri uri)
        {
            LatestHttpResponseMessage = _httpClient.Send(HttpMethod.GET, uri);

            return new Restfulie(_httpClient, _dynamicContentParserFactory)
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
    }
}