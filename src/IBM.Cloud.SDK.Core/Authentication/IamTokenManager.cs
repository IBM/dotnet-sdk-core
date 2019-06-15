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
using System.Net.Http;

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class IamTokenManager : JwtTokenManager
    {
        public IClient Client { get; set; }

        private string iamApikey;
        private string iamClientId;
        private string iamClientSecret;
        private string iamDefaultUrl = "https://iam.cloud.ibm.com/identity/token";

        private const string CLIENT_ID_SECRET_WARNING = "Warning: Client ID and Secret must BOTH be given, or the defaults will be used.";

        public IamTokenManager(IamTokenOptions options) : base(options)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (!string.IsNullOrEmpty(options.IamUrl))
                {
                    url = options.IamUrl;
                }
                else
                {
                    url = iamDefaultUrl;
                }
            }

            if (!string.IsNullOrEmpty(options.IamApiKey))
            {
                iamApikey = options.IamApiKey;
            }

            if (!string.IsNullOrEmpty(options.IamAccessToken))
            {
                userAccessToken = options.IamAccessToken;
            }

            if (!string.IsNullOrEmpty(options.IamClientId))
            {
                iamClientId = options.IamClientId;
            }

            if (!string.IsNullOrEmpty(options.IamClientSecret))
            {
                iamClientSecret = options.IamClientSecret;
            }

            if (OnlyOne(options.IamClientId, options.IamClientSecret))
            {
                Console.WriteLine(CLIENT_ID_SECRET_WARNING);
            }
        }

        public void SetIamAuthorizationInfo(string iamClientId, string iamClientSecret)
        {
            this.iamClientId = iamClientId;
            this.iamClientSecret = iamClientSecret;

            if (OnlyOne(iamClientId, iamClientSecret))
            {
                Console.WriteLine(CLIENT_ID_SECRET_WARNING);
            }
        }

        override protected DetailedResponse<TokenData> RequestToken()
        {
            DetailedResponse<TokenData> result = null;

            // Use bx:bx as default auth header creds.
            var clientId = "bx";
            var clientSecret = "bx";

            // If both the clientId and secret were specified by the user, then use them.
            if (!string.IsNullOrEmpty(iamClientId) && !string.IsNullOrEmpty(iamClientSecret))
            {
                clientId = iamClientId;
                clientSecret = iamClientSecret;
            }

            try
            {
                if (string.IsNullOrEmpty(iamApikey))
                    throw new ArgumentNullException("iamApikey is required to request a token");

                IClient client = Client.WithAuthentication(clientId, clientSecret);
                var request = Client.PostAsync(url);
                request.WithHeader("Content-type", "application/x-www-form-urlencoded");

                var formData = new MultipartFormDataContent();
                var grantTypeContent = new StringContent("urn:ibm:params:oauth:grant-type:apikey");
                formData.Add(grantTypeContent, "grant_type");
                var apiKeyContent = new StringContent(iamApikey);
                formData.Add(apiKeyContent, "apikey");
                var responseTypeContent = new StringContent("cloud_iam");
                formData.Add(responseTypeContent, "response_type");

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

        private bool OnlyOne(string a, string b)
        {
            return (string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b)) || (!string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b));
        }
    }

    public class IamTokenOptions : JwtTokenOptions
    {
        private string iamUrl;
        public string IamUrl
        {
            get { return iamUrl; }
            set
            {
                if (!Utility.HasBadFirstOrLastCharacter(value))
                {
                    iamUrl = value;
                }
                else
                {
                    throw new ArgumentException("The IamUrl shouldn't start or end with curly brackets or quotes. Be sure to remove any {} and \" characters surrounding your IamUrl");
                }
            }
        }

        private string iamApiKey;
        public string IamApiKey
        {
            get { return iamApiKey; }
            set
            {
                if (!Utility.HasBadFirstOrLastCharacter(value))
                {
                    iamApiKey = value;
                }
                else
                {
                    throw new ArgumentException("The IamApiKey shouldn't start or end with curly brackets or quotes. Be sure to remove any {} and \" characters surrounding your IamApiKey");
                }
            }
        }

        public string IamAccessToken { get; set; }
        public string IamClientId { get; set; }
        public string IamClientSecret { get; set; }
    }
}
