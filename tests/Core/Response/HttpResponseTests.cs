using FluentAssertions;

using Http2Client.Core.Response;

using System.Net;

namespace Http2Client.Test.Core.Response;

public class HttpResponseTests
{
    [Theory]
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData(HttpStatusCode.Created, true)]
    [InlineData(HttpStatusCode.NoContent, true)]
    [InlineData(HttpStatusCode.BadRequest, false)]
    [InlineData(HttpStatusCode.NotFound, false)]
    [InlineData(HttpStatusCode.InternalServerError, false)]
    public void IsSuccessStatus_ReturnsCorrectValue(HttpStatusCode status, bool expected)
    {
        var response = new HttpResponse { Status = status };

        response.IsSuccessStatus.Should().Be(expected);
    }

    [Fact]
    public void GetHeader_ExistingHeader_ReturnsFirstValue()
    {
        var response = new HttpResponse();
        response.Headers["Content-Type"] = ["application/json", "charset=utf-8"];

        var result = response.GetHeader("Content-Type");

        result.Should().Be("application/json");
    }

    [Fact]
    public void GetHeader_NonExistingHeader_ReturnsNull()
    {
        var response = new HttpResponse();

        var result = response.GetHeader("Non-Existing");

        result.Should().BeNull();
    }

    [Fact]
    public void GetHeaderValues_ExistingHeader_ReturnsAllValues()
    {
        var response = new HttpResponse();
        response.Headers["Set-Cookie"] = ["cookie1=value1", "cookie2=value2"];

        var result = response.GetHeaderValues("Set-Cookie");

        result.Should().BeEquivalentTo(["cookie1=value1", "cookie2=value2"]);
    }

    [Fact]
    public void GetHeaderValues_NonExistingHeader_ReturnsEmptyList()
    {
        var response = new HttpResponse();

        var result = response.GetHeaderValues("Non-Existing");

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetCookie_ExistingCookie_ReturnsValue()
    {
        var response = new HttpResponse();
        response.Cookies["session"] = "abc123";

        var result = response.GetCookie("session");

        result.Should().Be("abc123");
    }

    [Fact]
    public void GetCookie_NonExistingCookie_ReturnsNull()
    {
        var response = new HttpResponse();

        var result = response.GetCookie("session");

        result.Should().BeNull();
    }

    [Fact]
    public void ContentLength_ValidHeader_ReturnsLength()
    {
        var response = new HttpResponse();
        response.Headers["Content-Length"] = ["1024"];

        response.ContentLength.Should().Be(1024);
    }

    [Fact]
    public void ContentLength_InvalidHeader_ReturnsMinusOne()
    {
        var response = new HttpResponse();
        response.Headers["Content-Length"] = ["invalid"];

        response.ContentLength.Should().Be(-1);
    }
}