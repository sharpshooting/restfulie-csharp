using System;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShooting.Tests;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class HttpMethodDiscoveryTest
    {
        private HttpMethodDiscovery _httpMethodDiscovery;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpMethodDiscovery = new HttpMethodDiscovery();
        }

        [TestMethod]
        public void ShouldReturnDeleteMethodForDeleteCancelOrDestroyVerbs()
        {
            _httpMethodDiscovery.MethodFor("delete").ShouldBeEqualTo(HttpMethod.DELETE);

            _httpMethodDiscovery.MethodFor("cancel").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscovery.MethodFor("destroy").ShouldBeEqualTo(HttpMethod.DELETE);
        }

        [TestMethod]
        public void ShouldBeCaseInsensitive()
        {
            _httpMethodDiscovery.MethodFor("cancel").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscovery.MethodFor("CANCEL").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscovery.MethodFor("cAnCeL").ShouldBeEqualTo(HttpMethod.DELETE);
        }

        [TestMethod]
        public void ShouldReturnPostForPostOrUpdateVerbs()
        {
            _httpMethodDiscovery.MethodFor("post").ShouldBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("update").ShouldBeEqualTo(HttpMethod.POST);
        }

        [TestMethod]
        public void ShouldReturnGetForEverythingElse()
        {
            _httpMethodDiscovery.MethodFor("delete").ShouldNotBeEqualTo(HttpMethod.GET);
            _httpMethodDiscovery.MethodFor("post").ShouldNotBeEqualTo(HttpMethod.GET);

            _httpMethodDiscovery.MethodFor("get").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscovery.MethodFor("ABC30C99-E79F-488C-8855-F9DB5D5E6C52").ShouldBeEqualTo(HttpMethod.GET);
        }

        [TestMethod]
        public void ShouldReturnGetForNullOrEmptyVerb()
        {
            _httpMethodDiscovery.MethodFor(null).ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscovery.MethodFor(String.Empty).ShouldBeEqualTo(HttpMethod.GET);
        }
    }
}
