using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// HTTP cookie with standard attributes.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>Cookie</c> struct in the original library:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L155">types.go#L155</a>
/// </remarks>
public class ClientCookie
{
    /// <summary>
    /// Cookie name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cookie value.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Domain the cookie is valid for.
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Path the cookie is valid for.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Expiration time (Unix timestamp). 0 for session cookie.
    /// </summary>
    [JsonPropertyName("expires")]
    public long Expires { get; set; } = 0;

    /// <summary>
    /// Max age in seconds.
    /// </summary>
    [JsonPropertyName("maxAge")]
    public long MaxAge { get; set; } = 0;

    /// <summary>
    /// Send only over HTTPS.
    /// </summary>
    [JsonPropertyName("secure")]
    public bool Secure { get; set; } = false;

    /// <summary>
    /// Inaccessible to JavaScript (HttpOnly).
    /// </summary>
    [JsonPropertyName("httpOnly")]
    public bool HttpOnly { get; set; } = false;

    /// <summary>
    /// Creates cookie with name and value.
    /// </summary>
    /// <param name="name">Cookie name</param>
    /// <param name="value">Cookie value</param>
    public ClientCookie(string name, string value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ClientCookie()
    { }
}