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
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using IBM.Cloud.SDK.Core.Http.Filters;

namespace IBM.Cloud.SDK.Core.Http
{
    public class IBMHttpClient : IClient
    {
        private string errorMessageDisableSsl = "The connection failed because the SSL certificate is not valid. To use a self-signed certificate, set the `disableSslVerification` option in your authentication configuration and/or setting `DisableSslVerification(true);` on your service.";
        private bool isDisposed;
        private bool insecure = false;
        private string serviceUrl = default(string);

        public IBMHttpClient()
        {
            Filters = new List<IHttpFilter> { new ErrorFilter() };
            Formatters = new MediaTypeFormatterCollection();
            CreateClient();
        }

        ~IBMHttpClient()
        {
            Dispose(false);
        }

        public List<IHttpFilter> Filters { get; private set; }

        public HttpClient BaseClient { get; set; }

        public MediaTypeFormatterCollection Formatters { get; protected set; }

        public bool Insecure
        {
            get
            {
                return insecure;
            }

            set
            {
                insecure = value;
                CreateClient();
            }
        }

        public string ServiceUrl
        {
            get
            {
                return serviceUrl;
            }

            set
            {
                serviceUrl = value;
                CreateClient();
            }
        }

        public IClient WithAuthentication(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                string auth = string.Format("{0}:{1}", userName, password);
                string auth64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));

                BaseClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth64);
            }

            return this;
        }

        public IClient WithAuthentication(string bearerToken)
        {
            if (!string.IsNullOrEmpty(bearerToken))
            {
                BaseClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            return this;
        }

        public IRequest DeleteAsync(string resource)
        {
            return SendAsync(HttpMethod.Delete, resource);
        }

        public IRequest GetAsync(string resource)
        {
            return SendAsync(HttpMethod.Get, resource);
        }

        public IRequest PostAsync(string resource)
        {
            return SendAsync(HttpMethod.Post, resource);
        }

        public IRequest PostAsync<TBody>(string resource, TBody body)
        {
            return PostAsync(resource).WithBody(body);
        }

        public IRequest PutAsync(string resource)
        {
            return SendAsync(HttpMethod.Put, resource);
        }

        public IRequest PutAsync<TBody>(string resource, TBody body)
        {
            return PutAsync(resource).WithBody(body);
        }

        public virtual IRequest SendAsync(HttpMethod method, string resource)
        {
            AssertNotDisposed();

            if (string.IsNullOrEmpty(BaseClient.BaseAddress?.AbsoluteUri))
            {
                throw new ArgumentNullException("A service url is required");
            }

            Uri uri = new Uri(BaseClient.BaseAddress, resource);
            HttpRequestMessage message = HttpFactory.GetRequestMessage(method, uri, Formatters);
            return SendAsync(message);
        }

        public virtual IRequest SendAsync(HttpRequestMessage message)
        {
            AssertNotDisposed();
            return new Request(message, Formatters, request => BaseClient.SendAsync(request.Message), Filters.ToArray());
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DisableSslVerification(bool insecure)
        {
            Insecure = insecure;
        }

        protected void AssertNotDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(IBMHttpClient));
            }
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                BaseClient.Dispose();
            }

            isDisposed = true;
        }

        private void CreateClient()
        {
            if (Insecure)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                BaseClient = new HttpClient(httpClientHandler);
            }
            else
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (errors == default(System.Net.Security.SslPolicyErrors))
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Write(errorMessageDisableSsl);
                        return false;
                    }
                };
                BaseClient = new HttpClient(httpClientHandler);
            }

            if (!string.IsNullOrEmpty(ServiceUrl))
            {
                BaseClient.BaseAddress = new Uri(ServiceUrl);
            }
        }
    }
}
