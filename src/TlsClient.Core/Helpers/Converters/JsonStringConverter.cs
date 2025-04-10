using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Helpers.Converters
{
    public class JsonStringConverter<T> : JsonConverter<T> where T : class
    {
        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return this.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }
}
