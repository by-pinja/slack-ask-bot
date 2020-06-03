using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SlackLib.Tests
{
    public class HttpMessageHandle : HttpMessageHandler
        {
            private HttpResponseMessage _response;
            public HttpMessageHandle(HttpResponseMessage responseMessage)
            {
                _response = responseMessage;
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        public class MockLogger<T> : ILogger<T>
        {
            public LogLevel LastLogLevel { get; set; }
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                LastLogLevel = logLevel;
            }
        }
}