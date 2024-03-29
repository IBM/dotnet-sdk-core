﻿/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace IBM.Cloud.SDK.Core.Authentication.Cp4d
{
    /// <summary>
    /// This class implements support for the CP4D authentication mechanism.
    /// </summary>
    public class CloudPakForDataAuthenticator : Authenticator
    {
        public IClient Client { get; set; }

        // This is the suffix we'll need to add to the user-supplied URL to retrieve an access token.
        private static string UrlSuffix = "/v1/authorize";

        // These are keys for body request for cpd authorization token
        private const string KeyUsername = "username";
        private const string KeyPassword = "password";
        private const string KeyApikey = "api_key";

        // Configuration properties for this authenticator.
        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Apikey { get; private set; }
        public bool? DisableSslVerification { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        // This field holds an access token and its expiration time.
        private CloudPakForDataToken tokenData;

        // this empty constructor will be used by builder 
        public CloudPakForDataAuthenticator()
        {
        }

        /// <summary>
        /// Constructs a CloudPakForDataAuthenticator with all properties.
        /// </summary>
        /// <param name="url">The base URL associated with the token server. The path "/v1/authorize" will be appended to this value automatically.</param>
        /// <param name="username">The username to be used when retrieving the access token</param>
        /// <param name="password">The password to be used when retrieving the access token</param>
        /// <param name="disableSslVerification">A flag indicating whether SSL hostname verification should be disabled</param>
        /// <param name="headers">A set of user-supplied headers to be included in token server interactions</param>
        public CloudPakForDataAuthenticator(string url, string username, string password, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Init(url, username, password, disableSslVerification, headers);
        }

        /// <summary>
        /// Construct a CloudPakForDataAuthenticator instance using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">A map containing the configuration properties</param>
        public CloudPakForDataAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameUrl, out string url);
            config.TryGetValue(PropNameUsername, out string username);
            config.TryGetValue(PropNamePassword, out string password);
            config.TryGetValue(PropNameApikey, out string apikey);
            config.TryGetValue(PropNameDisableSslVerification, out string disableSslVerficiationString);
            bool.TryParse(disableSslVerficiationString, out bool disableSslVerification);
            Init(url, username, password, apikey, disableSslVerification);
        }

        public CloudPakForDataAuthenticator WithUrl(string url)
        {
            Url = url;
            return this;
        }

        public CloudPakForDataAuthenticator WithUserName(string username)
        {
            Username = username;
            return this;
        }

        public CloudPakForDataAuthenticator WithPassword(string password)
        {
            Password = password;
            return this;
        }

        public CloudPakForDataAuthenticator WithApikey(string apikey)
        {
            Apikey = apikey;
            return this;
        }

        public CloudPakForDataAuthenticator WithDisableSslVerification(bool disableSslVerification)
        {
            DisableSslVerification = disableSslVerification;
            return this;
        }

        public CloudPakForDataAuthenticator WithHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;
            return this;
        }

        public CloudPakForDataAuthenticator Build()
        {
            Init();

            return this;
        }

        private void Init(string url, string username, string password, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Url = url;
            Username = username;
            Password = password;
            
            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }

            if (headers != null)
            {
                Headers = headers;
            }

            Init();
        }

        private void Init(string url, string username, string password = null, string apikey = null, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Url = url;
            Username = username;

            if (password != null)
            {
                Password = password;
            }

            if (apikey != null)
            {
                Apikey = apikey;
            }

            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }

            if (headers != null)
            {
                Headers = headers;
            }

            Init();
        }

        public void Init()
        {
            Validate();

            Client = new IBMHttpClient()
            {
                ServiceUrl = Url + UrlSuffix
            };
        }

        public override string AuthenticationType
        {
            get { return AuthTypeCp4d; }
        }

        public override void Authenticate(IClient client)
        {
            client.WithAuthentication(GetToken());
        }

        /// <summary>
        /// This function returns an access token. The source of the token is determined by the following logic:
        /// 1. If user provides their own managed access token, assume it is valid and send it
        /// 2. If this class is managing tokens and does not yet have one, or the token is expired, make a request
        /// for one
        /// 3. If this class is managing tokens and has a valid token stored, send it
        /// </summary>
        /// <returns>the valid access token</returns>
        protected string GetToken()
        {
            string token;

            // Request a new token if necessary.
            if (tokenData == null || !tokenData.IsTokenValid())
            {
                tokenData = new CloudPakForDataToken(RequestToken().Result);
            }

            // Return the access token from our CP4DToken object.
            token = tokenData.AccessToken;

            return token;
        }

        protected DetailedResponse<CloudPakForDataTokenResponse> RequestToken()
        {
            DetailedResponse<CloudPakForDataTokenResponse> result = null;

            try
            {
                var request = Client.PostAsync(Url + UrlSuffix);
                request.WithHeader("Content-type", "Content-Type: application/json");

                if (DisableSslVerification != null)
                {
                    Client.DisableSslVerification((bool)DisableSslVerification);
                }

                if (Headers != null)
                {
                    request.WithHeaders(Headers);
                }

                JObject bodyObject = new JObject();
                bodyObject[KeyUsername] = Username;

                if (string.IsNullOrEmpty(Password))
                {
                    bodyObject[KeyApikey] = Apikey;
                }
                else
                {
                    bodyObject[KeyPassword] = Password;
                }

                var httpContent = new StringContent(JsonConvert.SerializeObject(bodyObject), Encoding.UTF8, HttpMediaType.APPLICATION_JSON);
                request.WithBodyContent(httpContent);

                result = request.As<CloudPakForDataTokenResponse>().Result;
                if (result == null)
                {
                    result = new DetailedResponse<CloudPakForDataTokenResponse>();
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }

            return result;
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Url))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Url"));
            }

            if (string.IsNullOrEmpty(Username))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Username"));
            }

            if (string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(Apikey))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Password or Apikey"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Url))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Url"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Username))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Username"));
            }

            if (!string.IsNullOrEmpty(Password) && CredentialUtils.HasBadStartOrEndChar(Password))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Password"));
            }

            if (!string.IsNullOrEmpty(Apikey) && CredentialUtils.HasBadStartOrEndChar(Apikey))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Apikey"));
            }
        }
    }
}
