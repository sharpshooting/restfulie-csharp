using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShooting.Dynamic;
using SharpShooting.Tests;

namespace SharpShooting.Dynamic.Tests
{
    [TestClass]
    public class DynamicXmlObjectTests
    {
        private const string XmlHeader = "<?xml version='1.0' encoding='UTF-8'?>\r\n";

        [TestMethod]
        public void ShouldGetValueFromRootElementNotBypassingIt()
        {
            const string xml = XmlHeader + "<a>value</a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, false);

            Assert.AreEqual("value", dynamicObject.a);
        }

        [TestMethod]
        public void ShouldGetValueFromRootEmptyElementNotBypassingIt()
        {
            const string xml = XmlHeader + "<a></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, false);

            Assert.AreEqual(string.Empty, dynamicObject.a);
        }

        [TestMethod]
        public void ShouldGetValueFromRootNullElementNotBypassingIt()
        {
            // carlos.mendonca: I was inclined to set that <a></a> == string.Empty, <a/> == null and inexistent
            //                  element throws RuntimeBinderException on TryGetMember method, but .NET's
            //                  System.Linq.Xml works differently.
            const string xml = XmlHeader + "<a/>";

            dynamic dynamicObject = new DynamicXmlObject(xml, false);

            Assert.AreEqual(string.Empty, dynamicObject.a);
        }

        [TestMethod]
        public void ShouldGetValueFromRootElement()
        {
            const string xml = XmlHeader + "<a>value</a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("value", dynamicObject.ToString());
        }

        [TestMethod, Ignore]
        public void ShouldGetValueFromRootElementWithDescendants()
        {
            // carlos.mendonca: This seens to be a .NET System.Linq.Xml convention: nested elements get
            //                  concatenated when you call the root element's value.
            const string xml = XmlHeader + "<p>This is a <strong>xhtml</strong> sample.</p>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("This is a xhtml sample.", dynamicObject.p.ToString());
        }

        [TestMethod]
        public void ShouldGetValueFromFirstLevelElementNotBypassingRootElement()
        {
            const string xml = XmlHeader + "<a><b>valueB</b></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, false);

            Assert.AreEqual("valueB", dynamicObject.a.b);
        }

        [TestMethod]
        public void ShouldGetValueFromFirstLevelElementBypassingRootElement()
        {
            const string xml = XmlHeader + "<a><b>valueB</b></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueB", dynamicObject.b);
        }

