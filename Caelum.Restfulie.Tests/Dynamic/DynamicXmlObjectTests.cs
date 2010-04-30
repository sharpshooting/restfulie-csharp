using System;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShooting.Tests;

namespace Caelum.Restfulie.Tests.Dynamic
{
    [TestClass]
    public class DynamicXmlObjectTests
    {
        private const string XmlHeader = "<?xml version='1.0' encoding='UTF-8'?>\r\n";

        [TestMethod]
        public void ShouldGetUriFromAtomLink()
        {
            const string xml = XmlHeader + "<order><atom:link rel='refresh' href='http://localhost/orders/1' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            var uri = new DynamicXmlContentParser(xml).UriFor("refresh");

            uri.ShouldBeEqualTo(new Uri("http://localhost/orders/1"));
        }

        [TestMethod]
        public void ShouldGetNullUriIfAtomLinkDoesNotExist()
        {
            const string xml = XmlHeader + "<order></order>";

            var uri = new DynamicXmlContentParser(xml).UriFor("refresh");

            uri.ShouldBeNull();
        }

        [TestMethod]
        public void ShouldGetNullUriIfAtomLinkIsMalformedWithLackingRelAttribute()
        {
            const string xml = XmlHeader + "<order><atom:link href='http://localhost/orders/1' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            var uri = new DynamicXmlContentParser(xml).UriFor("refresh");

            uri.ShouldBeNull();
        }

        [TestMethod]
        public void ShouldGetNullUriIfAtomLinkIsMalformedWithLackingHrefAttribute()
        {
            const string xml = XmlHeader + "<order><atom:link rel='refresh' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            var uri = new DynamicXmlContentParser(xml).UriFor("refresh");

            uri.ShouldBeNull();
        }

        [TestMethod]
        public void ShouldGetNullUriIfHrefAttributeValueIsNotAValidUri()
        {
            const string xml = XmlHeader + "<order><atom:link rel='refresh' href='' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            var uri = new DynamicXmlContentParser(xml).UriFor("refresh");

            uri.ShouldBeNull();
        }

        [TestMethod]
        public void ShouldGetFirstAtomLinkEvenIfTheyAreNotUnique()
        {
            const string xml = XmlHeader + "<order><atom:link rel='refresh' href='http://localhost/orders/1' xmlns:atom='http://www.w3.org/2005/Atom'/><atom:link rel='refresh' href='http://localhost/orders/2' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            var uri = new DynamicXmlContentParser(xml).UriFor("refresh");

            uri.ShouldBeEqualTo(new Uri("http://localhost/orders/1"));
        }
    }
}