using Http2Client.Core.Converters;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Http2Client.Utilities;

/// <summary>
/// JSON serialization with consistent settings.
/// </summary>
internal static class Serializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonBrowserTypeConverter()
        }
    };

    /// <summary>
    /// Serializes object to JSON string.
    /// </summary>
    public static string Serialize<T>(T data)
    {
        ThrowException.Null(data, nameof(data));
        return JsonSerializer.Serialize(data, Options);
    }

    /// <summary>
    /// Serializes object to UTF-8 bytes for native library.
    /// </summary>
    public static byte[] SerializeToBytes<T>(T data)
    {
        ThrowException.Null(data, nameof(data));
        return Encoding.UTF8.GetBytes(Serialize(data));
    }

    /// <summary>
    /// Deserializes JSON string to object.
    /// </summary>
    public static T? Deserialize<T>(string json)
    {
        ThrowException.NullOrEmpty(json, nameof(json));
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}