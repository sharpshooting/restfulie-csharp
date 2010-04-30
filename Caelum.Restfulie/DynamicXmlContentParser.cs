using System;
using System.Linq;
using System.Xml.Linq;

namespace Caelum.Restfulie
{
    public class DynamicXmlContentParser : SharpShooting.Dynamic.DynamicXmlObject, IDynamicContentParser
    {
        public DynamicXmlContentParser(string xml)
            : base(xml)
        {
        }

        public Uri UriFor(string stateTransition)
        {
            var xElement = (from element in XElement.Elements(XName.Get("link", "http://www.w3.org/2005/Atom"))
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