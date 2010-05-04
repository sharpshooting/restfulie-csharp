using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace SharpShooting.Dynamic
{
    public class DynamicXmlObject : DynamicObject, IEnumerable
    {
        public enum TryGetMemberBehavior { Loose, Strict }

        private readonly XElement _xElement;
        protected XElement XElement { get { return _xElement; } }

        private readonly TryGetMemberBehavior _tryGetMemberBehavior;

        public DynamicXmlObject(string xml, TryGetMemberBehavior tryGetMemberBehavior = TryGetMemberBehavior.Loose)
        {
            _xElement = XDocument.Parse(xml).Root;
            _tryGetMemberBehavior = tryGetMemberBehavior;
        }

        public DynamicXmlObject(XElement xElement, TryGetMemberBehavior tryGetMemberBehavior = TryGetMemberBehavior.Loose)
        {
            _xElement = xElement;
            _tryGetMemberBehavior = tryGetMemberBehavior;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var memberName = binder.Name;

            result = ResolveElement(memberName);

            return _tryGetMemberBehavior != TryGetMemberBehavior.Strict || result != null;
        }

        private object ResolveElement(string localName, string namespaceName = "")
        {
            object result;

            var xElements = XElement.Elements(XName.Get(localName, namespaceName));

            if (xElements.Count() == 1)
            {
                var xElement = xElements.Single();

                if (xElement.HasElements || xElement.HasAttributes)
                    result = new DynamicXmlObject(xElement);
                else
                    result = xElement.Value;
            }
            else if (xElements.Count() > 1)
                result = new DynamicXmlObject(new XElement(XElement.Name, xElements));
            else
                result = null;

            return result;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var index = (int)indexes[0];

            if (XElement.Elements().Count() > index)
                result = XElement.Elements().ElementAt(index).Value;
            else
                // TODO: carlos.mendonca: I forgot to consider Loose/Strict behavior.
                result = null;

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            if (binder.Name.Equals("element", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 2 && !(args[0] is string) && !(args[1] is string))
                    throw new ArgumentException("Method Element takes two string parameters.");

                result = ResolveElement((string)args[0], (string)args[1]);
                return _tryGetMemberBehavior != TryGetMemberBehavior.Strict || result != null;
            }

            if (binder.Name.Equals("attribute", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 1 && !(args[0] is string))
                    throw new ArgumentException("Method Attribute takes one string parameter.");

                var xAttribute = XElement.Attribute((string)args[0]);
                if (xAttribute != null)
                    result = xAttribute.Value;
                return _tryGetMemberBehavior != TryGetMemberBehavior.Strict || result != null;
            }

            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var xElements = XElement.Elements(binder.Name);

            if (xElements.Count() == 1)
            {
                var xElement = xElements.Single();

                if (xElement.HasElements || xElement.HasAttributes)
                    throw new NotImplementedException();
                else
                    xElement.Value = (string)value;
            }
            else
                throw new NotImplementedException();

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var index = (int)indexes[0];

            if (XElement.Elements().Count() > index)
                XElement.Elements().ElementAt(index).Value = (string)value;

            return true;
        }

        public IEnumerator GetEnumerator()
        {
            // carlos.mendonca: believe it or not, this is equivalent to a yield return.
            return XElement.Elements().Select(xElement => xElement.Value).GetEnumerator();
        }

        public override string ToString()
        {
            return XElement.Value;
        }
    }
}