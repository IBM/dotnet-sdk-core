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
using JWT;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
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
        private TokenInfo tokenInfo;
        private long? timeToLive;
        private long? expireTime;

        public JwtTokenManager(TokenOptions options)
        {
            tokenInfo = new TokenInfo();

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
            if (!string.IsNullOrEmpty(userAccessToken))
            {
                return userAccessToken;
            }
            else if (string.IsNullOrEmpty(userAccessToken) || IsTokenExpired())
            {
                return RequestToken();
            }
            else
            {
                return userAccessToken;
            }
        }

        protected virtual DetailedResponse<TokenInfo> RequestToken()
        {
            throw new Exception("`requestToken` MUST be overridden by a subclass of JwtTokenManagerV1.");
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

        private void SaveTokenInfo(TokenInfo tokenResponse)
        {
            var accessToken = tokenResponse.AccessToken;

            if (string.IsNullOrEmpty(userAccessToken))
            {
                throw new Exception("Access token not present in response");
            }

            expireTime = CalculateTimeForNewToken(accessToken);
            tokenInfo = tokenResponse;
        }

        private long CalculateTimeForNewToken(string accessToken)
        {
           int timeForNewToken = 0;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var decodedResponse = decoder.Decode(accessToken);

                if (!string.IsNullOrEmpty(decodedResponse))
                {
                    var o = JObject.Parse(decodedResponse);
                    int exp = (int)o["exp"];
                    int iat = (int)o["iat"];

                    float fractonOfTtl = 0.8f;
                    int timeToLive = exp - iat;
                    //timeForNewToken = exp - (timeToLive * (1.0f - fractonOfTtl));
                }
                else
                {
                    throw new Exception("Access token recieved is not a valid JWT");
                }
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }

            return timeForNewToken;
        }
    }

    public class TokenOptions
    {
        public string AccessToken { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool DisableSslVerification { get; set; }
        public string IamUrl { get; set; }
        public string IamApiKey { get; set; }
        public string IamAccessToken { get; set; }
        public string IamClientId { get; set; }
        public string IamClientSecret { get; set; }
    }

    public class TokenInfo
    {
        public string AccessToken { get; set; }
    }
}
