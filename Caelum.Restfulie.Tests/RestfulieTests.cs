using System;
using System.Dynamic;
using System.Net;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpShooting.Http;
using SharpShooting.Tests;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class RestfulieTests
    {
        private Mock<IHttpClient> _httpClientMock;
        private Mock<IDynamicContentParserFactory> _dynamicContentParserFactoryMock;
        private Mock<IHttpMethodDiscovery> _httpMethodDiscoveryMock;
        private Restfulie _restfulie;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            _dynamicContentParserFactoryMock = new Mock<IDynamicContentParserFactory>();
            _httpMethodDiscoveryMock = new Mock<IHttpMethodDiscovery>();
            _restfulie = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object);
        }

        [TestMethod]
        public void ShouldDoAGetHttpRequestToUri()
        {
            var theUri = new Uri("http://localhost");
            const HttpMethod theGetHttpMethod = HttpMethod.GET;
            SetupHttpClientMock(theGetHttpMethod, theUri, new HttpResponseMessage());

            _restfulie.At(theUri);

            _httpClientMock.Verify(it => it.Send(theGetHttpMethod, theUri), Times.Once());
        }

        [TestMethod]
        public void ShouldSetLatestHttpResponseMessageUponEntryPoint()
        {
            var httpResponseMessage = new HttpResponseMessage();
            SetupHttpClientMock(httpResponseMessage);

            Assert.IsNull(_restfulie.LatestHttpResponseMessage);

            _restfulie.At(new Uri("http://localhost"));

            Assert.AreSame(httpResponseMessage, _restfulie.LatestHttpResponseMessage);
        }

        [TestMethod]
        public void ShouldDelegateToInternalDynamicObjectOnTryGetMemberReturningTrue()
        {
            SetupHttpClientMock(new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return true; } });

            dynamic resource = _restfulie.At(It.IsAny<Uri>());

            Assert.IsNull(resource.AnyMember);
        }

        [TestMethod, ExpectedException(typeof(RuntimeBinderException))]
        public void ShouldDelegateToInternalDynamicObjectOnTryGetMemberReturningFalse()
        {
            SetupHttpClientMock(new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return false; } });

            dynamic resource = _restfulie.At(It.IsAny<Uri>());

            TestHelpers.TryGetAndThrow(resource.AnyMember);
        }

        [TestMethod]
        public void ShouldReturnRestfulieWithDynamicXmlContentParserUponReceivingResponseWithXmlContentType()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order/>";

            SetupHttpClientMock(new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(new DynamicXmlContentParser(orderXml));

            dynamic order = _restfulie.At(It.IsAny<Uri>());

            Assert.IsInstanceOfType(
                order,
                typeof(Restfulie));

            Assert.IsInstanceOfType(
                (order as Restfulie).DynamicContentParser,
                typeof(DynamicXmlContentParser));
        }

        [TestMethod]
        public void ShouldProvideHttpStatusCodeUponRecievingResponse()
        {
            SetupHttpClientMock(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            SetupDynamicContentParserFactoryMock(new DynamicObjectStub());

            dynamic resource = _restfulie.At(It.IsAny<Uri>());

            Assert.AreEqual("200", resource.StatusCode);
        }

        [TestMethod]
        public void ShouldDoAGetHttpRequestToAtomLinkOnResourceAndShouldSetLatestHttpResponseMessage()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order><id>1</id><atom:link rel='refresh' href='http://localhost/orders/1' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            SetupHttpClientMock(new HttpResponseMessage());

            dynamic order = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
            {
                DynamicContentParser = new DynamicXmlContentParser(orderXml),
            };

            order.refresh();

            _httpClientMock.Verify(it => it.Send(HttpMethod.GET, new Uri("http://localhost/orders/1")), Times.Once());
        }

        [TestMethod]
        public void ShouldDoMakeHttpRequestWithHttpMethodDiscovery()
        {
            SetupHttpClientMock(new HttpResponseMessage());

            dynamic resource = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
            {
                DynamicContentParser = new DynamicObjectStub()
            };

            resource.AnyMethod();

            _httpMethodDiscoveryMock.Verify(it => it.MethodFor(It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldSetLatestHttpResponseMessageWhenFollowingAtomLink()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order><id>1</id><atom:link rel='refresh' href='http://localhost/orders/1' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            var firstHttpResponseMesage = new HttpResponseMessage();
            var secondHttpResponseMesage = new HttpResponseMessage();

            SetupHttpClientMock(secondHttpResponseMesage);

            dynamic order = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
            {
                DynamicContentParser = new DynamicXmlContentParser(orderXml),
                LatestHttpResponseMessage = firstHttpResponseMesage
            };

            dynamic orderAfterRefresh = order.refresh();

            Assert.AreSame((order as Restfulie).LatestHttpResponseMessage, firstHttpResponseMesage);
            Assert.AreSame((orderAfterRefresh as Restfulie).LatestHttpResponseMessage, secondHttpResponseMesage);
        }

        [TestMethod, Ignore]
        public void ShouldRejectUnsupportedContentTypes()
        {
        }

        private void SetupHttpClientMock(HttpResponseMessage httpResponseMessageToReturn)
        {
            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>())).Returns(httpResponseMessageToReturn);
        }

        private void SetupHttpClientMock(HttpMethod httpMethod, Uri uri, HttpResponseMessage httpResponseMessageToReturn)
        {
            _httpClientMock.Setup(it => it.Send(httpMethod, uri)).Returns(httpResponseMessageToReturn);
        }

        private void SetupDynamicContentParserFactoryMock(IDynamicContentParser dynamicContentParser)
        {
            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(dynamicContentParser);
        }

        private void SetupDynamicContentParserFactoryMock(HttpContent httpContent, IDynamicContentParser dynamicContentParser)
        {
            _dynamicContentParserFactoryMock.Setup(it => it.New(httpContent)).Returns(dynamicContentParser);
        }

        private class DynamicObjectStub : DynamicObject, IDynamicContentParser
        {
            public delegate TR Func<in T1, T2, out TR>(T1 t1, out T2 t2);

            public Func<GetMemberBinder, object, bool> TryGetMemberDelegate { get; set; }

            public Func<string, Uri> UriForDelegate { get; set; }

            public new bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (TryGetMemberDelegate != null)
                {
                    return TryGetMemberDelegate(binder, out result);
                }

                return base.TryGetMember(binder, out result);
            }

            public Uri UriFor(string stateTransition)
            {
                if (UriForDelegate != null)
                {
                    return UriForDelegate(stateTransition);
                }

                return new Uri("http://localhost");
            }
        }
    }
}
