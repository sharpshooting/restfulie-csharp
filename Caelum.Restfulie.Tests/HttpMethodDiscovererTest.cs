using System;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShooting.Tests;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class HttpMethodDiscovererTest
    {
        private HttpMethodDiscoverer _httpMethodDiscoverer;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpMethodDiscoverer = new HttpMethodDiscoverer();
        }

        [TestMethod]
        public void ShouldReturnDeleteMethodForCancelDeleteOrDestroyVerbs()
        {
            _httpMethodDiscoverer.MethodFor("cancel").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscoverer.MethodFor("delete").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscoverer.MethodFor("destroy").ShouldBeEqualTo(HttpMethod.DELETE);
        }

        [TestMethod]
        public void ShouldBeCaseInsensitive()
        {
            _httpMethodDiscoverer.MethodFor("cancel").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscoverer.MethodFor("CANCEL").ShouldBeEqualTo(HttpMethod.DELETE);
            _httpMethodDiscoverer.MethodFor("cAnCeL").ShouldBeEqualTo(HttpMethod.DELETE);
        }

        [TestMethod]
        public void ShouldReturnPutForUpdateVerbs()
        {
            _httpMethodDiscoverer.MethodFor("update").ShouldBeEqualTo(HttpMethod.PUT);
        }

        [TestMethod]
        public void ShouldReturnGetForLatestRefreshReloadOrShowVerbs()
        {
            _httpMethodDiscoverer.MethodFor("latest").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscoverer.MethodFor("refresh").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscoverer.MethodFor("reload").ShouldBeEqualTo(HttpMethod.GET);
            _httpMethodDiscoverer.MethodFor("show").ShouldBeEqualTo(HttpMethod.GET);
        }

        [TestMethod]
        public void ShouldReturnPostForEverythingElse()
        {
            _httpMethodDiscoverer.MethodFor("cancel").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor("delete").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor("destroy").ShouldNotBeEqualTo(HttpMethod.POST);

            _httpMethodDiscoverer.MethodFor("update").ShouldNotBeEqualTo(HttpMethod.POST);

            _httpMethodDiscoverer.MethodFor("latest").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor("refresh").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor("reload").ShouldNotBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor("show").ShouldNotBeEqualTo(HttpMethod.POST);

            _httpMethodDiscoverer.MethodFor("get").ShouldBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor("ABC30C99-E79F-488C-8855-F9DB5D5E6C52").ShouldBeEqualTo(HttpMethod.POST);
        }

        [TestMethod]
        public void ShouldReturnPostForNullOrEmptyVerb()
        {
            _httpMethodDiscoverer.MethodFor(null).ShouldBeEqualTo(HttpMethod.POST);
            _httpMethodDiscoverer.MethodFor(String.Empty).ShouldBeEqualTo(HttpMethod.POST);
        }
    }
}
