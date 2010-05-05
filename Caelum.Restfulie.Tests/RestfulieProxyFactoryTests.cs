using System;
using Microsoft.Http;
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
        private Mock<IHttpMethodDiscoverer> _httpMethodDiscoveryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            _dynamicContentParserFactoryMock = new Mock<IDynamicContentParserFactory>();
            _httpMethodDiscoveryMock = new Mock<IHttpMethodDiscoverer>();
        }

        [TestMethod]
        public void ShouldDoAGetHttpRequestToUri()
        {
            var theGetHttpMethod = HttpMethod.GET;
            var theUri = new Uri("http://localhost");

            SetupHttpClientMock(theGetHttpMethod, theUri, new HttpResponseMessage());

            new RestfulieProxyFactory(theUri, _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
                .Get();

            _httpClientMock.Verify(it => it.Send(theGetHttpMethod, theUri), Times.Once());
        }

        [TestMethod]
        public void ShouldSetLatestHttpResponseMessageUponEntryPoint()
        {
            var httpResponseMessage = new HttpResponseMessage();

            _httpClientMock.SetupHttpClientMock(httpResponseMessage);

            var resource = new RestfulieProxyFactory(It.IsAny<Uri>(), _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
                .Get();

            Assert.AreSame(httpResponseMessage, resource.LatestHttpResponseMessage);
        }

        [TestMethod]
        public void ShouldReturnRestfulieProxyWithDynamicXmlContentParserUponReceivingResponseWithXmlContentType()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<resource/>";

            _httpClientMock.SetupHttpClientMock(new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(new DynamicXmlContentParser(orderXml));

            var resource = new RestfulieProxyFactory(It.IsAny<Uri>(), _httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
                .Get();

            Assert.IsInstanceOfType(
                resource,
                typeof(RestfulieProxy));

            Assert.IsInstanceOfType(
                resource.DynamicContentParser,
                typeof(DynamicXmlContentParser));
        }
        
        private void SetupHttpClientMock(HttpMethod httpMethod, Uri uri, HttpResponseMessage httpResponseMessageToReturn)
        {
            _httpClientMock.Setup(it => it.Send(httpMethod, uri)).Returns(httpResponseMessageToReturn);
        }

        private void SetupDynamicContentParserFactoryMock(IDynamicContentParser dynamicContentParser)
        {
            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(dynamicContentParser);
        }
    }
}
