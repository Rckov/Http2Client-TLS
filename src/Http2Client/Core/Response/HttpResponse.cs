using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Response;

/// <summary>
/// HTTP response with all the data you need. Includes helpers for common tasks.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>Response</c> struct in the original library:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L188">types.go#L188</a>
/// </remarks>
public class HttpResponse
{
    /// <summary>
    /// Unique response ID that matches the request ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Response body as string. Empty if it was a binary response.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code like 200, 404, etc.
    /// </summary>
    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; set; }

    /// <summary>
    /// All response headers. Some headers can have multiple values.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, List<string>> Headers { get; set; } = [];

    /// <summary>
    /// Cookies from Set-Cookie headers, parsed for convenience.
    /// </summary>
    [JsonPropertyName("cookies")]
    public Dictionary<string, string> Cookies { get; set; } = [];

    /// <summary>
    /// Session ID this response belongs to.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    /// Final URL after redirects.
    /// </summary>
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// HTTP version used (HTTP/1.1, HTTP/2, etc).
    /// </summary>
    [JsonPropertyName("usedProtocol")]
    public string UsedProtocol { get; set; } = string.Empty;

    /// <summary>
    /// True if status code indicates success (2xx range).
    /// </summary>
    public bool IsSuccessStatus => (int)Status is >= 200 and < 300;

    /// <summary>
    /// Get first value of a header. Returns null if header doesn't exist.
    /// </summary>
    /// <param name="name">Header name (case-insensitive usually)</param>
    /// <returns>First header value or null</returns>
    public string? GetHeader(string name)
    {
        return Headers.TryGetValue(name, out var values) && values.Count > 0 ? values[0] : null;
    }

    /// <summary>
    /// Get all values for a header. Some headers can appear multiple times.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <returns>List of header values (empty if header doesn't exist)</returns>
    public List<string> GetHeaderValues(string name)
    {
        return Headers.TryGetValue(name, out var values) ? values : [];
    }

    /// <summary>
    /// Check if response has a specific header.
    /// </summary>
    /// <param name="name">Header name to check</param>
    /// <returns>True if header exists</returns>
    public bool HasHeader(string name)
    {
        return GetHeaderValues(name).Count > 0;
    }

    /// <summary>
    /// Get cookie value by name. Convenient for parsed Set-Cookie headers.
    /// </summary>
    /// <param name="name">Cookie name</param>
    /// <returns>Cookie value or null if not found</returns>
    public string? GetCookie(string name)
    {
        return Cookies.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>
    /// Check if response has a specific cookie.
    /// </summary>
    /// <param name="name">Cookie name to check</param>
    /// <returns>True if cookie exists</returns>
    public bool HasCookie(string name)
    {
        return Cookies.ContainsKey(name);
    }

    /// <summary>
    /// Content-Type header value. Null if not present.
    /// </summary>
    public string? ContentType => GetHeader("Content-Type");

    /// <summary>
    /// Content length from header. Returns -1 if header is missing or invalid.
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