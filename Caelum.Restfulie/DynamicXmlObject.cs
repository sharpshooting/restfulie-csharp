using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Caelum.Restfulie
{
    public class DynamicXmlObject : DynamicObject, IEnumerable
    {
        private readonly XElement _xElement;

        public DynamicXmlObject(XElement xElement)
        {
            _xElement = xElement;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var memberName = binder.Name;

            if (memberName.Equals("value", StringComparison.InvariantCultureIgnoreCase))
                result = _xElement.Value;
            else
            {
                if (_xElement.Elements(memberName).Count() == 1)
                {
                    var xElement = _xElement.Elements(memberName).Single();

                    if (xElement.HasElements)
                        result = new DynamicXmlObject(xElement);
                    else
                        result = xElement.Value;
                }
                else if (_xElement.Elements(memberName).Count() > 1)
                    result = new DynamicXmlObject(new XElement(_xElement.Name, _xElement.Elements(memberName)));
                else
                    result = null;
            }

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var index = (int) indexes[0];

            if (_xElement.Elements().Count() > index)
                result = _xElement.Elements().ElementAt(index).Value;
            else
                result = null;

            return true;
        }

        public IEnumerator GetEnumerator()
        {
            // carlos.mendonca: believe it or not, this is equivalent to a yield return.
            return _xElement.Elements().Select(xElement => xElement.Value).GetEnumerator();
        }
    }
}
