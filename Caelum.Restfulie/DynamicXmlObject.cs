using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Caelum.Restfulie
{
    public class DynamicXmlObject : DynamicObject
    {
        private readonly XElement _xElement;

        public DynamicXmlObject(XElement xElement)
        {
            _xElement = xElement;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            var memberName = binder.Name;

            if (memberName.Equals("value", StringComparison.InvariantCultureIgnoreCase))
                result = _xElement.Value;
            else
            {
                var xElement = _xElement.Elements(memberName).SingleOrDefault();
                if (xElement != null) result = xElement.Value;
            }

            return true;
        }
    }
}
