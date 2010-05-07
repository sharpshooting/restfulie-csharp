using System;
using System.Dynamic;
using System.Net;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class RestfulieProxyTests
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
        public void ShouldDelegateToInternalDynamicObjectOnTryGetMemberReturningTrue()
        {
            var dynamicContentParserStub = new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return true; } };

            dynamic resource = new RestfulieProxy(It.IsAny<IHttpClient>(), It.IsAny<IDynamicContentParserFactory>(), It.IsAny<IHttpMethodDiscoverer>())
            {
                DynamicContentParser = dynamicContentParserStub
            };

            Assert.IsNull(resource.AnyMember);
        }

        [TestMethod, ExpectedException(typeof(RuntimeBinderException))]
        public void ShouldDelegateToInternalDynamicObjectOnTryGetMemberReturningFalse()
        {
            var dynamicContentParserStub = new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return false; } };

            dynamic resource = new RestfulieProxy(It.IsAny<IHttpClient>(), It.IsAny<IDynamicContentParserFactory>(), It.IsAny<IHttpMethodDiscoverer>())
            {
                DynamicContentParser = dynamicContentParserStub
            };

            SharpShooting.Tests.TestHelpers.TryGetAndThrow(resource.AnyMember);
        }

        [TestMethod]
        public void ShouldProvideHttpStatusCodeUponRecievingResponse()
        {
            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            dynamic resource = new RestfulieProxy(It.IsAny<IHttpClient>(), It.IsAny<IDynamicContentParserFactory>(), It.IsAny<IHttpMethodDiscoverer>())
            {
                LatestHttpResponseMessage = httpResponseMessage
            };

            Assert.AreEqual("200", resource.StatusCode);
        }

        [TestMethod]
        public void ShouldDoAGetHttpRequestToAtomLinkOnResourceAndShouldSetLatestHttpResponseMessage()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order><id>1</id><atom:link rel='refresh' href='http://localhost/orders/1' xmlns:atom='http://www.w3.org/2005/Atom'/></order>";

            _httpClientMock.SetupHttpClientMock(new HttpResponseMessage());

            _httpMethodDiscoveryMock.Setup(it => it.MethodFor(It.IsAny<string>())).Returns(HttpMethod.GET);

            dynamic order = new RestfulieProxy(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
            {
                DynamicContentParser = new DynamicXmlContentParser(orderXml),
            };

            order.refresh();

            _httpClientMock.Verify(it => it.Send(HttpMethod.GET, new Uri("http://localhost/orders/1"), It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>()), Times.Once());
        }

        [TestMethod]
        public void ShouldDoMakeHttpRequestWithHttpMethodDiscovery()
        {
            SetupHttpClientMock(new HttpResponseMessage());

            dynamic resource = new RestfulieProxy(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
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

            dynamic order = new RestfulieProxy(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object, _httpMethodDiscoveryMock.Object)
            {
                DynamicContentParser = new DynamicXmlContentParser(orderXml),
                LatestHttpResponseMessage = firstHttpResponseMesage
            };

            dynamic orderAfterRefresh = order.refresh();

            Assert.AreSame((order as RestfulieProxy).LatestHttpResponseMessage, firstHttpResponseMesage);
            Assert.AreSame((orderAfterRefresh as RestfulieProxy).LatestHttpResponseMessage, secondHttpResponseMesage);
        }

        [TestMethod, Ignore]
        public void ShouldRejectUnsupportedContentTypes()
        {
        }

        private void SetupHttpClientMock(HttpResponseMessage httpResponseMessageToReturn)
        {
            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<RequestHeaders>(), It.IsAny<HttpContent>())).Returns(httpResponseMessageToReturn);
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
