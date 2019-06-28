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

namespace IBM.Cloud.SDK.Core.Authentication.BasicAuth
{
    /// <summary>
    /// This class implements support for Basic Authentication. The main purpose of this authenticator is to construct the
    /// Authorization header and then add it to each outgoing REST API request.
    /// </summary>
    public class BasicAuthenticator : Authenticator
    {
        private string Username;
        private string Password;

        /// <summary>
        /// Initialize our Authorization header value using the information in the BasicAuthConfig instance.
        /// This ctor assumes that the config object has already passed validation.
        /// </summary>
        /// <param name="config">the BasicAuthConfig instance that holds the username and password values from which to build the
        /// Authorization header.</param>
        public BasicAuthenticator(BasicAuthConfig config)
        {
            Username = config.Username;
            Password = config.Password;
        }

        public override string AuthenticationType
        {
            get { return AuthtypeBasic; }
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
    }
}
