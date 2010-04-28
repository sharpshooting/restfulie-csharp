using System;
using System.Collections.Generic;
using Microsoft.Http;
using Microsoft.Http.Headers;

namespace SharpShooting.Http
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public RequestHeaders DefaultHeaders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Uri BaseAddress
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IList<HttpStage> Stages
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public HttpWebRequestTransportSettings TransportSettings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IAsyncResult BeginSend(HttpRequestMessage request, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage EndSend(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        public void SendAsync(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        public void SendAsync(HttpRequestMessage request, object userState)
        {
            throw new NotImplementedException();
        }

        public void SendAsyncCancel(object userState)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, Uri uri)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, Uri uri, RequestHeaders headers)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, Uri uri, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, string uri)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, string uri, RequestHeaders headers)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, string uri, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, string uri, RequestHeaders headers, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Send(HttpMethod method, Uri uri, RequestHeaders headers, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        public void Dispose()
        {
            // carlos.mendonca: Microsoft's reference implementation: http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
                _httpClient.Dispose();

            _disposed = true;
        }

        ~HttpClientWrapper()
        {
            Dispose(false);
        }
    }
}