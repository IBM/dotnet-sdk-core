

using System;
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
namespace IBM.Cloud.SDK.Core.Authentication.Icp4d
{
    /// <summary>
    /// This class holds relevant info re: an ICP4D access token for use by the ICP4DAuthenticator class.
    /// </summary>
    public class Icp4dToken
    {
        public string AccessToken { get; set; }
        public long ExpirationTimeInMillis { get; set; }

        /// <summary>
        /// This ctor is used to store a user-managed access token which will never expire.
        /// </summary>
        /// <param name="accessToken">accessToken the user-managed access token</param>
        public Icp4dToken(string accessToken)
        {
            AccessToken = accessToken;
            ExpirationTimeInMillis = -1;
        }

        public Icp4dToken(Icp4dTokenResponse response)
        {
            AccessToken = response.AccessToken;

            JsonWebToken jwt = new JsonWebToken(AccessToken);

            long? iat = jwt.GetPayload().GetIssuedAt();
            long? exp = jwt.GetPlayload().GetExpiresAt();

            if (iat != null && exp != null)
            {
                long ttl = (long)exp - (long)iat;
                ExpirationTimeInMillis = ((long)iat + (long)(0.8 * ttl)) * 1000;
            }
            else
            {
                throw new Exception("Properties 'iat' and 'exp' MUST be present within the encoded access token");
            }
        }

        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(AccessToken) && (ExpirationTimeInMillis < 0 || DateTimeOffset.Now.ToUnixTimeMilliseconds() <= ExpirationTimeInMillis);
        }
    }
}
