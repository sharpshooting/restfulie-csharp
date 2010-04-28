using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CSharp.RuntimeBinder;
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

            Assert.AreEqual("value", dynamicObject.ToString());
        }

        [TestMethod]
        public void ShouldGetValueFromCurrentEmptyElement()
        {
            const string xml = XmlHeader + "<a></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual(string.Empty, dynamicObject.ToString());
        }

        [TestMethod]
        public void ShouldGetValueFromCurrentNullElement()
        {
            // carlos.mendonca: I was inclined to set that <a></a> == string.Empty, <a/> == null and inexistent
            //                  element throws RuntimeBinderException on TryGetMember method, but .NET's
            //                  System.Linq.Xml works differently.
            const string xml = XmlHeader + "<a/>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual(string.Empty, dynamicObject.ToString());
        }

        [TestMethod]
        public void ShouldGetValueFromCurrentElementWithDescendants()
        {
            // carlos.mendonca: This seens to be a .NET System.Linq.Xml convention: nested elements get
            //                  concatenated when you call the root element's value.
            const string xml = XmlHeader + "<p>This is a <strong>xhtml</strong> sample.</p>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("This is a xhtml sample.", dynamicObject.ToString());
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
        public void ShouldGetExceptionOnTryingToGetValueFromInexistentFirstLevelElement()
        {
            const string xml = XmlHeader + "<a><b>valueB</b></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root, DynamicXmlObject.TryGetMemberBehavior.Strict);

            TestHelpers.ExpectExceptionTypeOf<RuntimeBinderException>(() => TestHelpers.TryGetAndThrow(dynamicObject.c));
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

            Assert.AreEqual("valueB1valueB2", dynamicObject.b.ToString());
            Assert.AreEqual("valueB1", dynamicObject.b[0]);
            Assert.AreEqual("valueB2", dynamicObject.b[1]);
        }

        [TestMethod]
        public void ShouldGetValueFromMultipleFirstLevelElementsByIndex()
        {
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><c>valueC</c><d></d><e/></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueB1", dynamicObject[0]);
            Assert.AreEqual("valueB2", dynamicObject[1]);
            Assert.AreEqual("valueC", dynamicObject[2]);
            Assert.AreEqual(string.Empty, dynamicObject[3]);
            Assert.AreEqual(string.Empty, dynamicObject[4]);

            Assert.AreEqual("valueB1", dynamicObject.b[0]);
            Assert.AreEqual("valueB2", dynamicObject.b[1]);
            Assert.AreEqual("valueC", dynamicObject.c);
            Assert.AreEqual(string.Empty, dynamicObject.d);
            Assert.AreEqual(string.Empty, dynamicObject.e);
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

        [TestMethod, Ignore]
        public void ShouldQueryMultipleFirstLevelElements()
        {
            // TODO: carlos.mendonca: fix this implementation; maybe use http://jrwren.wrenfam.com/blog/2010/03/04/linq-abuse-with-the-c-4-dynamic-type/
            //const string xml =
            //    XmlHeader +
            //    "<order><book><title>Pragmatic Project Automation</title></book><book><title>Coders at Work</title></book></order>";

            //dynamic order = new DynamicXmlObject(XDocument.Parse(xml).Root);
            //var bookTitlesInOrder = from dynamic book in order.book
            //                        select book.title;
        }

        [TestMethod]
        public void ShouldGetValueFromSecondLevelElement()
        {
            const string xml = XmlHeader + "<a><b><c>valueC1</c><c>valueC2</c><d>valueD</d><e></e><f/></b></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueC1valueC2", dynamicObject.b.c.ToString());

            Assert.AreEqual("valueC1", dynamicObject.b.c[0]);
            Assert.AreEqual("valueC2", dynamicObject.b.c[1]);
            Assert.AreEqual("valueD", dynamicObject.b.d);
            Assert.AreEqual(string.Empty, dynamicObject.b.e);
            Assert.AreEqual(string.Empty, dynamicObject.b.f);
        }

        [TestMethod]
        public void ShouldGetValueFromFirstLevelElementWithNamespace()
        {
            const string xml =
                XmlHeader +
                "<a xmlns:atom='http://www.w3.org/2005/Atom'><atom:b>valueB1</atom:b><atom:b>valueB2</atom:b><atom:c>valueC</atom:c><atom:d></atom:d><atom:e/><f>valueF</f></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueB1valueB2valueCvalueF", dynamicObject.ToString());

            Assert.AreEqual("valueB1",    dynamicObject.Element("b", "http://www.w3.org/2005/Atom")[0]);
            Assert.AreEqual("valueB2",    dynamicObject.Element("b", "http://www.w3.org/2005/Atom")[1]);
            Assert.AreEqual("valueC",     dynamicObject.Element("c", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("d", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("e", "http://www.w3.org/2005/Atom"));
            
            Assert.AreEqual("valueF", dynamicObject.f);
        }

        [TestMethod]
        public void ShouldGetValueFromSecondLevelElementWithNamespace()
        {
            const string xml = XmlHeader + "<a xmlns:atom='http://www.w3.org/2005/Atom'><atom:b><atom:c>valueC1</atom:c><atom:c>valueC2</atom:c><atom:d>valueD</atom:d><atom:e></atom:e><atom:f/><g>valueG</g></atom:b></a>";

            dynamic dynamicObject = new DynamicXmlObject(XDocument.Parse(xml).Root);

            Assert.AreEqual("valueC1valueC2", dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("c", "http://www.w3.org/2005/Atom").ToString());

            Assert.AreEqual("valueC1",    dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("c", "http://www.w3.org/2005/Atom")[0]);
            Assert.AreEqual("valueC2",    dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("c", "http://www.w3.org/2005/Atom")[1]);
            Assert.AreEqual("valueD",     dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("d", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("e", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("f", "http://www.w3.org/2005/Atom"));
            
            Assert.AreEqual("valueG", dynamicObject.Element("b", "http://www.w3.org/2005/Atom").g);
        }
    }
}
