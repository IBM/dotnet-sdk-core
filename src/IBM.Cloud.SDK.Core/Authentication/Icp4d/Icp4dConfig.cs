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

using IBM.Cloud.SDK.Core.Util;
using System;

namespace IBM.Cloud.SDK.Core.Authentication.Icp4d
{
    /// <summary>
    /// Options for authenticating via the ICP4D (IBM Cloud Private for Data) token service.
    /// This authentication mechanism allows an ICP4D access token to be obtained using a
    /// user-supplied username, password and URL.
    /// The ICP4D access token is then specified as a Bearer Token within outgoing REST API requests.
    /// </summary>
    public class Icp4dConfig : IAuthenticatorConfig
    {
        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string UserManagedAccessToken { get; private set; }
        public bool? DisableSslVerification { get; private set; }

        public string AuthenticationType
        {
            get { return Authenticator.AuthtypeIcp4d; }
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Url))
            {
                throw new ArgumentException("You must provide a URL.");
            }

            // If the user specifies their own access token, then username/password are not required.
            if (string.IsNullOrEmpty(UserManagedAccessToken))
            {
                return;
            }

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                throw new ArgumentException("You must provide both username and password values.");
            }

            if (Utility.HasBadFirstOrLastCharacter(Username) || Utility.HasBadFirstOrLastCharacter(Password))
            {
                throw new ArgumentException("The username and password shouldn't start or end with curly brackets orquotes. Please remove any surrounding {, }, or \" characters.");
            }
        }

        public Icp4dConfig(string url, string username = null, string password = null, string userManagedAccessToken = null, bool? disableSslVerification = null)
        {
            Url = url;

            if (!string.IsNullOrEmpty(username))
            {
                Username = username;
            }
            if (!string.IsNullOrEmpty(password))
            {
                Password = password;
            }
            if (!string.IsNullOrEmpty(userManagedAccessToken))
            {
                UserManagedAccessToken = userManagedAccessToken;
            }
            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }

            Validate();
        }
    }
}
