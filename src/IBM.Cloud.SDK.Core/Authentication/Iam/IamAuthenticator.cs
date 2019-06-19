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
            Client = new IBMHttpClient(config.Url);
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
                    tokenData = RequestToken().Result;
                }

                // Return the access token from our ICP4DToken object.
                token = tokenData.AccessToken;
            }

            return token;
        }

        protected DetailedResponse<IamToken> RequestToken()
        {
            DetailedResponse<IamToken> result = null;

            try
            {
                if (string.IsNullOrEmpty(config.Apikey))
                    throw new ArgumentNullException("Apikey is required to request a token");

                IClient client = Client.WithAuthentication(config.Apikey);
                var request = Client.GetAsync(config.Url);
                client.DisableSslVerification((bool)config.DisableSslVerification);

                result = request.As<IamToken>().Result;
                if (result == null)
                {
                    result = new DetailedResponse<IamToken>();
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
