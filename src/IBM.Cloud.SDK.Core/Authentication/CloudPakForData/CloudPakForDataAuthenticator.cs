/**
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
using System;
using System.Collections.Generic;
using System.Net.Http;

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
        private const string UsernameKey = "username";
        private const string PasswordKey = "password";
        private const string ApikeyKey = "api_key";

        // Configuration properties for this authenticator.
        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Apikey { get; private set; }
        public bool? DisableSslVerification { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        // This field holds an access token and its expiration time.
        private CloudPakForDataToken tokenData;

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
            Init(url, username, password, null, disableSslVerification, headers);
        }

        /// <summary>
        /// Constructs a CloudPakForDataAuthenticator with all properties.
        /// </summary>
        /// <param name="url">The base URL associated with the token server. The path "/v1/authorize" will be appended to this value automatically.</param>
        /// <param name="username">The username to be used when retrieving the access token</param>
        /// <param name="password">The password to be used when retrieving the access token</param>
        /// <param name="apikey">The apikey to be used when retrieving the access token</param>
        /// <param name="disableSslVerification">A flag indicating whether SSL hostname verification should be disabled</param>
        /// <param name="headers">A set of user-supplied headers to be included in token server interactions</param>
        public CloudPakForDataAuthenticator(string url, string username, string? password, string? apikey, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Init(url, username, password, apikey, disableSslVerification, headers);
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

        public CloudPakForDataAuthenticator SetUrl(string url)
        {
            this.Url = url;
            return this;
        }

        public CloudPakForDataAuthenticator SetUsername(string username)
        {
            this.Username = username;
            return this;
        }

        public CloudPakForDataAuthenticator setPassword(string password)
        {
            this.Password = this.Password;
            return this;
        }

        public CloudPakForDataAuthenticator setApikey(string apikey)
        {
            this.Apikey = this.Apikey;
            return this;
        }

        public CloudPakForDataAuthenticator SetDisableSslVerification(bool disableSslVerification)
        {
            this.DisableSslVerification = disableSslVerification;
            return this;
        }

        public CloudPakForDataAuthenticator SetHeaders(Dictionary<string, string> headers)
        {
            this.Headers = headers;
            return this;
        }

        public CloudPakForDataAuthenticator Build()
        {
            Init();

            return this;
        }

        private void Init(string url, string username, string? password, string? apikey, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Url = url;
            Username = username;
            Password = password;
            Apikey = apikey;

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
                request.WithHeader("Content-type", "application/x-www-form-urlencoded");

                if (DisableSslVerification != null)
                {
                    Client.DisableSslVerification((bool)DisableSslVerification);
                }

                if (Headers != null)
                {
                    request.WithHeaders(Headers);
                }

                List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> username = new KeyValuePair<string, string>(UsernameKey, Username);
                content.Add(username);

                if (string.IsNullOrEmpty(Password))
                {
                    KeyValuePair<string, string> apikey = new KeyValuePair<string, string>(ApikeyKey, Apikey);
                    content.Add(apikey);
                } else
                {
                    KeyValuePair<string, string> password = new KeyValuePair<string, string>(PasswordKey, Password);
                    content.Add(password);
                }

                var formData = new FormUrlEncodedContent(content);
                request.WithBodyContent(formData);

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

            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Apikey))
            {
                throw new ArgumentNullException("The Password or Apikey property is required but was not specified.");
            }

            if (CredentialUtils.HasBadStartOrEndChar(Url))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Url"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Username))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Username"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Password))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Password"));
            }
        }
    }
}
