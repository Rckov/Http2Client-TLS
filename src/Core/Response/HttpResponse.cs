using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Response;

/// <summary>
/// HTTP response with helper methods for common tasks.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>Response</c> struct in the original library:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L188">types.go#L188</a>
/// </remarks>
public class HttpResponse
{
    /// <summary>
    /// Response ID matching the request.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Response body as string. Empty for binary responses.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code (200, 404, etc).
    /// </summary>
    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; set; }

    /// <summary>
    /// Response headers. Headers can have multiple values.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, List<string>> Headers { get; set; } = [];

    /// <summary>
    /// Parsed cookies from Set-Cookie headers.
    /// </summary>
    [JsonPropertyName("cookies")]
    public Dictionary<string, string> Cookies { get; set; } = [];

    /// <summary>
    /// Session ID for this response.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    /// Final URL after following redirects.
    /// </summary>
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// HTTP protocol version used.
    /// </summary>
    [JsonPropertyName("usedProtocol")]
    public string UsedProtocol { get; set; } = string.Empty;

    /// <summary>
    /// True for 2xx status codes.
    /// </summary>
    public bool IsSuccessStatus => (int)Status is >= 200 and < 300;

    /// <summary>
    /// Gets first header value or null if not found.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <returns>First header value or null</returns>
    public string? GetHeader(string name)
    {
        return Headers.TryGetValue(name, out var values) && values.Count > 0 ? values[0] : null;
    }

    /// <summary>
    /// Gets all values for a header.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <returns>Header values or empty list</returns>
    public List<string> GetHeaderValues(string name)
    {
        return Headers.TryGetValue(name, out var values) ? values : [];
    }

    /// <summary>
    /// Checks if header exists.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <returns>True if header exists</returns>
    public bool HasHeader(string name)
    {
        return GetHeaderValues(name).Count > 0;
    }

    /// <summary>
    /// Gets cookie value by name.
    /// </summary>
    /// <param name="name">Cookie name</param>
    /// <returns>Cookie value or null</returns>
    public string? GetCookie(string name)
    {
        return Cookies.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>
    /// Checks if cookie exists.
    /// </summary>
    /// <param name="name">Cookie name</param>
    /// <returns>True if cookie exists</returns>
    public bool HasCookie(string name)
    {
        return Cookies.ContainsKey(name);
    }

    /// <summary>
    /// Content-Type header value.
    /// </summary>
    public string? ContentType => GetHeader("Content-Type");

    /// <summary>
    /// Content length from header. Returns -1 if missing or invalid.
    /// </summary>
    public long ContentLength
    {
        get
        {
            var header = GetHeader("Content-Length");
            return long.TryParse(header, out var length) ? length : -1;
        }
    }
}