        [TestMethod]
        public void ShouldGetValueFromFirstLevelElement()
        {
            const string xml = XmlHeader + "<a><b>valueB</b><c></c><d/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueB", dynamicObject.b);
            Assert.AreEqual(string.Empty, dynamicObject.c);
            Assert.AreEqual(string.Empty, dynamicObject.d);
        }

        [TestMethod]
        public void ShouldThrowRuntimeBinderExceptionIfElementDoesNotExistOnStrictBinding()
        {
            const string xml = XmlHeader + "<a><b>valueB</b></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, shouldThrowOnInexistentElement: true);

            TestHelpers.ExpectExceptionTypeOf<RuntimeBinderException>(() => TestHelpers.TryGetAndThrow(dynamicObject.c));
        }

        [TestMethod]
        public void ShouldGetNullValueFromFirstLevelElementThatDoesntExist()
        {
            const string xml = XmlHeader + "<a><b>value</b></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.IsNull(dynamicObject.c);
        }

        [TestMethod]
        public void ShouldGetValuesFromMultipleFirstLevelElements()
        {
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><b></b><b/><c>valueC</c><d></d><e/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueB1", dynamicObject.b[0]);
            Assert.AreEqual("valueB2", dynamicObject.b[1]);
            Assert.AreEqual(String.Empty, dynamicObject.b[2]);
            Assert.AreEqual(String.Empty, dynamicObject.b[3]);

            Assert.IsNull(dynamicObject.b[4]);
        }

        [TestMethod]
        public void ShouldThrowRuntimeBinderExpecetionIfIndexOfMultipleFirstLevelElementsIsOutOfRangeOnStrictBinding()
        {
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><b></b><b/><c>valueC</c><d></d><e/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, shouldThrowOnInexistentElement: true);

            TestHelpers.ExpectExceptionTypeOf<RuntimeBinderException>(() => TestHelpers.TryGetAndThrow(dynamicObject.b[4]));
        }

        [TestMethod]
        public void ShouldGetValueFromSingleSecondLevelElementsChildrenToMultipleFirstLevelElements()
        {
            const string xml = XmlHeader + "<a><b><c>valueC1</c></b><b><c>valueC2</c></b><b><c></c></b><b><c/></b></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueC1", dynamicObject.b[0].c);
            Assert.AreEqual("valueC2", dynamicObject.b[1].c);
            Assert.AreEqual(String.Empty, dynamicObject.b[2].c);
            Assert.AreEqual(String.Empty, dynamicObject.b[3].c);
        }

        [TestMethod]
        public void ShouldGetValueFromMultipleFirstLevelElementsByIndex()
        {
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><c>valueC</c><d></d><e/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueB1", dynamicObject[0]);
            Assert.AreEqual("valueB2", dynamicObject[1]);
            Assert.AreEqual("valueC", dynamicObject[2]);
            Assert.AreEqual(String.Empty, dynamicObject[3]);
            Assert.AreEqual(String.Empty, dynamicObject[4]);
        }

        [TestMethod]
        public void ShouldGetValueFromMultipleFirstLevelElementsByIndexNotBypassingRootElement()
        {
            const string xml = XmlHeader + "<a><b>valueB1</b><b>valueB2</b><c>valueC</c><d></d><e/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, false);

            Assert.AreEqual("valueB1", dynamicObject.a[0]);
            Assert.AreEqual("valueB2", dynamicObject.a[1]);
            Assert.AreEqual("valueC", dynamicObject.a[2]);
            Assert.AreEqual(String.Empty, dynamicObject.a[3]);
            Assert.AreEqual(String.Empty, dynamicObject.a[4]);

            Assert.AreEqual("valueB1", dynamicObject[0][0]);
            Assert.AreEqual("valueB2", dynamicObject[0][1]);
            Assert.AreEqual("valueC", dynamicObject[0][2]);
            Assert.AreEqual(String.Empty, dynamicObject[0][3]);
            Assert.AreEqual(String.Empty, dynamicObject[0][4]);
        }

        [TestMethod, Ignore]
        public void ShouldIterateTheValuesFromMultipleFirstLevelElements()
        {
            // TODO: carlos.mendonca: I'm not confortable with the assertions in this test.
            const string xml = XmlHeader + "<a><b>value</b><b>value</b><c>value</c></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

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

            dynamic dynamicObject = new DynamicXmlObject(xml);

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

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueB1", dynamicObject.Element("b", "http://www.w3.org/2005/Atom")[0]);
            Assert.AreEqual("valueB2", dynamicObject.Element("b", "http://www.w3.org/2005/Atom")[1]);
            Assert.AreEqual("valueC", dynamicObject.Element("c", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("d", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("e", "http://www.w3.org/2005/Atom"));

            Assert.AreEqual("valueF", dynamicObject.f);
        }

        [TestMethod]
        public void ShouldThrowRuntimeBinderExceptionIfElementWithNamespaceDoesNotExist()
        {
            const string xml =
                XmlHeader +
                "<a xmlns:atom='http://www.w3.org/2005/Atom'></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, shouldThrowOnInexistentElement: true);

            TestHelpers.ExpectExceptionTypeOf<RuntimeBinderException>(() => dynamicObject.Element("b", "http://www.w3.org/2005/Atom"));
        }

        [TestMethod]
        public void ShouldGetValueFromSecondLevelElementWithNamespace()
        {
            const string xml = XmlHeader + "<a xmlns:atom='http://www.w3.org/2005/Atom'><atom:b><atom:c>valueC1</atom:c><atom:c>valueC2</atom:c><atom:d>valueD</atom:d><atom:e></atom:e><atom:f/><g>valueG</g></atom:b></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("valueC1", dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("c", "http://www.w3.org/2005/Atom")[0]);
            Assert.AreEqual("valueC2", dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("c", "http://www.w3.org/2005/Atom")[1]);
            Assert.AreEqual("valueD", dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("d", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("e", "http://www.w3.org/2005/Atom"));
            Assert.AreEqual(string.Empty, dynamicObject.Element("b", "http://www.w3.org/2005/Atom").Element("f", "http://www.w3.org/2005/Atom"));

            Assert.AreEqual("valueG", dynamicObject.Element("b", "http://www.w3.org/2005/Atom").g);
        }

        [TestMethod]
        public void ShouldGetValueFromAttributeIfItExists()
        {
            // carlos.mendonca: this test introduces a new rule: if the element has an attribute, even if it has
            //                  no children elements, it has to return DynamicXmlObject so that we can access
            //                  the attributes. The element's value can only be accessed with the ToString
            //                  method.

            // carlos.mendonca: System.Xml.Linq.XElement does not support an element with two duplicate
            //                  attributes.
            const string xml = XmlHeader + "<a><b attribute1='attributeValueB1' attribute2=''/><c attribute='attributeValueC'><cc/></c><d attribute='attributeValueD'><dd/><dd/></d><e attribute='attributeValueE1'/><e attribute='attributeValueE2'/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml);

            Assert.AreEqual("attributeValueB1", dynamicObject.b.Attribute("attribute1"));
            Assert.AreEqual(string.Empty, dynamicObject.b.Attribute("attribute2"));

            Assert.IsNull(dynamicObject.b.Attribute("attribute3"));

            Assert.AreEqual("attributeValueC", dynamicObject.c.Attribute("attribute"));
            Assert.AreEqual("attributeValueD", dynamicObject.d.Attribute("attribute"));

            Assert.AreEqual("attributeValueE1", dynamicObject.e[0].Attribute("attribute"));
            Assert.AreEqual("attributeValueE2", dynamicObject.e[1].Attribute("attribute"));
        }

        [TestMethod]
        public void ShouldThrowRuntimeBinderExceptionIfAttributeDoesNotExistOnStrictBinding()
        {
            const string xml = XmlHeader + "<a><b attribute1='attributeValueB1' attribute2=''/></a>";

            dynamic dynamicObject = new DynamicXmlObject(xml, shouldThrowOnInexistentElement: true);

            Assert.AreEqual(string.Empty, dynamicObject.b.ToString());

            TestHelpers.ExpectExceptionTypeOf<RuntimeBinderException>(() => dynamicObject.b.Attribute("attribute3"));
        }

        [TestMethod, Ignore]
        public void ShouldThrowArgumentExceptionIfElementMethodIsCalledWithWrongSignature()
        {
            // TODO: carlos.mendonca: write this test!
        }

        [TestMethod, Ignore]
        public void ShouldThrowArgumentExceptionIfAttributeMethodIsCalledWithWrongSignature()
        {
            // TODO: carlos.mendonca: write this test!
        }

        //[TestMethod, Ignore]
        //public void ShouldSetValueToRootElement()
        //{
        //    // carlos.mendonca: not sure if I should introduce the reserved member "Root" or not.
        //    const string xml = XmlHeader + "<a>before</a>";

        //    dynamic dynamicObject = new DynamicXmlObject(xml);
        //    dynamicObject.Root = "after";

        //    Assert.AreEqual("after", dynamicObject.Root);
        //}

        //[TestMethod]
        //public void ShouldSetValueToFirstLevelUniqueElement()
        //{
        //    const string xml = XmlHeader + "<a><b>beforeB1</b><b>beforeB2</b><c>beforeC</c><d></d><e/></a>";

        //    dynamic dynamicObject = new DynamicXmlObject(xml);

        //    dynamicObject.c = "afterC";

        //    dynamicObject.d = "afterD";

        //    dynamicObject.e = "afterE";

        //    Assert.AreEqual("afterC", dynamicObject.c);
        //    Assert.AreEqual("afterD", dynamicObject.d);
        //    Assert.AreEqual("afterE", dynamicObject.e);
        //}

        //[TestMethod, Ignore]
        //public void ShouldSetValueToFirstLevelDuplicatedElement()
        //{
        //    const string xml = XmlHeader + "<a><b>beforeB1</b><b>beforeB2</b><c>beforeC</c><d></d><e/></a>";

        //    dynamic dynamicObject = new DynamicXmlObject(xml);

        //    dynamicObject.b[0] = "afterB1";
        //    dynamicObject.b[1] = "afterB2";

        //    Assert.AreEqual("afterB1", dynamicObject.b[0]);
        //    Assert.AreEqual("afterB2", dynamicObject.b[1]);
        //}
    }
}