using System;
using System.Collections.Generic;
using Microsoft.Http;
using Microsoft.Http.Headers;

namespace SharpShooting.Http
{
    public interface IHttpClient : IDisposable
    {
        RequestHeaders DefaultHeaders { get; set; }
        Uri BaseAddress { get; set; }
        IList<HttpStage> Stages { get; set; }
        HttpWebRequestTransportSettings TransportSettings { get; set; }

        IAsyncResult BeginSend(HttpRequestMessage request, AsyncCallback callback, object state);
        HttpResponseMessage EndSend(IAsyncResult result);
        HttpResponseMessage Send(HttpRequestMessage request);
        void SendAsync(HttpRequestMessage request);
        void SendAsync(HttpRequestMessage request, object userState);
        void SendAsyncCancel(object userState);
        HttpResponseMessage Send(HttpMethod method);
        HttpResponseMessage Send(HttpMethod method, Uri uri);
        HttpResponseMessage Send(HttpMethod method, Uri uri, RequestHeaders headers);
        HttpResponseMessage Send(HttpMethod method, Uri uri, HttpContent content);
        HttpResponseMessage Send(HttpMethod method, string uri);
        HttpResponseMessage Send(HttpMethod method, string uri, RequestHeaders headers);
        HttpResponseMessage Send(HttpMethod method, string uri, HttpContent content);
        HttpResponseMessage Send(HttpMethod method, string uri, RequestHeaders headers, HttpContent content);
        HttpResponseMessage Send(HttpMethod method, Uri uri, RequestHeaders headers, HttpContent content);
        event EventHandler<SendCompletedEventArgs> SendCompleted;
    }
}