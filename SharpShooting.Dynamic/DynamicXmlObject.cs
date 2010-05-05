using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace SharpShooting.Dynamic
{
    public class DynamicXmlObject : DynamicObject, IEnumerable
    {
        private readonly bool _shouldBypassRootElement;
        private readonly bool _shouldThrowOnInexistentElement;

        private readonly IEnumerable<XElement> _xElements;
        
        public DynamicXmlObject(string xml, bool shouldBypassRootElement = true, bool shouldThrowOnInexistentElement = false)
        {
            _xElements = new[] { XDocument.Parse(xml).Root };
            _shouldBypassRootElement = shouldBypassRootElement;
            _shouldThrowOnInexistentElement = shouldThrowOnInexistentElement;
        }

        private DynamicXmlObject(XElement xElement, bool shouldBypassRootElement, bool shouldThrowOnInexistentElement)
            : this(new[] { xElement }, shouldBypassRootElement, shouldThrowOnInexistentElement)
        {
        }

        private DynamicXmlObject(IEnumerable<XElement> xElements, bool shouldBypassRootElement, bool shouldThrowOnInexistentElement)
        {
            _xElements = xElements;
            _shouldBypassRootElement = shouldBypassRootElement;
            _shouldThrowOnInexistentElement = shouldThrowOnInexistentElement;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = ResolveElement(binder.Name);

            return ResolveResult(result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = ResolveElementAtIndex((int)indexes[0]);

            return ResolveResult(result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            if (binder.Name.Equals("element", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 2 && !(args[0] is string) && !(args[1] is string))
                    throw new ArgumentException("Method Element takes two string parameters.");

                result = ResolveElement((string)args[0], (string)args[1]);

                return ResolveResult(result);
            }

            if (binder.Name.Equals("attribute", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 1 && !(args[0] is string))
                    throw new ArgumentException("Method Attribute takes one string parameter.");

                if (_xElements.Count() == 1)
                {
                    var xAttribute = _xElements.Single().Attribute((string)args[0]);

                    if (xAttribute != null)
                        result = xAttribute.Value;
                }

                return ResolveResult(result);
            }

            return false;
        }

        private bool ResolveResult(object result)
        {
            return !_shouldThrowOnInexistentElement || result != null;
        }

        private object ResolveElement(string localName, string namespaceName = "")
        {
            var xName = XName.Get(localName, namespaceName);

            var xElementsToResolve = _shouldBypassRootElement ? _xElements.Elements(xName) : _xElements.Where(it => it.Name == xName);

            if (xElementsToResolve.Count() == 1)
            {
                var xElement = xElementsToResolve.Single();

                if (xElement.HasAttributes || xElement.HasElements)
                    return new DynamicXmlObject(xElement, true, _shouldThrowOnInexistentElement);

                return xElement.Value;
            }

            if (xElementsToResolve.Count() > 1)
                return new DynamicXmlObject(xElementsToResolve, false, _shouldThrowOnInexistentElement);

            return null;
        }

        private object ResolveElementAtIndex(int index)
        {
            var xElementsToResolve = _shouldBypassRootElement ? _xElements.Elements() : _xElements;

            if (xElementsToResolve.Count() > index)
            {
                var xElement = xElementsToResolve.ElementAt(index);

                if (xElement.HasAttributes || xElement.HasElements)
                    return new DynamicXmlObject(xElement, true, _shouldThrowOnInexistentElement);

                return xElement.Value;
            }

            return null;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotImplementedException();

            //var xElements = XElement.Elements(binder.Name);

            //if (xElements.Count() == 1)
            //{
            //    var xElement = xElements.Single();

            //    if (xElement.HasElements || xElement.HasAttributes)
            //        throw new NotImplementedException();
            //    else
            //        xElement.Value = (string)value;
            //}
            //else
            //    throw new NotImplementedException();

            //return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            //var index = (int)indexes[0];

            //if (XElement.Elements().Count() > index)
            //    XElement.Elements().ElementAt(index).Value = (string)value;

            //return true;

            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            // carlos.mendonca: believe it or not, this is equivalent to a yield return.
            //return XElements.Elements().Select(xElement => xElement.Value).GetEnumerator();

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _xElements.Single().Value;
        }
    }
}