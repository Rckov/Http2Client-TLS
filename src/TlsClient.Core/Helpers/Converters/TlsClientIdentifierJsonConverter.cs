using System;
using Newtonsoft.Json;
using TlsClient.Core.Models;

public class TlsClientIdentifierJsonConverter : JsonConverter<TlsClientIdentifier>
{
    public override void WriteJson(JsonWriter writer, TlsClientIdentifier? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(value.ToString());
    }

    public override TlsClientIdentifier? ReadJson(JsonReader reader, Type objectType, TlsClientIdentifier? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
       return this.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
    }

    public override bool CanRead => true;
}
