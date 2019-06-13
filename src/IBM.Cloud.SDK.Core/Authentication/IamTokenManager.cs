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
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Cloud.SDK.Core.Authentication
{
    public class IamTokenManager : JwtTokenManager
    {
        public IClient Client { get; set; }

        private string iamApikey;
        private string iamClientId;
        private string iamClientSecret;

        public IamTokenManager(TokenOptions options) : base(options)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (!string.IsNullOrEmpty(options.IamUrl))
                {
                    url = options.IamUrl;
                }
                else
                {
                    url = "https://iam.cloud.ibm.com/identity/token";
                }
            }

            if(!string.IsNullOrEmpty(options.IamApiKey))
            {
                iamApikey = options.IamApiKey;
            }

            if (!string.IsNullOrEmpty(options.IamAccessToken))
            {
                userAccessToken = options.IamAccessToken;
            }

            if(!string.IsNullOrEmpty(options.IamClientId))
            {
                iamClientId = options.IamClientId;
            }

            if (!string.IsNullOrEmpty(options.IamClientSecret))
            {
                iamClientSecret = options.IamClientSecret;
            }
        }
    }
}
