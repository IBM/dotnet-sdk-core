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

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class Icp4dTokenManager : JwtTokenManager
    {
        public IClient Client { get; set; }

        private string username;
        private string password;

        public Icp4dTokenManager(Icp4dTokenOptions options) : base(options)
        {
            tokenName = "accessToken";

            if (!string.IsNullOrEmpty(url))
            {
                url = url + "/v1/preauth/validateAuth";
            }
            else if (string.IsNullOrEmpty(userAccessToken))
            {
                // url is not needed if the user specifies their own access token
                throw new Exception("`url` is a required parameter for Icp4dTokenManagerV1");
            }

            // username and password are required too, unless there's access token
            if (!string.IsNullOrEmpty(options.Username))
            {
                username = options.Username;
            }
            if (!string.IsNullOrEmpty(options.Password))
            {
                password = options.Password;
            }

            disableSslVerification = options.DisableSslVerification;

            Client = new IBMHttpClient(url);
        }

        override protected DetailedResponse<TokenData> RequestToken()
        {
            DetailedResponse<TokenData> result = null;

            try
            {
                if (string.IsNullOrEmpty(username))
                    throw new ArgumentNullException("Username is required to request a token");
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException("Password is required to request a token");

                IClient client = Client.WithAuthentication(username, password);
                var request = Client.GetAsync(url);
                client.DisableSslVerification(disableSslVerification);

                result = request.As<TokenData>().Result;
                if (result == null)
                {
                    result = new DetailedResponse<TokenData>();
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }

            return result;
        }
    }

    public class Icp4dTokenOptions : TokenOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool DisableSslVerification { get; set; }
    }
}
