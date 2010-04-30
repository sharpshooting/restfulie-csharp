﻿using System;
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

        private readonly dynamic _dynamicContentParser;

        private HttpResponseMessage LatestHttpResponseMessage { get; set; }

        public Restfulie(IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory, dynamic dynamicContentParser = null)
        {
            _httpClient = httpClient;
            _dynamicContentParserFactory = dynamicContentParserFactory;
            _dynamicContentParser = dynamicContentParser;
        }

        public dynamic At(Uri uri)
        {
            LatestHttpResponseMessage = _httpClient.Send(HttpMethod.GET, uri);

            return new Restfulie(
                _httpClient,
                _dynamicContentParserFactory,
                _dynamicContentParserFactory.New(LatestHttpResponseMessage.Content))
                {
                    LatestHttpResponseMessage = LatestHttpResponseMessage
                };
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // TODO: carlos.mendonca: find a way to access last httpResponseMessage. Does this mean that _dynamicContentParser becomes a Property which is resolved on call and accessing httpResponseMessage.Content?
            if (binder.Name.Equals("statuscode", StringComparison.InvariantCultureIgnoreCase))
                result = ((int)LatestHttpResponseMessage.StatusCode).ToString();
            else
                _dynamicContentParser.TryGetMember(binder, out result);

            return true;
        }
    }
}