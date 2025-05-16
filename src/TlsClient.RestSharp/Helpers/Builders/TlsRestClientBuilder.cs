using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TlsClient.Core.Models.Entities;
using TlsClient.HttpClient;

namespace TlsClient.RestSharp.Helpers.Builders
{
    public class TlsRestClientBuilder
    {
        private Core.TlsClient _tlsClient { get; set; }
        private bool _isUseCookieContainer { get; set; } = false;
        private Uri _baseUrl { get; set; }
        private Action<RestClientOptions> _configureRestClient { get; set; }
        public TlsRestClientBuilder WithTlsClient(Core.TlsClient tlsClient)
        {
            _tlsClient = tlsClient;
            return this;
        }

        public TlsRestClientBuilder WithBaseUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl), "Base URL cannot be null or empty");
            }
            _baseUrl = new Uri(baseUrl);
            return this;
        }
        public TlsRestClientBuilder WithCookieContainer(bool useCookieContainer)
        {
            _isUseCookieContainer = useCookieContainer;
            return this;
        }

        public TlsRestClientBuilder WithConfigureRestClient(Action<RestClientOptions> configureRestClient)
        {
            _configureRestClient = configureRestClient;
            return this;
        }

        public RestClient Build()
        {
            if(_tlsClient == null)
            {
                throw new ArgumentNullException(nameof(_tlsClient), "TlsClient cannot be null");
            }

            var _tlsHandler = new TlsClientHandler(_tlsClient);

            var client = new RestClient(handler:_tlsHandler, configureRestClient: (options) =>
            {
                options.BaseUrl = _baseUrl;
                options.UserAgent = _tlsClient.Options.UserAgent;
                options.Timeout = _tlsClient.Options.Timeout;
                options.FollowRedirects = _tlsClient.Options.FollowRedirects;

                if (_tlsClient.Options.InsecureSkipVerify)
                {
                    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                }
                if (_isUseCookieContainer)
                {
                    options.CookieContainer = new System.Net.CookieContainer();
                }

                if (_configureRestClient != null)
                {
                    _configureRestClient(options);
                }
            });

            return client;
        }
    }
}
