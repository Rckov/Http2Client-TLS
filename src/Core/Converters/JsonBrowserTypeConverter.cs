using Http2Client.Core.Enums;
using Http2Client.Extensions;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Converters;

/// <summary>
/// JSON converter for BrowserType enum.
/// </summary>
internal class JsonBrowserTypeConverter : JsonConverter<BrowserType>
{
    /// <summary>
    /// Reads BrowserType from JSON.
    /// </summary>
    public override BrowserType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.String => string.IsNullOrEmpty(reader.GetString())
                ? default
                : BrowserTypeExtension.FromString(reader.GetString()),

            _ => throw new JsonException($"Cannot convert {reader.TokenType} to TlsClientIdentifier")
        };
    }

    /// <summary>
    /// Writes BrowserType to JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, BrowserType value, JsonSerializerOptions options)
    {
        if (value != default)
        {
            writer.WriteStringValue(value.GetValue());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}