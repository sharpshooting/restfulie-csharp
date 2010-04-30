using Microsoft.Http;

namespace Caelum.Restfulie
{
    public interface IDynamicContentParserFactory
    {
        IDynamicContentParser New(HttpContent httpContent);
    }
}