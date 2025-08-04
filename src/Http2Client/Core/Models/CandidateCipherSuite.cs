using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// Encrypted Client Hello (ECH) cipher suite configuration.
/// Part of advanced TLS fingerprinting. Most users won't need this.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>CandidateCipherSuite</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L124">types.go#L124</a>
/// </remarks>
public class CandidateCipherSuite
{
    /// <summary>
    /// Key Derivation Function identifier.
    /// </summary>
    [JsonPropertyName("kdfId")]
    public string KdfId { get; set; } = string.Empty;

    /// <summary>
    /// Authenticated Encryption with Associated Data identifier.
    /// </summary>
    [JsonPropertyName("aeadId")]
    public string AeadId { get; set; } = string.Empty;
}