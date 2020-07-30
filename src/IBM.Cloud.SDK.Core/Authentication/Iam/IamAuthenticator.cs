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
using System.Net.Http;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;

namespace IBM.Cloud.SDK.Core.Authentication.Iam
{
    /// <summary>
    /// This class implements support for the IAM authentication mechanism.
    /// </summary>
    public class IamAuthenticator : Authenticator
    {
        private const string GrantType = "grant_type";
        private const string RequestGrantType = "urn:ibm:params:oauth:grant-type:apikey";
        private const string ApikeyConst = "apikey";
        private const string ResponseType = "response_type";
        private const string CloudIam = "cloud_iam";
        private readonly string defaultUrl = "https://iam.cloud.ibm.com/identity/token";

        // This field holds an access token and its expiration time.
        private IamToken tokenData;

        /// <summary>
        /// Constructs an IamAuthenticator with all properties.
        /// </summary>
        /// <param name="apikey">The apikey to be used when retrieving the access token.</param>
        /// <param name="url">The URL representing the token server endpoint.</param>
        /// <param name="clientId">The clientId to be used in token server interactions.</param>
        /// <param name="clientSecret">The clientSecret to be used in token server interactions.</param>
        /// <param name="disableSslVerification">A flag indicating whether SSL hostname verification should be disabled.</param>
        /// <param name="headers">A set of user-supplied headers to be included in token server interactions.</param>
        public IamAuthenticator(string apikey, string url = null, string clientId = null, string clientSecret = null, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Init(apikey, url, clientId, clientSecret, disableSslVerification, headers);
        }

        /// <summary>
        /// Construct an IamAuthenticator instance using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">A map containing the configuration properties.</param>
        public IamAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameUrl, out string url);
            config.TryGetValue(PropNameApikey, out string apikey);
            config.TryGetValue(PropNameClientId, out string clientId);
            config.TryGetValue(PropNameClientSecret, out string clientSecret);
            config.TryGetValue(PropNameDisableSslVerification, out string disableSslVerficiationString);
            bool.TryParse(disableSslVerficiationString, out bool disableSslVerification);
            Init(apikey, url, clientId, clientSecret, disableSslVerification);
        }

        public IClient Client { get; set; }

        // Configuration properties for this authenticator.
        public string Apikey { get; private set; }

        public string Url { get; set; }

        public bool? DisableSslVerification { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public override string AuthenticationType
        {
            get { return AuthTypeIam; }
        }

        public override void Authenticate(IClient client)
        {
            client.WithAuthentication(GetToken());
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Apikey))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "apikey"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Apikey))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "apikey"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Url))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "url"));
            }

            if (Utility.OnlyOne(ClientId, ClientSecret))
            {
                Console.WriteLine("Warning: Client ID and Secret must BOTH be given, or the defaults will be used.");
            }
        }

        protected string GetToken()
        {
            string token;

            // Request a new token if necessary.
            if (tokenData == null || !tokenData.IsTokenValid())
            {
                tokenData = new IamToken(RequestToken().Result);
            }

            // Return the access token from our IamToken object.
            token = tokenData.AccessToken;

            return token;
        }

        protected DetailedResponse<IamTokenResponse> RequestToken()
        {
            DetailedResponse<IamTokenResponse> result = null;

            string clientId = default(string);
            string clientSecret = default(string);

            // If both the clientId and secret were specified by the user, then use them.
            if (!string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret))
            {
                clientId = ClientId;
                clientSecret = ClientSecret;
            }

            try
            {
                if (string.IsNullOrEmpty(Apikey))
                {
                    throw new ArgumentNullException("Apikey is required to request a token");
                }

                IClient client = Client;

                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                {
                    client = Client.WithAuthentication(ClientId, ClientSecret);
                }

                var request = Client.PostAsync(Url);
                request.WithHeader("Content-type", "application/x-www-form-urlencoded");

                if (Headers != null)
                {
                    request.WithHeaders(Headers);
                }

                if (DisableSslVerification != null)
                {
                    client.DisableSslVerification((bool)DisableSslVerification);
                }

                List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> grantType = new KeyValuePair<string, string>(GrantType, RequestGrantType);
                KeyValuePair<string, string> responseType = new KeyValuePair<string, string>(ResponseType, CloudIam);
                KeyValuePair<string, string> apikey = new KeyValuePair<string, string>(ApikeyConst, Apikey);
                content.Add(grantType);
                content.Add(responseType);
                content.Add(apikey);

                var formData = new FormUrlEncodedContent(content);

                request.WithBodyContent(formData);

                result = request.As<IamTokenResponse>().Result;
                if (result == null)
                {
                    result = new DetailedResponse<IamTokenResponse>();
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }

            return result;
        }

        private void Init(string apikey, string url = null, string clientId = null, string clientSecret = null, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Apikey = apikey;

            if (string.IsNullOrEmpty(url))
            {
                url = defaultUrl;
            }

            this.Url = url;
            if (!string.IsNullOrEmpty(clientId))
            {
                ClientId = clientId;
            }

            if (!string.IsNullOrEmpty(clientSecret))
            {
                ClientSecret = clientSecret;
            }

            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }

            if (headers != null)
            {
                this.Headers = headers;
            }

            Validate();

            Client = new IBMHttpClient()
            {
                ServiceUrl = Url,
            };
        }
    }
}
