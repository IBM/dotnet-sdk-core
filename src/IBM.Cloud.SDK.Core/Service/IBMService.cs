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
using IBM.Cloud.SDK.Core.Authentication.BasicAuth;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Cloud.SDK.Core.Authentication.Icp4d;
using IBM.Cloud.SDK.Core.Authentication.Noauth;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;

namespace IBM.Cloud.SDK.Core.Service
{
    public abstract class IBMService : IIBMService
    {
        //const string PATH_AUTHORIZATION_V1_TOKEN = "/authorization/api/v1/token";
        private const string icpPrefix = "icp-";
        private const string apikeyAsUsername = "apikey";
        public string SERVICE_NAME;
        public string ApiKey { get; set; }
        public IClient Client { get; set; }
        private bool skipAuthentication = false;
        public bool SkipAuthentication
        {
            get { return skipAuthentication; }
            set
            {
                skipAuthentication = value;
                authenticator = new NoauthAuthenticator(null);
            }
        }
        public string DefaultEndpoint { get; }
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

        public string UserName { get; set; }
        public string Password { get; set; }
        public string IamAccessToken { get; set; }
        public string IamUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Icp4dAccessToken { get; set; }
        public string Icp4dUrl { get; set; }

        private Authenticator authenticator;

        protected bool _userSetEndpoint = false;
        
        protected IBMService(string serviceName)
        {
            Client = new IBMHttpClient();
            ServiceName = serviceName;

            Utility.LoadExternalCredentials();
            string apiKey = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_IAM_APIKEY");
            // check for old IAM API key name as well
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_APIKEY");
            }

            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiKey = apiKey;
            }

            string username = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_USERNAME");
            if (!string.IsNullOrEmpty(username))
            {
                UserName = username;
            }

            string password = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_PASSWORD");
            if (!string.IsNullOrEmpty(password))
            {
                Password = password;
            }

            string ServiceUrl = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_URL");

            string iamAccessToken = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_IAM_ACCESS_TOKEN");
            if (!string.IsNullOrEmpty(iamAccessToken))
            {
                IamAccessToken = iamAccessToken;
            }

            string iamUrl = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_IAM_URL");
            if (!string.IsNullOrEmpty(iamUrl))
            {
                IamUrl = iamUrl;
            }

            string clientId = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_IAM_CLIENT_ID");
            if (!string.IsNullOrEmpty(clientId))
            {
                ClientId = clientId;
            }

            string clientSecret = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_IAM_CLIENT_SECRET");
            if (!string.IsNullOrEmpty(clientSecret))
            {
                ClientSecret = clientSecret;
            }

            string icp4dAccessToken = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_ICP4D_ACCESS_TOKEN");
            if (!string.IsNullOrEmpty(icp4dAccessToken))
            {
                Icp4dAccessToken = icp4dAccessToken;
            }

            string icp4dUrl = Environment.GetEnvironmentVariable(ServiceName.ToUpper() + "_ICP4D_URL");
            if (!string.IsNullOrEmpty(icp4dUrl))
            {
                Icp4dUrl = icp4dUrl;
            }

