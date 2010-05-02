using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Http;

namespace Caelum.Restfulie
{
    public interface IHttpMethodDiscovery
    {
        HttpMethod MethodFor(string rel);
    }
}
