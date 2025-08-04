using Http2Client.Core.Enums;
using Http2Client.Extensions;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Converters;

/// <summary>
/// JSON converter for <see cref="BrowserType"/> enum. Converts to/from string values.
/// </summary>
internal class JsonBrowserTypeConverter : JsonConverter<BrowserType?>
{
    /// <summary>
    /// Read <see cref="BrowserType"/> from JSON string.
    /// </summary>
    public override BrowserType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => string.IsNullOrEmpty(reader.GetString())
                ? null
                : BrowserTypeExtension.FromString(reader.GetString()!),

            _ => throw new JsonException($"Cannot convert {reader.TokenType} to TlsClientIdentifier")
        };
    }

    /// <summary>
    /// Write <see cref="BrowserType"/> to JSON string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, BrowserType? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.GetValue());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}