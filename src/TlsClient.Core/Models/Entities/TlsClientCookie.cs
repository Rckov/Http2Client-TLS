using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models.Entities
{
    public class TlsClientCookie
    {
        public long Expires { get; set; }
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
        public long MaxAge { get; set; }

        public TlsClientCookie(string name, string value)
        {
            Name = name;
            Value = value;
            Domain = string.Empty;
            Path = string.Empty;
            Expires = 0;
            MaxAge = 0;
        }
    }
}
