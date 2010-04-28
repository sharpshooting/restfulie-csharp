using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

// carlos.mendonca: this has become a general-purpose fluent XML parser; consider moving it to
//                  SharpShooting.Dynamic.Xml and using it as a base class in Caelum.Restfulie.
namespace Caelum.Restfulie
{
    public class DynamicXmlObject : DynamicObject, IEnumerable
    {
        public enum TryGetMemberBehavior { Loose, Strict }

        private readonly XElement _xElement;
        private readonly TryGetMemberBehavior _tryGetMemberBehavior;

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
            var xName = XName.Get(localName, namespaceName);
            
            if (_xElement.Elements(xName).Count() == 1)
            {
                var xElement = _xElement.Elements(xName).Single();

                if (xElement.HasElements)
                    result = new DynamicXmlObject(xElement);
                else
                    result = xElement.Value;
            }
            else if (_xElement.Elements(xName).Count() > 1)
                result = new DynamicXmlObject(new XElement(_xElement.Name, _xElement.Elements(xName)));
            else
                result = null;
            
            return result;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var index = (int)indexes[0];

            if (_xElement.Elements().Count() > index)
                result = _xElement.Elements().ElementAt(index).Value;
            else
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

                result = ResolveElement((string) args[0], (string) args[1]);
                return _tryGetMemberBehavior != TryGetMemberBehavior.Strict || result != null;
            }

            return result != null;
        }

        public IEnumerator GetEnumerator()
        {
            // carlos.mendonca: believe it or not, this is equivalent to a yield return.
            return _xElement.Elements().Select(xElement => xElement.Value).GetEnumerator();
        }

        public override string ToString()
        {
            return _xElement.Value;
        }
    }
}
