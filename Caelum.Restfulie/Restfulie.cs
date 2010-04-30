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

        public IDynamicContentParser DynamicContentParser { get; set; }
        public HttpResponseMessage LatestHttpResponseMessage { get; set; }

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

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            //var uri = (DynamicContentParser as IDynamicContentParser).Link(binder.Name);

            //if (uri != null)
            //    _httpClient.Send(HttpMethod.GET, uri);

            result = null;
            return true;
        }
    }
}