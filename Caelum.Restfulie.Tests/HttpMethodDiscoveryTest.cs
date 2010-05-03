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
        public void ShouldReturnDeleteMethodForCancelDeleteOrDestroyVerbs()
        {
            _httpMethodDiscovery.MethodFor("cancel").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscovery.MethodFor("delete").ShouldBeEqualTo(HttpMethod.DELETE);
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
        public void ShouldReturnPutForUpdateVerbs()
        {
            _httpMethodDiscovery.MethodFor("update").ShouldBeEqualTo(HttpMethod.PUT);
        }

        [TestMethod]
        public void ShouldReturnGetForLatestRefreshReloadOrShowVerbs()
        {
            _httpMethodDiscovery.MethodFor("latest").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscovery.MethodFor("refresh").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscovery.MethodFor("reload").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscovery.MethodFor("show").ShouldBeEqualTo(HttpMethod.GET);
        }

        [TestMethod]
        public void ShouldReturnPostForEverythingElse()
        {
            _httpMethodDiscovery.MethodFor("cancel").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("delete").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("destroy").ShouldNotBeEqualTo(HttpMethod.POST);

            _httpMethodDiscovery.MethodFor("update").ShouldNotBeEqualTo(HttpMethod.POST);

            _httpMethodDiscovery.MethodFor("latest").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("refresh").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("reload").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("show").ShouldNotBeEqualTo(HttpMethod.POST);

            _httpMethodDiscovery.MethodFor("get").ShouldBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor("ABC30C99-E79F-488C-8855-F9DB5D5E6C52").ShouldBeEqualTo(HttpMethod.POST);
        }

        [TestMethod]
        public void ShouldReturnPostForNullOrEmptyVerb()
        {
            _httpMethodDiscovery.MethodFor(null).ShouldBeEqualTo(HttpMethod.POST);
            _httpMethodDiscovery.MethodFor(String.Empty).ShouldBeEqualTo(HttpMethod.POST);
        }
    }
}
