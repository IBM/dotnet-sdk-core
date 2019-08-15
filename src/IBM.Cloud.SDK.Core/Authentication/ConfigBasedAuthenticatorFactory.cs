/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using IBM.Cloud.SDK.Core.Authentication.BasicAuth;
using IBM.Cloud.SDK.Core.Authentication.Bearer;
using IBM.Cloud.SDK.Core.Authentication.Cp4d;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Cloud.SDK.Core.Authentication.Noauth;
using IBM.Cloud.SDK.Core.Util;
using System.Collections.Generic;

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class ConfigBasedAuthenticatorFactory
    {
        public static Authenticator GetAuthenticator(string serviceName)
        {
            Authenticator authenticator = null;

            // Gather authentication-related properties from all the supported config sources:
            // - 1) Credential file
            // - 2) Environment variables
            // - 3) VCAP_SERVICES env variable
            Dictionary<string, string> authProps = new Dictionary<string, string>();

            // First check to see if this service has any properties defined in a credential file.
            authProps = CredentialUtils.GetFileCredentialsAsMap(serviceName);

            // If we didn't find any properties so far, then try the environment.
            if (authProps == null || authProps.Count == 0)
            {
                authProps = CredentialUtils.GetEnvCredentialsAsMap(serviceName);
            }

            // If we didn't find any properties so far, then try VCAP_SERVICES
            if (authProps == null || authProps.Count == 0)
            {
                authProps = CredentialUtils.GetVcapCredentialsAsMap(serviceName);
            }

            // Now create an authenticator from the map.
            if (authProps != null && authProps.Count > 0)
            {
                authenticator = CreateAuthenticator(authProps);
            }

            return authenticator;
        }

        /**
   * Instantiates an Authenticator that reflects the properties contains in the specified Map.
   * @param props a Map containing configuration properties
   * @return an Authenticator instance
   */


        private static Authenticator CreateAuthenticator(Dictionary<string, string> props)
        {
            Authenticator authenticator = null;

            // If auth type was not specified, we'll use "iam" as the default.
            props.TryGetValue(Authenticator.PropnameAuthType, out string authType);
            if (string.IsNullOrEmpty(authType))
            {
                authType = Authenticator.AuthtypeIam;
            }

            switch (authType)
            {
                case Authenticator.AuthtypeNoauth:
                    authenticator = new NoauthAuthenticator(props);
                    break;

                case Authenticator.AuthtypeBasic:
                    authenticator = new BasicAuthenticator(props);
                    break;

                case Authenticator.AuthtypeIam:
                    authenticator = new IamAuthenticator(props);
                    break;

                case Authenticator.AuthtypeCp4d:
                    authenticator = new CloudPakForDataAuthenticator(props);
                    break;

                case Authenticator.AuthtypeBearer:
                    authenticator = new BearerTokenAuthenticator(props);
                    break;
                default:
                    break;
            }

            return authenticator;
        }
    }
}
