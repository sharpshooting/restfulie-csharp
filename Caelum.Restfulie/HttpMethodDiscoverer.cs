using System;
using System.Collections.Generic;
using Microsoft.Http;

namespace Caelum.Restfulie
{
    public class HttpMethodDiscoverer : IHttpMethodDiscoverer
    {
        private readonly IDictionary<string, HttpMethod> _httpMethodLookupDictionary = new Dictionary<string, HttpMethod>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "cancel",  HttpMethod.DELETE },
            { "delete",  HttpMethod.DELETE },
            { "destroy", HttpMethod.DELETE },

            { "update", HttpMethod.PUT },

            { "latest",  HttpMethod.GET },
            { "refresh", HttpMethod.GET },
            { "reload",  HttpMethod.GET },
            { "show",    HttpMethod.GET }
        };

        public HttpMethod MethodFor(string rel)
        {
            if (rel == null)
                return HttpMethod.POST;

            HttpMethod httpMethod;

            // carlos.mendonca: HttpMethod.GET is the default(HttpMethod) value.
            if (!_httpMethodLookupDictionary.TryGetValue(rel, out httpMethod))
                httpMethod = HttpMethod.POST;

            return httpMethod;
        }
    }
}