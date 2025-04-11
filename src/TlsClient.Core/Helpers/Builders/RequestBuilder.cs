using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Core.Helpers.Builders
{
    public class RequestBuilder
    {
        private readonly Request _request = new Request();

        public RequestBuilder WithUrl(string url)
        {
            _request.RequestUrl = url;
            return this;
        }

        public RequestBuilder WithMethod(HttpMethod method)
        {
            _request.RequestMethod = method;
            return this;
        }

        public RequestBuilder WithHeader(string key, string value)
        {
            _request.Headers[key] = value;
            return this;
        }

        public RequestBuilder WithHeaders(Dictionary<string, string> headers)
        {
            foreach (var kvp in headers)
            {
                _request.Headers[kvp.Key] = kvp.Value;
            }
            return this;
        }

        public RequestBuilder WithBody(string body)
        {
            _request.RequestBody = body;
            return this;
        }

        public RequestBuilder WithBody(byte[] bytes)
        {
            _request.IsByteRequest = true;
            _request.RequestBody = RequestHelpers.PrepareBody(bytes);
            return this;
        }

        public RequestBuilder WithBody(object data)
        {
            _request.RequestBody = JsonConvert.SerializeObject(data);
            return this;
        }

        public RequestBuilder WithByteRequest()
        {
            _request.IsByteRequest = true;
            return this;
        }

        public RequestBuilder WithByteResponse()
        {
            _request.IsByteResponse = true;
            return this;
        }

        public Request Build()
        {
            return _request;
        }
    }
}
