namespace Caelum.Restfulie.Tests
{
    public interface IRestfulieProxyFactory
    {
        RestfulieProxy Get();

        RestfulieProxy Create(object content);

        IRestfulieProxyFactory As(string contentType);
    }
}