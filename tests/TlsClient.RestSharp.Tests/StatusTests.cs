using FluentAssertions;
using RestSharp;
using RestSharp.Interceptors;
using System.Net;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.RestSharp.Tests
{
    public class TestInterceptor : Interceptor
    {
        public override ValueTask AfterRequest(RestResponse response, CancellationToken cancellationToken)
        {
            response.Content= "Test Interceptor";
            return base.AfterRequest(response, cancellationToken);
        }
    }

    public class StatusTests
    {
        [Fact]
        public async void ShouldTimeout()
        {
            var tlsClient= new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithTimeout(TimeSpan.FromSeconds(1))
                .Build();

            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();


            var restReq= new RestRequest("/delay/5", Method.Get);
            var restResponse = await restClient.ExecuteAsync(restReq);

            restResponse.StatusCode.Should().Be(0);
            restResponse.ResponseStatus.Should().Be(ResponseStatus.TimedOut);
        }

        [Fact]
        public async void ShouldError()
        {
            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithTimeout(TimeSpan.FromSeconds(10))
                .Build();
            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin234534543.org")
                .WithTlsClient(tlsClient)
                .Build();

            var restReq = new RestRequest("/get", Method.Get);
            var restResponse = await restClient.ExecuteAsync(restReq);

            restResponse.StatusCode.Should().Be(0);
            restResponse.ResponseStatus.Should().Be(ResponseStatus.Error);
        }

        [Fact]
        public async void ShouldInvokeInterceptor()
        {
            var tlsClient = new TlsClientBuilder()
               .WithIdentifier(TlsClientIdentifier.Chrome132)
               .WithUserAgent("TestClient 1.0")
               .WithFollowRedirects(true)
               .WithTimeout(TimeSpan.FromSeconds(10))
               .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
               .Build();

            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .WithConfigureRestClient((x) =>
                {
                    x.Interceptors= new List<Interceptor>()
                    {
                        new TestInterceptor()
                    };
                })
                .Build();

            var restReq = new RestRequest("/get", Method.Get);
            var restResponse = await restClient.ExecuteAsync(restReq);

            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Content.Should().Contain("Test Interceptor");
        }
    }
}