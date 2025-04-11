using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Helpers
{
    public static class RequestHelpers
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        private static string ConvertJson(object data) => JsonConvert.SerializeObject(data, _jsonSettings);
        private static byte[] GetBytes(string data) => Encoding.UTF8.GetBytes(data);
        public static byte[] Prepare(object data) => GetBytes(ConvertJson(data));
        public static string PrepareBody(byte[] data) => Convert.ToBase64String(data);
    }
}
