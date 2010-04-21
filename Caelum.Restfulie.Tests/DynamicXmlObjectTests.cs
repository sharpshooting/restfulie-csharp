using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caelum.Restfulie.Tests
{
    [TestClass]
    public class DynamicXmlObjectTests
    {
        private const string XmlHeader = "<?xml version='1.0' encoding='UTF-8'?>\r\n";

        [TestMethod]
        public void ShouldGetValueFromCurrentElement()
        {
            const string xml = XmlHeader + "<a>value</a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("value", dynamicObject.Value);
        }

        [TestMethod]
        public void ShouldGetValueFromCurrentEmptyElement()
        {
            const string xml = XmlHeader + "<a></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual(string.Empty, dynamicObject.Value);
        }

        [TestMethod]
        public void ShouldGetValueFromCurrentNullElement()
        {
            // carlos.mendonca: I was inclined to set that <a></a> == string.Empty, <a/> == null and inexistent
            //                  element throws RuntimeBinderException on TryGetMember method, but .NET's
            //                  System.Linq.Xml works differently.
            const string xml = XmlHeader + "<a/>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual(string.Empty, dynamicObject.Value);
        }

        [TestMethod]
        public void ShouldGetValueFromCurrentElementWithDescendants()
        {
            // carlos.mendonca: This seens to be a .NET System.Linq.Xml convention: nested elements get
            //                  concatenated when you call the root element's value.
            const string xml = XmlHeader + "<a>valueA<a1>valueA1<a11>valueA11</a11></a1><a2>valueA2</a2></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);
            
            Assert.AreEqual("valueAvalueA1valueA11valueA2", dynamicObject.Value);
        }

        [TestMethod]
        public void ShouldGetValueFromFirstLevelElement()
        {
            const string xml = XmlHeader + "<a><b>valueB</b><c>valueC</c><d></d><e></e></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueB", dynamicObject.b);
            Assert.AreEqual("valueC", dynamicObject.c);
            Assert.AreEqual(string.Empty, dynamicObject.d);
            Assert.AreEqual(string.Empty, dynamicObject.e);
        }

        [TestMethod]
        public void ShouldGetNullValueFromFirstLevelElementThatDoesntExist()
        {
            // carlos.mendonca: Idea: consider introducing TryGetMemberBehavior.Strict as an enum in
            //                  DynamicXmlObject's constructor. When on, it should throw RuntimeBinderException
            //                  if element doens't exist.
            const string xml = XmlHeader + "<a><b>value</b></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.IsNull(dynamicObject.c);
        }

        [TestMethod]
        [Ignore]
        public void ShouldGetValueFromFirstLevelElementCalledValue()
        {
            
        }
    }
}
