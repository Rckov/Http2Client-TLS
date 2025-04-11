using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using TlsClient.Core;
using TlsClient.Core.Helpers.Builders;
using System.Linq;

using System.Collections.Generic;
using System.Net;
using TlsClient.HttpClient.Helpers;
using System.Net.Http.Headers;

namespace TlsClient.HttpClient
{
    public class TlsClientHandler : HttpClientHandler
    {
        private readonly Core.TlsClient _client;
        public TlsClientHandler(Core.TlsClient client)
        {
            _client = client;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tlsRequestBuilder = new RequestBuilder()
                .WithUrl(request.RequestUri.ToString())
                .WithMethod(request.Method)
                .WithByteRequest()
                .WithByteResponse();

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                tlsRequestBuilder.WithBody(content);

                if (request.Content?.Headers?.ContentType is MediaTypeHeaderValue contentType)
                {
                    tlsRequestBuilder.WithHeader("Content-Type", contentType.ToString());
                }
            }

            foreach (var header in request.GetHeaderDictionary())
            {
                tlsRequestBuilder.WithHeader(header.Key, header.Value);
            }

            // Adding cookies with header, native tls client not tested with cookiejar
            var cookies = this.CookieContainer?.GetAllCookies();
            if (cookies?.Count() > 0)
            {
                var cookieHeader = string.Join("; ", cookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
                tlsRequestBuilder.WithHeader("Cookie", cookieHeader);
            }

            var tlsRequest= tlsRequestBuilder.Build();
            
            var response = await _client.RequestAsync(tlsRequest, cancellationToken);
           

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = response.Status,
                Version = HttpVersionHelper.Map(response.UsedProtocol),
                RequestMessage = request,
            };

            if (!string.IsNullOrWhiteSpace(response.Body))
            {
                var parsed = response.Body.ToParsedBase64();
                httpResponseMessage.Content = new ByteArrayContent(Convert.FromBase64String(parsed.Item2));
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(parsed.Item1);
            }
            else
            {
                httpResponseMessage.Content = new ByteArrayContent(Array.Empty<byte>());
            }

            foreach (var header in response.Headers)
            {
                httpResponseMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return httpResponseMessage;
        }
    }
}
