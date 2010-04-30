using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SharpShooting.Dynamic
{
    public class DynamicJsonObject : DynamicObject
    {
        private readonly JToken _jToken;

        public DynamicJsonObject(JToken jToken)
        {
            _jToken = jToken;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        public override string ToString()
        {
            return _jToken.Value<string>();
        }
    }
}