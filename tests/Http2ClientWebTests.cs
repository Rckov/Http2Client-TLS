using FluentAssertions;

using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Request;

using System.Net;

using Xunit;

namespace Http2Client.Test;

public class Http2ClientWebTests
{
    private Http2Client? _client;

    static Http2ClientWebTests()
    {
        Http2Client.Initialize(TestConstants.LibraryPath);
    }

    [Fact]
    public void TlsPeet_Works()
    {
        _client ??= CreateClient();

        var request = new HttpRequest
        {
            RequestUrl = "https://tls.peet.ws/api/all",
            RequestMethod = "GET"
        };

        var response = _client.Send(request);

        response.Status.Should().Be(HttpStatusCode.OK);
        response.Body.Should().NotBeEmpty();

        // Check main structure
        response.Body.Should().Contain("\"donate\":");
        response.Body.Should().Contain("\"ip\":");
        response.Body.Should().Contain("\"http_version\":");
        response.Body.Should().Contain("\"method\":");
        response.Body.Should().Contain("\"user_agent\":");

        // Check TLS section
        response.Body.Should().Contain("\"tls\":");
        response.Body.Should().Contain("\"ja3_hash\":");
        response.Body.Should().Contain("\"ja4\":");
        response.Body.Should().Contain("\"peetprint_hash\":");

        // Check HTTP/2 section
        response.Body.Should().Contain("\"http2\":");
        response.Body.Should().Contain("\"akamai_fingerprint\":");
        response.Body.Should().Contain("\"akamai_fingerprint_hash\":");

        // Check protocol version
        response.Body.Should().Contain("\"http_version\": \"h2\"");
        response.Body.Should().Contain("\"method\": \"GET\"");
    }

    [Fact]
    public void SelfSigned_Fails()
    {
        _client ??= CreateClient();

        var request = new HttpRequest
        {
            RequestUrl = "https://self-signed.badssl.com/",
            RequestMethod = "GET",
            InsecureSkipVerify = false
        };

        var response = _client.Send(request);

        response.Status.Should().Be(0);
        response.IsSuccessStatus.Should().BeFalse();
    }

    [Fact]
    public void Get_Works()
    {
        _client ??= CreateClient();

        var request = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/get",
            RequestMethod = "GET"
        };

        var response = _client.Send(request);

        response.Status.Should().Be(HttpStatusCode.OK);
        response.Body.Should().NotBeEmpty();
        response.Body.Should().Contain("\"url\": \"https://httpbin.org/get\"");
    }

    [Fact]
    public void Post_Works()
    {
        _client ??= CreateClient();

        var request = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/post",
            RequestMethod = "POST",
            RequestBody = "{\"test\": \"data\"}",
            Headers = { ["Content-Type"] = "application/json" }
        };

        var response = _client.Send(request);

        response.Status.Should().Be(HttpStatusCode.OK);
        response.Body.Should().NotBeEmpty();
        response.Body.Should().Contain("\"test\": \"data\"");
    }

    [Fact]
    public void Headers_Work()
    {
        _client ??= CreateClient();

        var request = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/headers",
            RequestMethod = "GET",
            Headers = { ["X-Integration-Test"] = "web-test-value" }
        };

        var response = _client.Send(request);

        response.Status.Should().Be(HttpStatusCode.OK);
        response.Body.Should().Contain("X-Integration-Test");
        response.Body.Should().Contain("web-test-value");
    }

    private Http2Client CreateClient()
    {
        return new HttpClientBuilder()
            .WithBrowserType(BrowserType.Chrome133)
            .WithUserAgent("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36")
            .WithCookies()
            .Build();
    }
}