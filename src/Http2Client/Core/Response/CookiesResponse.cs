using Http2Client.Core.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Response;

/// <summary>
/// Response from cookie management operations. Contains the cookies from a session.
/// </summary>
/// <remarks>
/// Corresponds to Go struct <c>CookiesFromSessionOutput</c>:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L45C1-L45C6">types.go#L45C1-L45C6</a>
/// </remarks>
public class CookiesResponse
{
    /// <summary>
    /// Response ID for tracking.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// List of cookies from the session.
    /// </summary>
    [JsonPropertyName("cookies")]
    public List<ClientCookie> Cookies { get; set; } = [];
}