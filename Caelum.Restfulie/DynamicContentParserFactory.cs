using Microsoft.Http;
using SharpShooting.Dynamic;

namespace Caelum.Restfulie
{
    /// <summary>
    /// This class creates instances of DynamicObjects which can parse some given content type. It
    /// ends-up defining the scope of the Accept parameter in the HttpRequestMessage given by the
    /// Restfulie client.
    /// </summary>
    public class DynamicContentParserFactory : IDynamicContentParserFactory
    {
        public IDynamicContentParser New(HttpContent httpContent)
        {
            if (httpContent.ContentType == "application/xml")
                return new DynamicXmlContentParser(httpContent.ReadAsString());

            throw new MediaTypeNotSupportedException();
        }
    }
}