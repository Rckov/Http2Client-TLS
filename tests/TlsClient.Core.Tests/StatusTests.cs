using FluentAssertions;
using System.Net;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Tests
{
    public class StatusTests
    {
        [Fact]
        public async Task ShouldReturn200()
        {
            var tlsClient = new ClientBuilder().Build();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/get",
                RequestMethod = HttpMethod.Get,
            });

            response.IsSuccessStatus.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldReturn400()
        {
            var tlsClient = new Core.TlsClient();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/status/400",
                RequestMethod = HttpMethod.Get,
            });
            response.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ShouldFollowRedirect_WithOptions()
        {
            var tlsClient= new ClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .Build();
           
            var targetUrl = "https://httpbin.org/get";


            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = $"https://httpbin.org/redirect-to?url={WebUtility.UrlEncode(targetUrl)}",
                RequestMethod = HttpMethod.Get,
            });

            response.Status.Should().Be(HttpStatusCode.OK);
            response.Target.Should().Be(targetUrl);
        }

        [Fact]
        public async Task ShouldNotFollowRedirect_WithOptions()
        {
            var tlsClient = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome132, "TestClient 1.0")
            {
                FollowRedirects = false,
            });
            var targetUrl = "https://httpbin.org/get";
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = $"https://httpbin.org/redirect-to?url={WebUtility.UrlEncode(targetUrl)}",
                RequestMethod = HttpMethod.Get,
            });
            response.Status.Should().Be(HttpStatusCode.Found);
            response.Target.Should().NotBe(targetUrl);
        }

        [Fact]
        public async Task ShouldFollowRedirect()
        {
            var tlsClient = new Core.TlsClient();
            var targetUrl = "https://httpbin.org/get";
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/get",
                RequestMethod = HttpMethod.Get,
                FollowRedirects = true,
            });
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Target.Should().Be(targetUrl);
        }

        [Fact]
        public async Task ShouldNotFollowRedirect()
        {
            var tlsClient = new Core.TlsClient();
            var targetUrl = "https://httpbin.org/get";
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = $"https://httpbin.org/redirect-to?url={WebUtility.UrlEncode(targetUrl)}",
                RequestMethod = HttpMethod.Get,
                FollowRedirects = false,
            });
            response.Status.Should().Be(HttpStatusCode.Found);
            response.Target.Should().NotBe(targetUrl);
        }

        [Fact]
        public async Task ShouldTimeout()
        {
            var tlsClient = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome132, "TestClient 1.0")
            {
                Timeout = TimeSpan.FromSeconds(1),
            });
            var request = new Request
            {
                RequestUrl = "https://httpbin.org/delay/5",
                RequestMethod = HttpMethod.Get
            };
            var response = await tlsClient.RequestAsync(request);
            response.Status.Should().Be(0);
            response.Body.Should().Contain("Timeout");
        }

        [Fact]
        public async Task ShouldNotTimeout()
        {
            var tlsClient = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome132, "TestClient 1.0")
            {
                Timeout = TimeSpan.FromSeconds(10),
            });
            var request = new Request
            {
                RequestUrl = "https://httpbin.org/delay/5",
                RequestMethod = HttpMethod.Get
            };
            var response = await tlsClient.RequestAsync(request);
            response.Status.Should().Be(HttpStatusCode.OK);
            response.IsSuccessStatus.Should().BeTrue();
        }
    }
}