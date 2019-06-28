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
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IBM.Cloud.SDK.Core.Authentication.Iam
{
    /// <summary>
    /// This class implements support for the IAM authentication mechanism.
    /// </summary>
    public class IamAuthenticator : Authenticator
    {
        public IClient Client { get; set; }

        // Configuration properties for this authenticator.
        private IamConfig config;

        // This field holds an access token and its expiration time.
        private IamToken tokenData;

        public IamAuthenticator(IamConfig config)
        {
            this.config = config;
            Client = new IBMHttpClient(config.IamUrl);
        }

        public override string AuthenticationType
        {
            get { return AuthtypeIam; }
        }

        public override void Authenticate(IClient client)
        {
            client.WithAuthentication(GetToken());
        }

        protected string GetToken()
        {
            string token;

            if (!string.IsNullOrEmpty(config.UserManagedAccessToken))
            {
                // If the user set their own access token, then use it.
                token = config.UserManagedAccessToken;
            }
            else
            {
                // Request a new token if necessary.
                if (tokenData == null || !tokenData.IsTokenValid())
                {
                    tokenData = new IamToken(RequestToken().Result);
                }

                // Return the access token from our IamToken object.
                token = tokenData.AccessToken;
            }

            return token;
        }

        protected DetailedResponse<IamTokenResponse> RequestToken()
        {
            DetailedResponse<IamTokenResponse> result = null;

            // Use bx:bx as default auth header creds.
            var clientId = "bx";
            var clientSecret = "bx";

            // If both the clientId and secret were specified by the user, then use them.
            if (!string.IsNullOrEmpty(config.IamClientId) && !string.IsNullOrEmpty(config.IamClientSecret))
            {
                clientId = config.IamClientId;
                clientSecret = config.IamClientSecret;
            }

            try
            {
                if (string.IsNullOrEmpty(config.Apikey))
                    throw new ArgumentNullException("Apikey is required to request a token");

                IClient client = Client.WithAuthentication(clientId, clientSecret);
                var request = Client.PostAsync(config.IamUrl);
                request.WithHeader("Content-type", "application/x-www-form-urlencoded");

                if (config.DisableSslVerification != null)
                    client.DisableSslVerification((bool)config.DisableSslVerification);

                List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> grantType = new KeyValuePair<string, string>("grant_type", "urn:ibm:params:oauth:grant-type:apikey");
                KeyValuePair<string, string> responseType = new KeyValuePair<string, string>("response_type", "cloud_iam");
                KeyValuePair<string, string> apikey = new KeyValuePair<string, string>("apikey", config.Apikey);
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
    }
}
