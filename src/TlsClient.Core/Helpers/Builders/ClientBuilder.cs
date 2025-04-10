using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Core.Helpers.Builders
{
    public class ClientBuilder
    {
        private TlsClientOptions _options= new TlsClientOptions();

        public ClientBuilder WithIdentifier(TlsClientIdentifier clientIdentifier)
        {
            _options.TlsClientIdentifier = clientIdentifier;
            return this;
        }

        public ClientBuilder WithUserAgent(string userAgent)
        {
            if (_options.DefaultHeaders.ContainsKey("User-Agent"))
                _options.DefaultHeaders["User-Agent"].Add(userAgent);
            else
                _options.DefaultHeaders["User-Agent"] = new List<string> { userAgent };
            return this;
        }

        public ClientBuilder WithProxyUrl(string proxyUrl, bool isRotating = false)
        {
            _options.ProxyURL = proxyUrl;
            _options.IsRotatingProxy = isRotating;
            return this;
        }

        public ClientBuilder WithTimeout(TimeSpan timeout)
        {
            _options.Timeout = timeout;
            return this;
        }

        public ClientBuilder WithDebug(bool enabled = true)
        {
            _options.WithDebug = enabled;
            return this;
        }

        public ClientBuilder WithFollowRedirects(bool enabled = true)
        {
            _options.FollowRedirects = enabled;
            return this;
        }

        public ClientBuilder WithSkipTlsVerification(bool skip = true)
        {
            _options.InsecureSkipVerify = skip;
            return this;
        }

        public ClientBuilder DisableIPV4(bool disabled = true)
        {
            _options.DisableIPV4 = disabled;
            return this;
        }

        public ClientBuilder DisableIPV6(bool disabled = true)
        {
            _options.DisableIPV6 = disabled;
            return this;
        }

        public ClientBuilder WithDefaultCookieJar(bool enabled = true)
        {
            _options.WithDefaultCookieJar = enabled;
            return this;
        }

        public ClientBuilder WithoutCookieJar(bool enabled = true)
        {
            _options.WithoutCookieJar = enabled;
            return this;
        }

        public ClientBuilder AddHeader(string key, string value)
        {
            if (_options.DefaultHeaders.ContainsKey(key))
                _options.DefaultHeaders[key].Add(value);
            else
                _options.DefaultHeaders[key] = new List<string> { value };

            return this;
        }

        public TlsClient Build()
        {
            return new TlsClient(_options);
        }
    }
}
