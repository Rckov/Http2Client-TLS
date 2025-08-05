using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// Custom TLS fingerprint config. For advanced users - most should use browser presets.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>CustomTlsClient</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L90">types.go#L90</a>
/// </remarks>
public class CustomHttp2Client
{
    /// <summary>
    /// HTTP/2 settings to advertise.
    /// </summary>
    [JsonPropertyName("h2Settings")]
    public Dictionary<string, uint> Http2Settings { get; set; } = [];

    /// <summary>
    /// HTTP/2 header priority config.
    /// </summary>
    [JsonPropertyName("headerPriority")]
    public PriorityParam? HeaderPriority { get; set; }

    /// <summary>
    /// Certificate compression algorithms.
    /// </summary>
    [JsonPropertyName("certCompressionAlgos")]
    public List<string> CertificateCompressionAlgorithms { get; set; } = [];

    /// <summary>
    /// JA3 fingerprint string. Auto-generated if empty.
    /// </summary>
    [JsonPropertyName("ja3String")]
    public string Ja3Fingerprint { get; set; } = string.Empty;

    /// <summary>
    /// HTTP/2 settings order. Affects fingerprint.
    /// </summary>
    [JsonPropertyName("h2SettingsOrder")]
    public List<string> Http2SettingsOrder { get; set; } = [];

    /// <summary>
    /// Elliptic curves for key exchange.
    /// </summary>
    [JsonPropertyName("keyShareCurves")]
    public List<string> KeyShareCurves { get; set; } = [];

    /// <summary>
    /// ALPN protocols to advertise.
    /// </summary>
    [JsonPropertyName("alpnProtocols")]
    public List<string> AlpnProtocols { get; set; } = [];

    /// <summary>
    /// ALPS protocols for app layer settings.
    /// </summary>
    [JsonPropertyName("alpsProtocols")]
    public List<string> AlpsProtocols { get; set; } = [];

    /// <summary>
    /// ECH candidate payloads.
    /// </summary>
    [JsonPropertyName("ECHCandidatePayloads")]
    public List<ushort> EchCandidatePayloads { get; set; } = [];

    /// <summary>
    /// ECH cipher suites.
    /// </summary>
    [JsonPropertyName("ECHCandidateCipherSuites")]
    public List<CandidateCipherSuite> EchCandidateCipherSuites { get; set; } = [];

    /// <summary>
    /// HTTP/2 priority frames for stream prioritization.
    /// </summary>
    [JsonPropertyName("priorityFrames")]
    public List<PriorityFrame> PriorityFrames { get; set; } = [];

    /// <summary>
    /// HTTP/2 pseudo-header order.
    /// </summary>
    [JsonPropertyName("pseudoHeaderOrder")]
    public List<string> PseudoHeaderOrder { get; set; } = [];

    /// <summary>
    /// Delegated credentials algorithms.
    /// </summary>
    [JsonPropertyName("supportedDelegatedCredentialsAlgorithms")]
    public List<string> SupportedDelegatedCredentialsAlgorithms { get; set; } = [];

    /// <summary>
    /// Signature algorithms to advertise.
    /// </summary>
    [JsonPropertyName("supportedSignatureAlgorithms")]
    public List<string> SupportedSignatureAlgorithms { get; set; } = [];

    /// <summary>
    /// TLS versions to support.
    /// </summary>
    [JsonPropertyName("supportedVersions")]
    public List<string> SupportedVersions { get; set; } = [];

    /// <summary>
    /// HTTP/2 connection flow control window size.
    /// </summary>
    [JsonPropertyName("connectionFlow")]
    public uint ConnectionFlow { get; set; }

    /// <summary>
    /// TLS record size limit in bytes.
    /// </summary>
    [JsonPropertyName("recordSizeLimit")]
    public ushort RecordSizeLimit { get; set; }
}