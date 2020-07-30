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

using System;
using System.Collections.Generic;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;

namespace IBM.Cloud.SDK.Core.Authentication.Cp4d
{
    /// <summary>
    /// This class implements support for the CP4D authentication mechanism.
    /// </summary>
    public class CloudPakForDataAuthenticator : Authenticator
    {
        // This is the suffix we'll need to add to the user-supplied URL to retrieve an access token.
        private static string urlSuffix = "/v1/preauth/validateAuth";

        // This field holds an access token and its expiration time.
        private CloudPakForDataToken tokenData;

        /// <summary>
        /// Constructs a CloudPakForDataAuthenticator with all properties.
        /// </summary>
        /// <param name="url">The base URL associated with the token server. The path "/v1/preauth/validateAuth" will be appended to this value automatically.</param>
        /// <param name="username">The username to be used when retrieving the access token.</param>
        /// <param name="password">The password to be used when retrieving the access token.</param>
        /// <param name="disableSslVerification">A flag indicating whether SSL hostname verification should be disabled.</param>
        /// <param name="headers">A set of user-supplied headers to be included in token server interactions.</param>
        public CloudPakForDataAuthenticator(string url, string username, string password, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Init(url, username, password, disableSslVerification, headers);
        }

        /// <summary>
        /// Construct a CloudPakForDataAuthenticator instance using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">A map containing the configuration properties.</param>
        public CloudPakForDataAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameUrl, out string url);
            config.TryGetValue(PropNameUsername, out string username);
            config.TryGetValue(PropNamePassword, out string password);
            config.TryGetValue(PropNameDisableSslVerification, out string disableSslVerficiationString);
            bool.TryParse(disableSslVerficiationString, out bool disableSslVerification);
            Init(url, username, password, disableSslVerification);
        }

        public IClient Client { get; set; }

        // Configuration properties for this authenticator.
        public string Url { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public bool? DisableSslVerification { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public override string AuthenticationType
        {
            get { return AuthTypeCp4d; }
        }

        public override void Authenticate(IClient client)
        {
            client.WithAuthentication(GetToken());
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

            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Password"));
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

        /// <summary>
        /// This function returns an access token. The source of the token is determined by the following logic:
        /// 1. If user provides their own managed access token, assume it is valid and send it
        /// 2. If this class is managing tokens and does not yet have one, or the token is expired, make a request
        /// for one
        /// 3. If this class is managing tokens and has a valid token stored, send it.
        /// </summary>
        /// <returns>the valid access token.</returns>
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
                IClient client = Client.WithAuthentication(Username, Password);
                var request = Client.GetAsync(Url + urlSuffix);
                if (DisableSslVerification != null)
                {
                    Client.DisableSslVerification((bool)DisableSslVerification);
                }

                if (Headers != null)
                {
                    request.WithHeaders(Headers);
                }

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

            Validate();

            Client = new IBMHttpClient()
            {
                ServiceUrl = Url + urlSuffix,
            };
        }
    }
}
