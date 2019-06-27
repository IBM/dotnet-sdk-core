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

namespace IBM.Cloud.SDK.Core.Authentication.Icp4d
{
    /// <summary>
    /// This class implements support for the ICP4D authentication mechanism.
    /// </summary>
    public class Icp4dAuthenticator : Authenticator
    {
        public IClient Client { get; set; }

        // This is the suffix we'll need to add to the user-supplied URL to retrieve an access token.
        private static string UrlSuffix = "/v1/preauth/validateAuth";

        // Configuration properties for this authenticator.
        private Icp4dConfig config;

        // This field holds an access token and its expiration time.
        private Icp4dToken tokenData;

        public Icp4dAuthenticator(Icp4dConfig config)
        {
            this.config = config;
            Client = new IBMHttpClient(config.Url + UrlSuffix, this.config.DisableSslVerification);
        }

        public override string AuthenticationType
        {
            get { return AuthtypeIcp4d; }
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
                    tokenData = new Icp4dToken(RequestToken().Result);
                }

                // Return the access token from our ICP4DToken object.
                token = tokenData.AccessToken;
            }

            return token;
        }

        protected DetailedResponse<Icp4dTokenResponse> RequestToken()
        {
            DetailedResponse<Icp4dTokenResponse> result = null;

            try
            {
                if (string.IsNullOrEmpty(config.Username))
                    throw new ArgumentNullException("Username is required to request a token");
                if (string.IsNullOrEmpty(config.Password))
                    throw new ArgumentNullException("Password is required to request a token");

                IClient client = Client.WithAuthentication(config.Username, config.Password);
                var request = Client.GetAsync(config.Url + UrlSuffix);
                if (config.DisableSslVerification != null)
                    Client.DisableSslVerification((bool)config.DisableSslVerification);

                result = request.As<Icp4dTokenResponse>().Result;
                if (result == null)
                {
                    result = new DetailedResponse<Icp4dTokenResponse>();
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
