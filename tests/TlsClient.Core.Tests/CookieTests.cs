using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Tests
{
    public class CookieTests
    {

        [Fact]
        public async Task ShouldSendRequestCookies()
        {
            var tlsClient = new Core.TlsClient();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/cookies",
                RequestMethod = HttpMethod.Get,
                RequestCookies= new List<TlsClientCookie>()
                {
                    new TlsClientCookie("TestCookie", "CookieValue123")
                },  
            });
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("\"TestCookie\": \"CookieValue123\"");
        }

        [Fact]
        public async Task ShouldCaptureSetCookieFromResponse()
        {
            var tlsClient = new Core.TlsClient();
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/cookies/set?MyCookie=MyValue",
                RequestMethod = HttpMethod.Get,
                WithDefaultCookieJar = true
            });
            response.Status.Should().Be(HttpStatusCode.Found);
            response.Cookies["MyCookie"].Should().Be("MyValue");
        }

        [Fact]
        public async Task ShouldPersistCookiesAcrossRequests()
        {
            var tlsClient = new Core.TlsClient();
            var setCookieResponse = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/cookies/set/TestSession/Session123",
                RequestMethod = HttpMethod.Get,
                WithDefaultCookieJar = true
            });

            setCookieResponse.Status.Should().Be(HttpStatusCode.Found);

            var getCookieResponse = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/cookies",
                RequestMethod = HttpMethod.Get,
                WithDefaultCookieJar = true
            });

            getCookieResponse.Status.Should().Be(HttpStatusCode.OK);
            getCookieResponse.Body.Should().Contain("\"TestSession\": \"Session123\"");
        }

        [Fact]
        public async Task ShouldSendCookieViaHeader()
        {
            var tlsClient = new Core.TlsClient();
            tlsClient.DefaultHeaders.Add("Cookie", new List<string>() { "HeaderCookie2=FromHeader321" });
            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://httpbin.org/cookies",
                RequestMethod = HttpMethod.Get,
                Headers = new Dictionary<string, string>
                {
                    { "Cookie", "HeaderCookie=FromHeader123" }
                },
            });

            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("\"HeaderCookie\": \"FromHeader123\"");
        }


    }
}