using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TlsClientMethod
    {
        GET, POST, PUT, DELETE, HEAD, OPTIONS, PATCH
    }
}
