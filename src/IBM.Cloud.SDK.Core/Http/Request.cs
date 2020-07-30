/**
* Copyright 2017 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IBM.Cloud.SDK.Core.Http.Extensions;
using IBM.Cloud.SDK.Core.Http.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBM.Cloud.SDK.Core.Http
{
    public sealed class Request : IRequest
    {
        private readonly IHttpFilter[] filters;
        private readonly Lazy<Task<HttpResponseMessage>> dispatch;

        public Request(HttpRequestMessage message, MediaTypeFormatterCollection formatters, Func<IRequest, Task<HttpResponseMessage>> dispatcher, IHttpFilter[] filters)
        {
            this.Message = message;
            this.Formatters = formatters;
            this.dispatch = new Lazy<Task<HttpResponseMessage>>(() => dispatcher(this));
            this.filters = filters;
            this.Message.Headers.Accept.Clear();
        }

        public HttpRequestMessage Message { get; }

        public MediaTypeFormatterCollection Formatters { get; }

        public IRequest WithBody<T>(T body, MediaTypeHeaderValue contentType = null)
        {
            MediaTypeFormatter formatter = HttpFactory.GetFormatter(this.Formatters, contentType);
            string mediaType = contentType != null ? contentType.MediaType : null;
            return this.WithBody(body, formatter, mediaType);
        }

        public IRequest WithBody<T>(T body, MediaTypeFormatter formatter, string mediaType = null)
        {
            return this.WithBodyContent(new ObjectContent<T>(body, formatter, mediaType));
        }

        public IRequest WithBodyContent(HttpContent body)
        {
            this.Message.Content = body;
            return this;
        }

        public IRequest WithHeader(string key, string value)
        {
            this.Message.Headers.TryAddWithoutValidation(key, value);
            return this;
        }

        public IRequest WithHeaders(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                this.Message.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }

            return this;
        }

        public IRequest WithArgument(string key, object value)
        {
            this.Message.RequestUri = this.Message.RequestUri.WithArguments(new KeyValuePair<string, object>(key, value));
            return this;
        }

        public IRequest WithArguments(object arguments)
        {
            this.Message.RequestUri = this.Message.RequestUri.WithArguments(this.GetArguments(arguments).ToArray());
            return this;
        }

        public IRequest WithCustom(Action<HttpRequestMessage> request)
        {
            request(this.Message);
            return this;
        }

        public IRequest WithFormatter(MediaTypeHeaderValue contentType)
        {
            this.Formatters.JsonFormatter.SupportedMediaTypes.Add(contentType);
            return this;
        }

        public TaskAwaiter<IResponse> GetAwaiter()
        {
            Func<Task<IResponse>> waiter = async () =>
            {
                await this.AsMessage();
                return this;
            };
            return waiter().GetAwaiter();
        }

        public async Task<HttpResponseMessage> AsMessage()
        {
            return await this.GetResponse(this.dispatch.Value).ConfigureAwait(false);
        }

        public async Task<DetailedResponse<T>> As<T>()
        {
            HttpResponseMessage message = await this.AsMessage().ConfigureAwait(false);
            DetailedResponse<T> detailedResponse = new DetailedResponse<T>();

            var result = message.Content.ReadAsStringAsync().Result;

            // Set response headers
            foreach (var header in message.Headers)
            {
                detailedResponse.Headers.Add(header.Key, string.Join(",", header.Value));
            }

            // Set staus code
            detailedResponse.StatusCode = (long)message.StatusCode;

            // Set response
            if (!string.IsNullOrEmpty(result))
            {
                detailedResponse.Response = JValue.Parse(result).ToString(Formatting.Indented);
            }

            // Set result
            detailedResponse.Result = await message.Content.ReadAsAsync<T>(this.Formatters).ConfigureAwait(false);

            return detailedResponse;
        }

        public async Task<byte[]> AsByteArray()
        {
            HttpResponseMessage message = await this.AsMessage().ConfigureAwait(false);
            return await message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        public async Task<string> AsString()
        {
            HttpResponseMessage message = await this.AsMessage().ConfigureAwait(false);
            return await message.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<Stream> AsStream()
        {
            HttpResponseMessage message = await this.AsMessage().ConfigureAwait(false);
            Stream stream = await message.Content.ReadAsStreamAsync().ConfigureAwait(false);
            stream.Position = 0;
            return stream;
        }

        private async Task<HttpResponseMessage> GetResponse(Task<HttpResponseMessage> request)
        {
            foreach (IHttpFilter filter in this.filters)
            {
                filter.OnRequest(this, this.Message);
            }

            HttpResponseMessage response = await request.ConfigureAwait(false);
            foreach (IHttpFilter filter in this.filters)
            {
                filter.OnResponse(this, response);
            }

            return response;
        }

        private IDictionary<string, object> GetArguments(object arguments)
        {
            // null
            if (arguments == null)
            {
                return new Dictionary<string, object>();
            }

            // generic dictionary
            if (arguments is IDictionary<string, object>)
            {
                return (IDictionary<string, object>)arguments;
            }

            // dictionary
            if (arguments is IDictionary)
            {
                IDictionary<string, object> dict = new Dictionary<string, object>();
                IDictionary argDict = (IDictionary)arguments;
                foreach (var key in argDict.Keys)
                {
                    dict.Add(key.ToString(), argDict[key]);
                }

                return dict;
            }

            // object
            return arguments.GetType()
                .GetRuntimeProperties()
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(arguments));
        }
    }
}