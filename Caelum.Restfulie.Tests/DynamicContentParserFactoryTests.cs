using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpShooting.Dynamic.Xml;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class DynamicContentParserFactoryTests
    {
        private readonly Encoding _anyEncoding = Encoding.Default;

        [TestMethod]
        public void ShouldReturnDynamicXmlObjectOnApplicationXmlContentType()
        {
            const string anyValidXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<root/>";
            var httpContent = HttpContent.Create(anyValidXml, _anyEncoding, "application/xml");

            dynamic dynamicObject = new DynamicContentParserFactory().New(httpContent);

            Assert.IsInstanceOfType(dynamicObject, typeof(DynamicXmlObject));
        }

        [TestMethod, ExpectedException(typeof(MediaTypeNotSupportedException))]
        public void ShouldThrowMediaTypeNotSupportedExceptionOnUnkownContentType()
        {
            var httpContent = HttpContent.Create(String.Empty, _anyEncoding, "application/unknown");

            new DynamicContentParserFactory().New(httpContent);
        }
    }

}
