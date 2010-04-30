using Microsoft.Http;

namespace Caelum.Restfulie
{
    public interface IDynamicContentParserFactory
    {
        dynamic New(HttpContent httpContent);
    }
}