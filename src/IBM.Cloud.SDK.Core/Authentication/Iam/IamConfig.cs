﻿/**
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

using IBM.Cloud.SDK.Core.Util;
using System;

namespace IBM.Cloud.SDK.Core.Authentication.Iam
{
    /// <summary>
    /// Options for authenticating via the IAM (Identity Access Management) token service.
    /// This authentication mechanism allows an IAM access token to be obtained using a
    /// user-supplied apikey.
    /// The IAM access token is then specified as a Bearer Token within outgoing REST API requests.
    /// </summary>
    public class IamConfig : IAuthenticatorConfig
    {
        public string Apikey { get; private set; }
        public string IamUrl { get; private set; }
        public string UserManagedAccessToken { get; private set; }
        public bool? DisableSslVerification { get; private set; }
        public string IamClientId { get; private set; }
        public string IamClientSecret { get; private set; }
        private readonly string defaultUrl = "https://iam.cloud.ibm.com/identity/token";

        public string AuthenticationType
        {
            get { return Authenticator.AuthtypeIam; }
        }

        public void Validate()
        {
            // If the user specifies their own access token, then apikey is not required.
            if (!string.IsNullOrEmpty(UserManagedAccessToken))
            {
                return;
            }

            if (string.IsNullOrEmpty(Apikey))
            {
                throw new ArgumentException("You must provide apikey.");
            }

            if (Utility.HasBadFirstOrLastCharacter(Apikey))
            {
                throw new ArgumentException("The apikey shouldn't start or end with curly brackets or quotes. Please remove any surrounding {, }, or \" characters.");
            }

            if (Utility.HasBadFirstOrLastCharacter(IamUrl))
            {
                throw new ArgumentException("The Url shouldn't start or end with curly brackets or quotes. Please remove any surrounding {, }, or \" characters.");
            }

            if (Utility.OnlyOne(IamClientId, IamClientSecret))
            {
                throw new ArgumentException("Warning: Client ID and Secret must BOTH be given, or the defaults will be used.");
            }
        }

        public IamConfig(string apikey = null, string iamUrl = null, string userManagedAccessToken = null, bool? disableSslVerification = null, string iamClientId = null, string iamClientSecret = null)
        {
            if (!string.IsNullOrEmpty(apikey))
            {
                Apikey = apikey;
            }
            if (!string.IsNullOrEmpty(iamUrl))
            {
                IamUrl = iamUrl;
            }
            else
            {
                IamUrl = defaultUrl;
            }
            if (!string.IsNullOrEmpty(UserManagedAccessToken))
            {
                UserManagedAccessToken = userManagedAccessToken;
            }
            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }
            if (!string.IsNullOrEmpty(iamClientId))
            {
                IamClientId = iamClientId;
            }
            if (!string.IsNullOrEmpty(iamClientSecret))
            {
                IamClientSecret = iamClientSecret;
            }
        }
    }
}