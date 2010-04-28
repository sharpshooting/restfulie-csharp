using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShooting.Tests;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class RestfulieTests
    {
        private Restfulie _restfulie;

        [TestMethod]
        public void ShouldGetResourceFromXml()
        {
            var httpClientFake = new HttpClientFake(new HttpResponseMessage(), null);

            _restfulie = new Restfulie(httpClientFake);

            var actualHttpResponseMessage = _restfulie.At(new Uri("http://localhost:3000/orders/1"));

            Assert.AreEqual("GET", actualHttpResponseMessage.Method);
        }
    }

    public class Restfulie
    {
        private readonly HttpClient _httpClient;

        public Restfulie(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpResponseMessage At(Uri uri)
        {
            return _httpClient.Get(uri);
        }
    }
}