            if (string.IsNullOrEmpty(IamAccessToken) &&
                string.IsNullOrEmpty(Icp4dAccessToken) &&
                string.IsNullOrEmpty(ApiKey) &&
                (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                )
            {
                throw new NullReferenceException(string.Format("Either {0}_IAM_ACCESS_TOKEN, {0}_ICP4D_ACCESS_TOKEN, {0}_APIKEY or {0}_USERNAME and {0}_PASSWORD did not exist. Please add credentials with this key in ibm-credentials.env.", ServiceName.ToUpper()));
            }

            Icp4dConfig icp4dConfig = new Icp4dConfig(
                url: !string.IsNullOrEmpty(Icp4dUrl) ? Icp4dUrl : null,
                username: !string.IsNullOrEmpty(UserName) ? UserName : null,
                password: !string.IsNullOrEmpty(Password) ? Password : null,
                userManagedAccessToken: !string.IsNullOrEmpty(Icp4dAccessToken) ? Icp4dAccessToken : null
                );

            IamConfig iamConfig = new IamConfig(
                apikey: !string.IsNullOrEmpty(ApiKey) ? ApiKey : null,
                iamUrl: !string.IsNullOrEmpty(IamUrl) ? IamUrl : null,
                userManagedAccessToken: !string.IsNullOrEmpty(IamAccessToken) ? IamAccessToken : null,
                iamClientId: !string.IsNullOrEmpty(ClientId) ? ClientId : null,
                iamClientSecret: !string.IsNullOrEmpty(ClientSecret) ? ClientSecret : null
                );

            BasicAuthConfig basicAuthConfig = new BasicAuthConfig(
                username: !string.IsNullOrEmpty(UserName) ? UserName : null,
                password: !string.IsNullOrEmpty(Password) ? Password : null
                );

            if (!string.IsNullOrEmpty(ServiceUrl))
                Endpoint = ServiceUrl;

            if (!string.IsNullOrEmpty(Icp4dAccessToken) && !string.IsNullOrEmpty(Icp4dUrl))
            {
                SetAuthenticator(icp4dConfig);
            }

            if (!string.IsNullOrEmpty(ApiKey))
            {
                SetAuthenticator(iamConfig);
            }

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                SetAuthenticator(basicAuthConfig);
            }
        }

        protected IBMService(string serviceName, string url)
        {
            ServiceName = serviceName;
            Client = new IBMHttpClient(url, UserName, Password);

            if (!string.IsNullOrEmpty(Endpoint))
                Endpoint = url;
            //TODO: verificar como iremos obter de um arquivo json por injeção de dependencia
            //ApiKey = CredentialUtils.GetApiKey(serviceName);
            //Endpoint = CredentialUtils.GetApiUrl(serviceName);
        }

        protected IBMService(string serviceName, string url, IClient httpClient)
        {
            ServiceName = serviceName;
            Client = httpClient;

            if (!string.IsNullOrEmpty(Endpoint))
                Endpoint = url;

            //TODO: verificar como iremos obter de um arquivo json por injeção de dependencia
            //ApiKey = CredentialUtils.GetApiKey(serviceName);
            //Endpoint = CredentialUtils.GetApiUrl(serviceName);
        }

        protected IBMService(string serviceName, IAuthenticatorConfig authenticatorConfig)
        {
            Client = new IBMHttpClient();
            ServiceName = serviceName;
            SetAuthenticator(authenticatorConfig);
        }

        /// <summary>
        /// Sets the username and password credentials.
        /// </summary>
        /// <param name="userName">The username</param>
        /// <param name="password">The password</param>
        public void SetCredential(string userName, string password)
        {
            if (userName == apikeyAsUsername && !password.StartsWith(icpPrefix))
            {
                IamConfig iamConfig = new IamConfig(password);
                SetAuthenticator(iamConfig);
            }
            else
            {
                BasicAuthConfig basicAuthConfig = new BasicAuthConfig(userName, password);
                SetAuthenticator(basicAuthConfig);
            }
        }

        /// <summary>
        /// Sets the tokenOptions for the service. 
        /// Also sets the endpoint if the user has not set the endpoint.
        /// </summary>
        /// <param name="options"></param>
        public void SetCredential(TokenOptions options)
        {
            if (!string.IsNullOrEmpty(options.ServiceUrl))
            {
                if (!_userSetEndpoint)
                {
                    Endpoint = options.ServiceUrl;
                }
            }
            else
            {
                options.ServiceUrl = DefaultEndpoint;
            }

            if (!string.IsNullOrEmpty(options.IamApiKey))
            {
                if (options.IamApiKey.StartsWith(icpPrefix))
                {
                    SetCredential(apikeyAsUsername, options.IamApiKey);
                }
                else
                {
                    IamConfig iamConfig = new IamConfig(options.IamApiKey);
                    SetAuthenticator(iamConfig);
                    SetAuthentication();
                }
            }
            else if (!string.IsNullOrEmpty(options.IamAccessToken))
            {
                IamConfig iamConfig = new IamConfig(userManagedAccessToken: options.IamAccessToken);
                SetAuthenticator(iamConfig);
                SetAuthentication();
            }
            else
            {
                throw new ArgumentNullException("An iamApikey or iamAccessToken is required.");
            }
        }
        
        /// <summary>
        /// Initializes a new Authenticator instance based on the input AuthenticatorConfig instance and sets it as
        /// the current authenticator on the BaseService instance.
        /// </summary>
        /// <param name="authConfig">the AuthenticatorConfig instance containing the authentication configuration</param>
        protected void SetAuthenticator(IAuthenticatorConfig authConfig)
        {
            try
            {
                authenticator = AuthenticatorFactory.GetAuthenticator(authConfig);
                if (authenticator is NoauthAuthenticator)
                {
                    SkipAuthentication = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error: {0}", e));
            }

        }

        protected void SetAuthentication()
        {
            if (SkipAuthentication)
            {
                return;
            }

            if(authenticator != null)
            {
                if(authenticator.AuthenticationType == Authenticator.AuthtypeIcp4d)
                {
                    DisableSslVerification(((authenticator as Icp4dAuthenticator).Client as IBMHttpClient).Insecure);
                }
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
    }
}
