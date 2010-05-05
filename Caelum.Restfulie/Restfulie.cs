using System;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    public static class Restfulie
    {
        public static IRestfulieProxyFactory At(string uri)
        {
            throw new NotImplementedException();
        }

        public static IRestfulieProxyFactory At(Uri uri, IHttpClient httpClient, IDynamicContentParserFactory dynamicContentParserFactory, IHttpMethodDiscoverer httpMethodDiscoverer)
        {
            return new RestfulieProxyFactory(uri, httpClient, dynamicContentParserFactory, httpMethodDiscoverer);
        }
    }
}