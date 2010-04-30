using System;
using System.Dynamic;
using System.Net;
using System.Text;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpShooting.Dynamic.Xml;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class RestfulieTests
    {
        private Mock<IHttpClient> _httpClientMock;
        private Mock<IDynamicContentParserFactory> _dynamicContentParserFactoryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            _dynamicContentParserFactoryMock = new Mock<IDynamicContentParserFactory>();
        }

        private class DynamicObjectStub : DynamicObject { }

        [TestMethod]
        public void ShouldGETResourceOnUriWithRequestHeaders()
        {
            var theUri = new Uri("http://localhost");
            var theRequestHeaders = new RequestHeaders();

            const HttpMethod getHttpMethod = HttpMethod.GET;

            _httpClientMock.Setup(it => it.Send(getHttpMethod, theUri, theRequestHeaders)).Returns(new HttpResponseMessage());

            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicObjectStub());

            new Restfulie(_httpClientMock.Object, theRequestHeaders, _dynamicContentParserFactoryMock.Object).At(theUri);

            _httpClientMock.Verify(it => it.Send(getHttpMethod, theUri, theRequestHeaders), Times.Once());
        }

        [TestMethod]
        public void ShouldReturnRestfulieWithDynamicXmlObjectAsParserUponReceivingResponseWithXmlContentType()
        {
            const string applicationXmlContentType = "application/xml";
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order><id>1</id></order>";

            var requestHeaders = new RequestHeaders();
            requestHeaders.Accept.AddString(applicationXmlContentType);

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = HttpContent.Create(orderXml, Encoding.UTF8, applicationXmlContentType)
            };

            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), requestHeaders)).Returns(httpResponseMessage);

            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicXmlObject(orderXml));

            var restfulie = new Restfulie(_httpClientMock.Object, requestHeaders, _dynamicContentParserFactoryMock.Object);
            var order = restfulie.At(It.IsAny<Uri>());

            Assert.IsInstanceOfType(order, typeof(Restfulie));
            Assert.AreEqual("1", order.id);
        }

        [TestMethod]
        public void ShouldProvideHttpStatusCodeUponRecievingResponse()
        {
            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<RequestHeaders>())).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicObjectStub());

            var restfulie = new Restfulie(_httpClientMock.Object, It.IsAny<RequestHeaders>(), _dynamicContentParserFactoryMock.Object);
            var resource = restfulie.At(It.IsAny<Uri>());

            Assert.AreEqual("200", resource.StatusCode);
        }

        // TODO: carlos.mendonca: refactor tests to use this:
        // public void GivenHttpClientReturning(HttpResponseMessage httpResponseMessage, HttpMethod httpMethod = It.IsAny<HttpMethod>(), Uri uri = It.IsAny<Uri>(), RequestHeaders requestHeaders = It.IsAny<RequestHeaders>()) { }

        [TestMethod, Ignore]
        public void ShouldRejectUnsupportedContentTypes()
        {
        }
    }
}
