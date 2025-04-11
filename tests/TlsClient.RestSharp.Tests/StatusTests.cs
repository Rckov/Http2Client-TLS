using FluentAssertions;
using RestSharp;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.RestSharp.Tests
{
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
    }
}