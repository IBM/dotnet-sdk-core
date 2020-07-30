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

using System;
using System.Collections.Generic;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Util;

namespace IBM.Cloud.SDK.Core.Authentication.BasicAuth
{
    /// <summary>
    /// This class implements support for Basic Authentication. The main purpose of this authenticator is to construct the
    /// Authorization header and then add it to each outgoing REST API request.
    /// </summary>
    public class BasicAuthenticator : Authenticator
    {
        /// <summary>
        /// Construct a BasicAuthenticator instance with the specified username and password.
        /// These values are used to construct an Authorization header value that will be included
        /// in outgoing REST API requests.
        /// </summary>
        /// <param name="username">The basic auth username.</param>
        /// <param name="password">The basic auth password.</param>
        public BasicAuthenticator(string username, string password)
        {
            Init(username, password);
        }

        /// <summary>
        /// Construct a BasicAuthenticator using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">A map containing the username and password values.</param>
        public BasicAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameUsername, out string username);
            config.TryGetValue(PropNamePassword, out string password);
            Init(username, password);
        }

        /// <summary>
        /// The username configured on this authenticator.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// The password configured on this authenticator.
        /// </summary>
        public string Password { get; private set; }

        public override string AuthenticationType
        {
            get { return AuthTypeBasic; }
        }

        /// <summary>
        /// This method is called to authenticate an outgoing REST API request.
        /// Here, we'll just set the Authorization header to provide the necessary authentication info.
        /// </summary>
        /// <param name="client"></param>
        public override void Authenticate(IClient client)
        {
            client.WithAuthentication(Username, Password);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Username))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Username"));
            }

            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Password"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Username))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Username"));
            }

            if (CredentialUtils.HasBadStartOrEndChar(Password))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Password"));
            }
        }

        private void Init(string username, string password)
        {
            Username = username;
            Password = password;

            Validate();
        }
    }
}
