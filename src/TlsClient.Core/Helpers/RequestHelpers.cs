using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Helpers
{
    public static class RequestHelpers
    {
        public static DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        public static byte[] Prepare(object data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data,new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver= contractResolver
            }));
        }
    }
}
