using System;
using System.Linq;
using System.Xml.Linq;
using SharpShooting.Dynamic;

namespace Caelum.Restfulie
{
    public class DynamicXmlContentParser : DynamicXmlObject, IDynamicContentParser
    {
        public DynamicXmlContentParser(string xml)
            : base(xml)
        {
        }

        public Uri UriFor(string stateTransition)
        {
            var xElementsToResolve = ShouldBypassRootElement ? XElements.Elements() : XElements;

            var xElement = (from element in xElementsToResolve
                            where element.Name == XName.Get("link", "http://www.w3.org/2005/Atom")
                            from attribute in element.Attributes("rel")
                            where attribute.Value == stateTransition && element.Attribute("href") != null
                            select element).FirstOrDefault();

            Uri uri = null;

            if (xElement != null)
            {
                var hrefAttributeValue = xElement.Attribute("href").Value;
                if (Uri.IsWellFormedUriString(hrefAttributeValue, UriKind.Absolute))
                    uri = new Uri(hrefAttributeValue);
            }

            return uri;
        }
    }
}