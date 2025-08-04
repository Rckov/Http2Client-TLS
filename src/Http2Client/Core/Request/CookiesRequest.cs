using Http2Client.Core.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Request;

/// <summary>
/// Request to add cookies to a session for a specific URL.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>AddCookiesToSessionInput</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L34">types.go#L34</a>
/// </remarks>
public class AddCookiesRequest
{
    /// <summary>
    /// Session ID to add cookies to.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; set; }

    /// <summary>
    /// URL the cookies are for (sets domain).
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// List of cookies to add to the session.
    /// </summary>
    [JsonPropertyName("cookies")]
    public List<ClientCookie> Cookies { get; set; } = [];
}

/// <summary>
/// Request to get cookies from a session for a specific URL.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>GetCookiesFromSessionInput</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L40">types.go#L40</a>
/// </remarks>
public class GetCookiesRequest
{
    /// <summary>
    /// Session ID to get cookies from.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; set; }

    /// <summary>
    /// URL to get cookies for (domain matching).
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}