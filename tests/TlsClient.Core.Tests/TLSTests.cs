using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using TlsClient.Core.Models;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Tests
{
    public class TLSTests
    {

        [Fact]
        public async Task ShouldFailSSL()
        {
            var tlsClient = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome132, "TestClient 1.0")
            {
                InsecureSkipVerify = false,
            });
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://self-signed.badssl.com/",
                RequestMethod = HttpMethod.Get,
            });
            response.Status.Should().Be(0);
            response.IsSuccessStatus.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldNotFailSSL()
        {
            var tlsClient = new Core.TlsClient(new TlsClientOptions(TlsClientIdentifier.Chrome132, "TestClient 1.0")
            {
                InsecureSkipVerify = true,
            });
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://self-signed.badssl.com/",
                RequestMethod = HttpMethod.Get,
            });
            response.Status.Should().Be(HttpStatusCode.OK);
            response.IsSuccessStatus.Should().BeTrue();
        }

        [Fact] 
        public async Task ShouldUseServerNameOverwrite()
        {
            var tlsClient = new Core.TlsClient();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://tls.peet.ws/api/all",
                RequestMethod = HttpMethod.Get,
                ServerNameOverwrite = "example.com",
                InsecureSkipVerify= true
            });

            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("\"server_name\": \"example.com\"");
        }
    }
}