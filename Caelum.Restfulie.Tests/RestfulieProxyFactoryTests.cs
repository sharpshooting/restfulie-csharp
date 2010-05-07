using System;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class RestfulieProxyFactoryTests
    {
        private Mock<IHttpClient> _httpClientMock;
        private Mock<IDynamicContentParserFactory> _dynamicContentParserFactoryMock;
        private Mock<IHttpMethodDiscoverer> _httpMethodDiscovererMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            _dynamicContentParserFactoryMock = new Mock<IDynamicContentParserFactory>();
            _httpMethodDiscovererMock = new Mock<IHttpMethodDiscoverer>();
        }

        [TestMethod]
        public void ShouldDoAGetHttpRequestToUri()
        {
            const HttpMethod theGetHttpMethod = HttpMethod.GET;
            var theUri = new Uri("http://localhost");

            _httpClientMock.Setup(it => it.Send(theGetHttpMethod, theUri, It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>())).Returns(new HttpResponseMessage());

            new Restfulie(theUri, _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscovererMock.Object)
                .Get();

            _httpClientMock.Verify(it => it.Send(theGetHttpMethod, theUri, It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>()), Times.Once());
        }

        [TestMethod]
        public void ShouldSetHttpResponseMessageToRestfulieProxy()
        {
            var httpResponseMessage = new HttpResponseMessage();

            _httpClientMock.SetupHttpClientMock(httpResponseMessage);

            var resource = new Restfulie(It.IsAny<Uri>(), _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscovererMock.Object)
                .Get();

            Assert.AreSame(httpResponseMessage, resource.LatestHttpResponseMessage);
        }

        [TestMethod]
        public void ShouldReturnRestfulieProxyWithDynamicXmlContentParserUponRecievingResponseWithXmlContentTypeOnGet()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<resource/>";

            _httpClientMock.SetupHttpClientMock(new HttpResponseMessage());

            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicXmlContentParser(orderXml));

            var resource = new Restfulie(It.IsAny<Uri>(), _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscovererMock.Object)
                .Get();

            Assert.IsInstanceOfType(resource.DynamicContentParser, typeof(DynamicXmlContentParser));
        }

        [TestMethod]
        public void ShouldDoAPostHttpRequestToUri()
        {
            const HttpMethod thePostHttpMethod = HttpMethod.POST;
            var theUri = new Uri("http://localhost");

            var anyContent = new object();

            _httpClientMock.Setup(it => it.Send(thePostHttpMethod, theUri, It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>())).Returns(new HttpResponseMessage());

            new Restfulie(theUri, _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscovererMock.Object)
                .Create(anyContent);

            _httpClientMock.Verify(it => it.Send(thePostHttpMethod, theUri, It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>()), Times.Once());
        }

        [TestMethod]
        public void ShouldReturnRestfulieProxyWithDynamicXmlContentParserUponRecievingResponseWithXmlContentTypeOnCreate()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<resource/>";
            var anyContent = new object();

            _httpClientMock
                .Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>()))
                .Returns(new HttpResponseMessage());

            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicXmlContentParser(orderXml));

            var resource = new Restfulie(It.IsAny<Uri>(), _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscovererMock.Object)
                .Create(anyContent);

            Assert.IsInstanceOfType(
                resource.DynamicContentParser,
                typeof(DynamicXmlContentParser));
        }

        [TestMethod, Ignore]
        public void ShouldSetContentStringAndContentTypeToHttpRequest()
        {
            // carlos.mendonca: no idea how to test this.

            const string contentType = "application/xml";
            var content = new object();

            _httpClientMock
                .Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>()))
                .Returns(new HttpResponseMessage());

            var resource = new Restfulie(It.IsAny<Uri>(), _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscovererMock.Object)
                .Create(content);

            Assert.AreEqual(content.ToString(), resource.LatestHttpResponseMessage.Request.Content.ReadAsString());
            Assert.AreEqual(contentType, resource.LatestHttpResponseMessage.Request.Headers.ContentType);
        }
    }
}
