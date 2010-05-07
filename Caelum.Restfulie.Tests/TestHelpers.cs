using System;
using Microsoft.Http;
using Moq;
using SharpShooting.Http;

namespace Caelum.Restfulie.Tests
{
    public static class TestHelpers
    {
        public static void SetupHttpClientMock(this Mock<IHttpClient> httpClientMock, HttpResponseMessage httpResponseMessageToReturn)
        {
            httpClientMock.Setup(it => it.Send(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), null, null)).Returns(httpResponseMessageToReturn);
        }
    }
}