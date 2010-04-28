using System;
using Microsoft.Http;

namespace SharpShooting.Http
{
    /// <summary>
    /// This class can be used if you don't want your implementation to hold a reference to
    /// SharpShooting.Http.IHttpClient, which is implemented by SharpShooting.Http.HttpClientWrapper
    /// (adapter/DI pattern). What you do is use it in your test to set behavior. An idea for
    /// improvement is expose delegates just like Microsoft Moles does, so that
    /// HttpWebRequestTransportStakeFake's behavior can be changed on the test body.
    /// </summary>
    public class HttpClientFake : HttpClient
    {
        private readonly HttpResponseMessage _httpResponseMessage;
        private readonly object _state;

        public HttpClientFake(HttpResponseMessage httpResponseMessage, object state)
            : base()
        {
            _httpResponseMessage = httpResponseMessage;
            _state = state;
        }

        protected override HttpStage CreateTransportStage()
        {
            return new HttpWebRequestTransportStageFake(_httpResponseMessage, _state);
        }

        internal class HttpWebRequestTransportStageFake : HttpWebRequestTransportStage
        {
            private readonly HttpResponseMessage _httpResponseMessage;
            private readonly object _state;

            public HttpWebRequestTransportStageFake(HttpResponseMessage httpResponseMessage, object state)
            {
                _httpResponseMessage = httpResponseMessage;
                _state = state;
            }

            protected override IAsyncResult BeginProcessRequestAndTryGetResponse(HttpRequestMessage request, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            protected override IAsyncResult BeginProcessResponse(HttpResponseMessage response, object state, AsyncCallback callback, object callbackState)
            {
                throw new NotImplementedException();
            }

            protected override void EndProcessRequestAndTryGetResponse(IAsyncResult result, out HttpResponseMessage response, out object state)
            {
                throw new NotImplementedException();
            }

            protected override void EndProcessResponse(IAsyncResult result)
            {
                throw new NotImplementedException();
            }

            protected override void ProcessRequestAndTryGetResponse(HttpRequestMessage request, out HttpResponseMessage response, out object state)
            {
                response = _httpResponseMessage;

                response.Request = request;
                response.Method = request.Method;

                state = _state;
            }

            protected override void ProcessResponse(HttpResponseMessage response, object state)
            {
                // carlos.mendonca: do nothing else with the response.
            }
        }
    }
}