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
        public const string AuthtypeBasic = "basic";
        public const string AuthtypeNoauth = "noauth";
        public const string AuthtypeIam = "iam";
        public const string AuthtypeCp4d = "cp4d";
        public const string AuthtypeBearer = "bearerToken";

        /// <summary>
        /// Constants which define the names of external config propreties (credential file, environment variable, etc.).
        /// </summary>
        public static string PropnameAuthType = "AUTH_TYPE";
        public static string PropnameUsername = "USERNAME";
        public static string PropnamePassword = "PASSWORD";
        public static string PropnameBearerToken = "BEARER_TOKEN";
        public static string PropnameUrl = "AUTH_URL";
        public static string PropnameDisableSsl = "AUTH_DISABLE_SSL";
        public static string PropnameApikey = "APIKEY";
        public static string PropnameClientId = "CLIENT_ID";
        public static string PropnameClientSecret = "CLIENT_SECRET";

        public static string ErrormsgPropMissing = "The {0} property is required but was not specified.";
        public static string ErrormsgPropInvalid = "The {0} property is invalid. Please remove any surrounding {{, }}, or \" characters.";
        public static string ErrormsgReqFailed = "Error while fetching access token from token service: ";

        /// <summary>
        /// Returns the authentication type associated with the Authenticator instance.
        /// </summary>
        virtual public string AuthenticationType { get; }

        /// <summary>
        /// Perform the necessary authentication steps for the specified request.
        /// </summary>
        virtual public void Authenticate(IClient client) { }

        /// <summary>
        /// Validates the current set of configuration information in the Authenticator.
        /// </summary>
        virtual public void Validate() { }
    }
}
