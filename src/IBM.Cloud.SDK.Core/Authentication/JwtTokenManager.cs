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
using System.Text;

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class JwtTokenManager
    {
        protected string url;
        protected string tokenName;
        protected string userAccessToken;
        protected bool rejectUnauthorized;
        private object tokenInfo;
        private long? timeToLive;
        private long? expireTime;

        public JwtTokenManager(TokenOptions options)
        {
            tokenInfo = new object();

            tokenName = "access_token";

            if (!string.IsNullOrEmpty(options.Url))
            {
                this.url = options.Url;
            }

            if (!string.IsNullOrEmpty(options.AccessToken))
            {
                this.userAccessToken = options.AccessToken;
            }
        }

        public string GetToken()
        {
            if(!string.IsNullOrEmpty(userAccessToken))
            {
                return userAccessToken;
            }
            else if(string.IsNullOrEmpty(userAccessToken) || IsTokenExpired())
            {
                return RequestToken();
            }
            else
            {
                return userAccessToken;
            }
        }

        protected string RequestToken()
        {
            throw new 
        }

        private bool IsTokenExpired()
        {
            if (expireTime == null)
            {
                return true;
            }

            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            return expireTime < currentTime;
        }
    }

    public class TokenOptions
    {
        public string AccessToken { get; set; }
        public string Url { get; set; }
    }
}
