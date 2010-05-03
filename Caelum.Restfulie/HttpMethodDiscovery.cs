using System;
using System.Collections.Generic;
using Microsoft.Http;

namespace Caelum.Restfulie
{
    public class HttpMethodDiscovery : IHttpMethodDiscovery
    {
        private readonly IDictionary<string, HttpMethod> _httpMethodLookupDictionary = new Dictionary<string, HttpMethod>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "delete",  HttpMethod.DELETE },
            { "cancel",  HttpMethod.DELETE },
            { "destroy", HttpMethod.DELETE },

            { "post",   HttpMethod.POST },
            { "update", HttpMethod.POST }
        };

        public HttpMethod MethodFor(string rel)
        {
            if (rel == null)
                return HttpMethod.GET;

            HttpMethod httpMethod;

            // carlos.mendonca: HttpMethod.GET is the default(HttpMethod) value.
            _httpMethodLookupDictionary.TryGetValue(rel, out httpMethod);

            return httpMethod;
        }
    }
}