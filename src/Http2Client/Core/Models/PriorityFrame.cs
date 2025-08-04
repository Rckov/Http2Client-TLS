using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// HTTP/2 priority frame configuration. Used for advanced TLS fingerprinting.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>PriorityFrames</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L144">types.go#L144</a>
/// </remarks>
public class PriorityFrame
{
    /// <summary>
    /// Priority parameters for this frame.
    /// </summary>
    [JsonPropertyName("priorityParam")]
    public PriorityParam PriorityParam { get; set; } = new();

    /// <summary>
    /// HTTP/2 stream ID this priority applies to.
    /// </summary>
    [JsonPropertyName("streamID")]
    public uint StreamID { get; set; }
}

/// <summary>
/// HTTP/2 priority parameters. Controls stream priority and dependencies.
/// </summary>
public class PriorityParam
{
    /// <summary>
    /// Stream ID this stream depends on.
    /// </summary>
    [JsonPropertyName("streamDep")]
    public uint StreamDep { get; set; }

    /// <summary>
    /// Whether this stream has exclusive dependency.
    /// </summary>
    [JsonPropertyName("exclusive")]
    public bool Exclusive { get; set; }

    /// <summary>
    /// Priority weight (1-256). Higher means more important.
    /// </summary>
    [JsonPropertyName("weight")]
    public byte Weight { get; set; }
}