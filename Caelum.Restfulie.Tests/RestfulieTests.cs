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
        private Restfulie _restfulie;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            _dynamicContentParserFactoryMock = new Mock<IDynamicContentParserFactory>();
            _restfulie = new Restfulie(_httpClientMock.Object, _dynamicContentParserFactoryMock.Object);
        }

        [TestMethod]
        public void ShouldDoGetHttpRequestToUri()
        {
            var theUri = new Uri("http://localhost");

            const HttpMethod theGetHttpMethod = HttpMethod.GET;

            SetupHttpClientMock(httpMethod: theGetHttpMethod,
                uri: theUri, httpResponseMessageToReturn: new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(dynamicObjectToReturn: new DynamicObjectStub());

            _restfulie.At(theUri);

            _httpClientMock.Verify(it => it.Send(theGetHttpMethod, theUri), Times.Once());
        }

        [TestMethod]
        public void ShouldReturnSameResponseAsInternalDynamicObjectOnTryGetMemberReturningTrue()
        {
            SetupHttpClientMock(httpResponseMessageToReturn: new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(dynamicObjectToReturn: new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return true; } });

            dynamic dynamicObject = _restfulie.At(It.IsAny<Uri>());

            Assert.IsNull(dynamicObject.AnyMember);
        }

        [TestMethod, ExpectedException(typeof(RuntimeBinderException))]
        public void ShouldReturnSameResponseAsInternalDynamicObjectOnTryGetMemberReturningFalse()
        {
            SetupHttpClientMock(httpResponseMessageToReturn: new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(dynamicObjectToReturn: new DynamicObjectStub { TryGetMemberDelegate = (GetMemberBinder getMemberBinder, out object result) => { result = null; return false; } });

            dynamic dynamicObject = _restfulie.At(It.IsAny<Uri>());

            TestHelpers.TryGetAndThrow(dynamicObject.AnyMember);
        }

        [TestMethod]
        public void ShouldReturnRestfulieWithDynamicXmlObjectAsParserUponReceivingResponseWithXmlContentType()
        {
            const string orderXml = "<?xml version='1.0' encoding='UTF-8'?>\r\n<order><id>1</id></order>";

            SetupHttpClientMock(httpResponseMessageToReturn: new HttpResponseMessage());

            SetupDynamicContentParserFactoryMock(dynamicObjectToReturn: new DynamicXmlObject(orderXml));

            var order = _restfulie.At(It.IsAny<Uri>());

            Assert.IsInstanceOfType(order, typeof(Restfulie));
            Assert.AreEqual("1", order.id);
        }

        [TestMethod]
        public void ShouldProvideHttpStatusCodeUponRecievingResponse()
        {
            SetupHttpClientMock(httpResponseMessageToReturn: new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            SetupDynamicContentParserFactoryMock(dynamicObjectToReturn: new DynamicObjectStub());

            var restfulie = _restfulie;
            var resource = restfulie.At(It.IsAny<Uri>());

            Assert.AreEqual("200", resource.StatusCode);
        }

        [TestMethod, Ignore]
        public void ShouldRejectUnsupportedContentTypes()
        {
        }

        private void SetupHttpClientMock(HttpResponseMessage httpResponseMessageToReturn)
        {
            SetupHttpClientMock(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), httpResponseMessageToReturn);
        }

        private void SetupHttpClientMock(HttpMethod httpMethod, Uri uri, HttpResponseMessage httpResponseMessageToReturn)
        {
            _httpClientMock.Setup(it => it.Send(httpMethod, uri)).Returns(httpResponseMessageToReturn);
        }

        private void SetupDynamicContentParserFactoryMock(DynamicObject dynamicObjectToReturn)
        {
            SetupDynamicContentParserFactoryMock(It.IsAny<HttpContent>(), dynamicObjectToReturn);
        }

        private void SetupDynamicContentParserFactoryMock(HttpContent httpContent, DynamicObject dynamicObjectToReturn)
        {
            _dynamicContentParserFactoryMock.Setup(it => it.New(httpContent)).Returns(dynamicObjectToReturn);
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
    }
}
