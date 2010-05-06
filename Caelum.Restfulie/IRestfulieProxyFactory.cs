namespace Caelum.Restfulie.Tests
{
    public interface IRestfulieProxyFactory
    {
        RestfulieProxy Get();

        RestfulieProxy Create(string contentType, object content);
    }
}