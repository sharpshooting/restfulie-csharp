using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
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
        [TestMethod]
        public void ShouldGetResourceOnUriWithPredefinedRequestHeaders()
        {
            var anyUri = new Uri("http://localhost");

            var httpClientMock = new Mock<IHttpClient>();

            var requestHeaders = new RequestHeaders();

            var restfulie = new Restfulie(httpClientMock.Object, requestHeaders);
            restfulie.At(anyUri);

            httpClientMock.Verify(it => it.Send(HttpMethod.GET, anyUri, requestHeaders), Times.Once());
        }

        [TestMethod, Ignore]
        public void ShouldRejectUnsupportedContentTypes()
        {
        }

    }

    // carlos.mendonca: temporary piece of code for reference only.
    public class RestfuliePrototype : DynamicObject
    {
        private DynamicObject _dynamicContentParserObject;
        private IHttpClient _httpClient;

        public dynamic At(Uri uri)
        {
            var httpMessageResponse = _httpClient.Send(HttpMethod.GET, uri);

            // if (httpMessageResponse.Content.ContentType == "text/xml")
            //     return DynamicXmlObject(XDocument.Parse(httpMessageResponse.ToString());

            return null;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return _dynamicContentParserObject.TryInvokeMember(binder, args, out result);
        }
    }
}
