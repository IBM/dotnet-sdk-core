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


using IBM.Cloud.SDK.Core.Authentication.BasicAuth;
using IBM.Cloud.SDK.Core.Authentication.Icp4d;
using IBM.Cloud.SDK.Core.Authentication;
using System;

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class AuthenticatorFactory
    {

        public static IAuthenticator GetAuthenticator(IAuthenticatorConfig config)
        {
            //  Validate the configuration passed in.
            config.Validate();

            switch (config.AuthenticationType)
            {
                case Authenticator.AuthtypeIam:
                    return new IamTokenManager((IamTokenOptions)config);
                case Authenticator.AuthtypeIcp4d:
                    return new Icp4dAuthenticator((Icp4dConfig)config);
                case Authenticator.AuthtypeBasic:
                    return new BasicAuthenticator((BasicAuthConfig)config);
                case Authenticator.AuthtypeNoauth:
                    return new NoauthAuthenticator((NoauthConfig)config);
                default:
                    throw new ArgumentException(string.Format("Unrecognized AuthenticatorConfig type: {0}", config.GetClass().GetName()));
            }
        }
    }
}
