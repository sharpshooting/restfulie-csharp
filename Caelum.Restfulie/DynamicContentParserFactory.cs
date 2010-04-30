using Microsoft.Http;
using SharpShooting.Dynamic.Xml;

namespace Caelum.Restfulie
{
    /// <summary>
    /// This class creates instances of DynamicObjects which can parse some given content type. It
    /// ends-up defining the scope of the Accept parameter in the HttpRequestMessage given by the
    /// Restfulie client.
    /// </summary>
    public class DynamicContentParserFactory : IDynamicContentParserFactory
    {
        public dynamic New(HttpContent httpContent)
        {
            if (httpContent.ContentType == "application/xml")
                return new DynamicXmlObject(httpContent.ReadAsString());

            throw new MediaTypeNotSupportedException();
        }
    }
}