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
using IBM.Cloud.SDK.Core.Authentication;
using IBM.Cloud.SDK.Core.Authentication.Cp4d;
using IBM.Cloud.SDK.Core.Authentication.Noauth;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;

namespace IBM.Cloud.SDK.Core.Service
{
    public abstract class IBMService : IIBMService
    {
        private const string icpPrefix = "icp-";
        private const string apikeyAsUsername = "apikey";
        public string serviceName;
        public IClient Client { get; set; }
        public string ServiceName { get; set; }
        public string Url { get { return Endpoint; } }
        protected Dictionary<string, string> customRequestHeaders = new Dictionary<string, string>();

        protected string Endpoint
        {
            get
            {
                if (Client.BaseClient == null ||
                    Client.BaseClient.BaseAddress == null)
                    return string.Empty;

                return Client.BaseClient.BaseAddress.AbsoluteUri;
            }
            set
            {
                if (Client.BaseClient == null)
                {
                    Client.BaseClient = new HttpClient();
                }
                Client.BaseClient.BaseAddress = new Uri(value);
            }
        }

        private Authenticator authenticator;

        protected bool _userSetEndpoint = false;
        
        protected IBMService(string serviceName, string defaultEndpoint)
        {
            ServiceName = serviceName;
            Client = new IBMHttpClient();
            authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceName);
            var creds = CredentialUtils.GetServiceProperties(serviceName);
            creds.TryGetValue("URL", out string url);
            SetEndpoint(string.IsNullOrEmpty(url) ? defaultEndpoint : url);
        }

        protected IBMService(string serviceName, string url, IClient httpClient)
        {
            ServiceName = serviceName;
            Client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            authenticator = new NoauthAuthenticator();

            if (!string.IsNullOrEmpty(Endpoint))
                Endpoint = url;
        }

        protected IBMService(string serviceName, Authenticator authenticator)
        {
            Client = new IBMHttpClient();
            ServiceName = serviceName;
            this.authenticator = authenticator;
        }
        
        /// <summary>
        /// Initializes a new Authenticator instance based on the input AuthenticatorConfig instance and sets it as
        /// the current authenticator on the BaseService instance.
        /// </summary>
        /// <param name="authConfig">the AuthenticatorConfig instance containing the authentication configuration</param>
        protected void SetAuthenticator(Authenticator authenticator)
        {
            try
            {
                authenticator = this.authenticator;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error: {0}", e));
            }

        }

        protected void SetAuthentication()
        {
            if(authenticator != null)
            {
                authenticator.Authenticate(Client);
            }
            else
            {
                throw new ArgumentException("Authentication information was not properly configured.");
            }
        }

        public void SetEndpoint(string url)
        {
            _userSetEndpoint = true;
            Endpoint = url;
        }

        public void DisableSslVerification(bool insecure)
        {
            Client.DisableSslVerification(insecure);
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
        public Authenticator GetAuthenticator()
        {
            return authenticator;
        }
    }
}
