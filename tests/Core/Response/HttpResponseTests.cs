using FluentAssertions;

using Http2Client.Core.Response;

using System.Net;

using Xunit;

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
    public void IsSuccess_Works(HttpStatusCode status, bool expected)
    {
        var response = new HttpResponse { Status = status };
        response.IsSuccessStatus.Should().Be(expected);
    }

    [Fact]
    public void GetHeader_Exists_Works()
    {
        var response = new HttpResponse();
        response.Headers["Content-Type"] = ["application/json", "charset=utf-8"];

        var result = response.GetHeader("Content-Type");
        result.Should().Be("application/json");
    }

    [Fact]
    public void GetHeader_Missing_Null()
    {
        var response = new HttpResponse();

        var result = response.GetHeader("Non-Existing");
        result.Should().BeNull();
    }

    [Fact]
    public void GetHeaderValues_Exists_Works()
    {
        var response = new HttpResponse();
        response.Headers["Set-Cookie"] = ["cookie1=value1", "cookie2=value2"];

        var result = response.GetHeaderValues("Set-Cookie");
        result.Should().BeEquivalentTo(["cookie1=value1", "cookie2=value2"]);
    }

    [Fact]
    public void GetHeaderValues_Missing_Empty()
    {
        var response = new HttpResponse();

        var result = response.GetHeaderValues("Non-Existing");
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetCookie_Exists_Works()
    {
        var response = new HttpResponse();
        response.Cookies["session"] = "abc123";

        var result = response.GetCookie("session");
        result.Should().Be("abc123");
    }

    [Fact]
    public void GetCookie_Missing_Null()
    {
        var response = new HttpResponse();

        var result = response.GetCookie("session");
        result.Should().BeNull();
    }

    [Fact]
    public void ContentLength_Valid_Works()
    {
        var response = new HttpResponse();
        response.Headers["Content-Length"] = ["1024"];
        response.ContentLength.Should().Be(1024);
    }

    [Fact]
    public void ContentLength_Invalid_MinusOne()
    {
        var response = new HttpResponse();
        response.Headers["Content-Length"] = ["invalid"];

        response.ContentLength.Should().Be(-1);
    }
}