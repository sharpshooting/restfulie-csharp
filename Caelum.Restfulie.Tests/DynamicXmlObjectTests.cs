using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShooting.Tests;

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

        [TestMethod, Ignore]
        public void ShouldGetValueFromFirstLevelElementCalledValue()
        {
            // carlos.mendonca: Idea: introduce case-sensitivity/camelization.

            // carlos.mendonca: how to treat "reserved words"?
            const string xml = XmlHeader + "<a><value>value</value><VALUE>VALUE</VALUE></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("value", dynamicObject.value);
            Assert.AreEqual("VALUE", dynamicObject.VALUE);
        }

        [TestMethod]
        public void ShouldGetValuesFromMultipleFirstLevelElements()
        {
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><c>valueC</c></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueB1valueB2", dynamicObject.b.Value);
            Assert.AreEqual("valueB1", dynamicObject.b[0]);
            Assert.AreEqual("valueB2", dynamicObject.b[1]);
        }

        [TestMethod, Ignore]
        public void ShouldGetValueFromMultipleFirstLevelElementsByIndex()
        {
            // TODO: carlos.mendonca: fix this test and its implementation.
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><c>valueC</c><d></d><e/></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueB1",    dynamicObject[0]);
            Assert.AreEqual("valueB2",    dynamicObject[1]);
            Assert.AreEqual("valueC",     dynamicObject[2]);
            Assert.AreEqual(string.Empty, dynamicObject[3]);
            Assert.AreEqual(string.Empty, dynamicObject[4]);

            Assert.AreEqual("valueB1",    dynamicObject.b[0]);
            Assert.AreEqual("valueB2",    dynamicObject.b[1]);
            Assert.AreEqual("valueC",     dynamicObject.c[0]);
            Assert.AreEqual(string.Empty, dynamicObject.d[0]);
            Assert.AreEqual(string.Empty, dynamicObject.e[0]);

            Assert.AreEqual("valueB1",    dynamicObject.Value[0]);
            Assert.AreEqual("valueB2",    dynamicObject.Value[1]);
            Assert.AreEqual("valueC",     dynamicObject.Value[2]);
            Assert.AreEqual(string.Empty, dynamicObject.Value[3]);
            Assert.AreEqual(string.Empty, dynamicObject.Value[4]);
        }

        [TestMethod, Ignore]
        public void ShouldIterateTheValuesFromMultipleFirstLevelElements()
        {
            // carlos.mendonca: i'm not confortable with this implementation.
            const string xml = XmlHeader + "<a><b>value</b><b>value</b><c>value</c></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            int i = 0;

            foreach (dynamic _dynamicObject in dynamicObject)
            {
                Assert.AreEqual(_dynamicObject, "value");
                i++;
            }

            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void ShouldGetValueFromSecondLevelElement()
        {
            const string xml = XmlHeader + "<a><b><c>valueC1</c><c>valueC2</c><d>valueD</d><e></e><f/></b></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueC1valueC2", dynamicObject.b.c.Value);

            Assert.AreEqual("valueC1", dynamicObject.b.c[0]);
            Assert.AreEqual("valueC2", dynamicObject.b.c[1]);
            Assert.AreEqual("valueD", dynamicObject.b.d);
            Assert.AreEqual(string.Empty, dynamicObject.b.e);
            Assert.AreEqual(string.Empty, dynamicObject.b.f);
        }
    }
}
