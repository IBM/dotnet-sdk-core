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

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class Authenticator : IAuthenticator
    {
        /// <summary>
        /// These are the valid authentication types.
        /// </summary>
        public const string AuthTypeBasic = "basic";
        public const string AuthTypeNoAuth = "noAuth";
        public const string AuthTypeIam = "iam";
        public const string AuthTypeCp4d = "cp4d";
        public const string AuthTypeBearer = "bearerToken";

        /// <summary>
        /// Constants which define the names of external config propreties (credential file, environment variable, etc.).
        /// </summary>
        public static string PropNameAuthType = "AUTH_TYPE";
        public static string PropNameUsername = "USERNAME";
        public static string PropNamePassword = "PASSWORD";
        public static string PropNameBearerToken = "BEARER_TOKEN";
        public static string PropNameUrl = "AUTH_URL";
        public static string PropNameDisableSslVerification = "AUTH_DISABLE_SSL";
        public static string PropNameApikey = "APIKEY";
        public static string PropNameClientId = "CLIENT_ID";
        public static string PropNameClientSecret = "CLIENT_SECRET";

        public static string ErrorMessagePropMissing = "The {0} property is required but was not specified.";
        public static string ErrorMessagePropInvalid = "The {0} property is invalid. Please remove any surrounding {{, }}, or \" characters.";
        public static string ErrorMessageReqFailed = "Error while fetching access token from token service: ";

        /// <summary>
        /// Returns the authentication type associated with the Authenticator instance.
        /// </summary>
        public virtual string AuthenticationType { get; }

        /// <summary>
        /// Perform the necessary authentication steps for the specified request.
        /// </summary>
        public virtual void Authenticate(IClient client) { }

        /// <summary>
        /// Validates the current set of configuration information in the Authenticator.
        /// </summary>
        public virtual void Validate() { }
    }
}
