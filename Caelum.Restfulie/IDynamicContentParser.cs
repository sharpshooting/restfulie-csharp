using System;
using System.Dynamic;

namespace Caelum.Restfulie
{
    public interface IDynamicContentParser
    {
        bool TryGetMember(GetMemberBinder binder, out object result);
        bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result);
        bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result);

        Uri UriFor(string stateTransition);
    }
}