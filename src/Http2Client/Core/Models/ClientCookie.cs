using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// HTTP cookie with all the standard attributes. Works like browser cookies.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>Cookie</c> struct in the original library:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L155">types.go#L155</a>
/// </remarks>
public class ClientCookie
{
    /// <summary>
    /// Cookie name like "sessionid".
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cookie value.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Domain the cookie is valid for. Like ".example.com".
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Path the cookie is valid for. Usually "/".
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// When cookie expires (Unix timestamp). 0 means session cookie.
    /// </summary>
    [JsonPropertyName("expires")]
    public long Expires { get; set; } = 0;

    /// <summary>
    /// Max age in seconds. Alternative to Expires.
    /// </summary>
    [JsonPropertyName("maxAge")]
    public long MaxAge { get; set; } = 0;

    /// <summary>
    /// True if cookie should only be sent over HTTPS.
    /// </summary>
    [JsonPropertyName("secure")]
    public bool Secure { get; set; } = false;

    /// <summary>
    /// True if cookie is inaccessible to JavaScript (HttpOnly flag).
    /// </summary>
    [JsonPropertyName("httpOnly")]
    public bool HttpOnly { get; set; } = false;

    /// <summary>
    /// Create cookie with name and value. Other fields can be set later.
    /// </summary>
    /// <param name="name">Cookie name</param>
    /// <param name="value">Cookie value</param>
    public ClientCookie(string name, string value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Default constructor for deserialization.
    /// </summary>
    public ClientCookie()
    { }
}