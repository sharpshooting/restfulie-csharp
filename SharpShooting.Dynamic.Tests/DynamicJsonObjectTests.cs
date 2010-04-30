using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace SharpShooting.Dynamic.Tests
{
    [TestClass]
    public class DynamicJsonObjectTests
    {
        [TestMethod, Ignore]
        public void ShouldGetValueFromCurrentElement()
        {
            // carlos.mendonca: Json.Net doesn't seen to be compatible with '<node>.Value' notation; maybe I
            //                  should change DynamicXmlObject to always demmand that the root element be
            //                  specified in order to access the hierarchy (e.g.: <root>.Order.Book instead of
            //                  <root>.Book).
            const string json = "{ 'a': 'value' }";
            
            dynamic dynamicObject = new DynamicJsonObject(JObject.Parse(json).Root);

            Assert.AreEqual("value", dynamicObject.ToString());
        }
    }
}
