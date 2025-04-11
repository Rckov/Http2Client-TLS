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

        public RestClient Build()
        {
            if(_tlsClient == null)
            {
                throw new ArgumentNullException(nameof(_tlsClient), "TlsClient cannot be null");
            }

            var _tlsHandler = new TlsClientHandler(_tlsClient);

            var client = new RestClient(_tlsHandler, configureRestClient: (x) =>
            {
                x.BaseUrl = _baseUrl;
                x.UserAgent = _tlsClient.Options.UserAgent;
                x.Timeout = _tlsClient.Options.Timeout;
                x.FollowRedirects = _tlsClient.Options.FollowRedirects;

                if (_tlsClient.Options.InsecureSkipVerify)
                {
                    x.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                }

                if (_isUseCookieContainer)
                {
                    x.CookieContainer = _tlsHandler.CookieContainer;
                }
            });

            return client;
        }
    }
}
