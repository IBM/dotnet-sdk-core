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
using System.Net.Http;
using System.Net;
using IBM.Cloud.SDK.Core.Authentication;
using IBM.Cloud.SDK.Core.Authentication.NoAuth;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;
using IBM.Cloud.SDK.Core.Model;

namespace IBM.Cloud.SDK.Core.Service
{
    public abstract class IBMService : IIBMService
    {
        public static string PropNameServiceUrl = "URL";
        public static string PropNameServiceDisableSslVerification = "DISABLE_SSL";

        private const string icpPrefix = "icp-";
        private const string apikeyAsUsername = "apikey";
        public IClient Client { get; set; }
        public string ServiceName { get; set; }
        public string ServiceUrl { get { return Endpoint; } }
        protected Dictionary<string, string> customRequestHeaders = new Dictionary<string, string>();
        private const string ErrorMessageNoAuthenticator = "Authentication information was not properly configured.";

        protected string Endpoint
        {
            get
            {
                if (Client.BaseClient == null ||
                    Client.BaseClient.BaseAddress == null)
                    return string.Empty;

                //  remove trailing `/` if it exists
                return Client.BaseClient.BaseAddress.AbsoluteUri.TrimEnd('/');
            }
            set
            {
                if (Client.BaseClient == null)
                {
                    Client.BaseClient = new HttpClient();
                }
                Client.ServiceUrl = value;
            }
        }

        private IAuthenticator authenticator;

        protected IBMService(string serviceName, IClient httpClient)
        {
            ServiceName = serviceName;
            Client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            authenticator = new NoAuthAuthenticator();
        }

        protected IBMService(string serviceName, IAuthenticator authenticator, WebProxy webProxy = null)
        {
            ServiceName = serviceName;

            this.authenticator = authenticator ?? throw new ArgumentNullException(ErrorMessageNoAuthenticator);

            Client = new IBMHttpClient(webProxy);

            // Try to retrieve the service URL from either a credential file, environment, or VCAP_SERVICES.
            Dictionary<string, string> props = CredentialUtils.GetServiceProperties(serviceName);
            props.TryGetValue(PropNameServiceUrl, out string serviceUrl);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                SetServiceUrl(serviceUrl);
            }
        }

        public void ConfigureClient(HttpConfigOptions httpConfigOptions)
        {
            string CurrentServiceUrl = ServiceUrl;

            if (httpConfigOptions.Proxy != null)
            {
                Client = new IBMHttpClient(httpConfigOptions.Proxy);
            }

            if (httpConfigOptions.DisableSslVerification)
            {
                Client.DisableSslVerification(httpConfigOptions.DisableSslVerification);
            }

            SetServiceUrl(CurrentServiceUrl);
        }

        protected void SetAuthentication()
        {
            if (authenticator != null)
            {
                authenticator.Authenticate(Client);
            }
            else
            {
                throw new ArgumentException("Authentication information was not properly configured.");
            }
        }

        public void SetServiceUrl(string serviceUrl)
        {
            Endpoint = serviceUrl;
        }

        public void WithHeader(string name, string value)
        {
            if (!customRequestHeaders.ContainsKey(name))
            {
                customRequestHeaders.Add(name, value);
            }
            else
            {
                customRequestHeaders[name] = value;
            }
        }

        public void WithHeaders(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                if (!customRequestHeaders.ContainsKey(kvp.Key))
                {
                    customRequestHeaders.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    customRequestHeaders[kvp.Key] = kvp.Value;
                }
            }
        }

        protected void ClearCustomRequestHeaders()
        {
            customRequestHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns a Dictionary of custom request headers.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCustomRequestHeaders()
        {
            return customRequestHeaders;
        }

        /// <summary>
        /// Returns the authenticator for the service.
        /// </summary>
        public IAuthenticator GetAuthenticator()
        {
            return authenticator;
        }
    }
}
