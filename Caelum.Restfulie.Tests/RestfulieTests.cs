using System;
using System.Dynamic;
using System.Net;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpShooting.Dynamic.Xml;
using SharpShooting.Http;
using SharpShooting.Tests;

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

        private class DynamicObjectStub : DynamicObject
        {
            public delegate TR Func<in T1, T2, out TR>(T1 t1, out T2 t2);

            public Func<GetMemberBinder, object, bool> TryGetMemberDelegate { get; set; }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (TryGetMemberDelegate != null)
                {
                    return TryGetMemberDelegate(binder, out result);
                }

                return base.TryGetMember(binder, out result);
            }
        }

        [TestMethod]
        public void ShouldGETResourceOnUriWithRequestHeaders()
        {
            var theUri = new Uri("http://localhost");

            const HttpMethod getHttpMethod = HttpMethod.GET;

            _httpClientMock.Setup(it => it.Send(getHttpMethod, theUri)).Returns(new HttpResponseMessage());

            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicObjectStub());

            new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object).At(theUri);

            _httpClientMock.Verify(it => it.Send(getHttpMethod, theUri), Times.Once());
        }

        [TestMethod]
        public void ShouldReturnSameResponseAsInternalDynamicObjectOnTryGetMemberReturningTrue()
        {
            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>())).Returns(new HttpResponseMessage());

            _dynamicContentParserFactoryMock
                .Setup(it => it.New(It.IsAny<HttpContent>()))
                .Returns(new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return true; } });

            dynamic dynamicObject = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object).At(It.IsAny<Uri>());

            Assert.IsNull(dynamicObject.AnyMember);
        }

        [TestMethod, ExpectedException(typeof(RuntimeBinderException))]
        public void ShouldReturnSameResponseAsInternalDynamicObjectOnTryGetMemberReturningFalse()
        {
            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>())).Returns(new HttpResponseMessage());

            _dynamicContentParserFactoryMock
                .Setup(it => it.New(It.IsAny<HttpContent>()))
                .Returns(new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return false; } });

            dynamic dynamicObject = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object).At(It.IsAny<Uri>());

            TestHelpers.TryGetAndThrow(dynamicObject.AnyMember);
        }

        [TestMethod]
        public void ShouldReturnRestfulieWithDynamicXmlObjectAsParserUponReceivingResponseWithXmlContentType()
        {
            const string applicationXmlContentType = "application/xml";
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order><id>1</id></order>";

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = HttpContent.Create(orderXml, Encoding.UTF8, applicationXmlContentType)
            };

            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>())).Returns(httpResponseMessage);

            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicXmlObject(orderXml));

            var restfulie = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object);
            var order = restfulie.At(It.IsAny<Uri>());

            Assert.IsInstanceOfType(order, typeof(Restfulie));
            Assert.AreEqual("1", order.id);
        }

        [TestMethod]
        public void ShouldProvideHttpStatusCodeUponRecievingResponse()
        {
            _httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>())).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
            _dynamicContentParserFactoryMock.Setup(it => it.New(It.IsAny<HttpContent>())).Returns(new DynamicObjectStub());

            var restfulie = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object);
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
