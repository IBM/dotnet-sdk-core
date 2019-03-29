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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Cloud.SDK.Core.Util;
using System;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class TokenManagerIntegrationTests
    {
        [TestMethod]
        public void GetIamToken()
        {
            var credentialsPaths = Utility.GetCredentialsPaths();
            foreach (string path in credentialsPaths)
            {
                if (Utility.LoadEnvFile(path))
                {
                    break;
                }
            }

            string apiKey = Environment.GetEnvironmentVariable("DISCOVERY_APIKEY");
            string serviceUrl = Environment.GetEnvironmentVariable("DISCOVERY_URL");

            TokenOptions options = new TokenOptions()
            {
                IamApiKey = apiKey
            };
            options.ServiceUrl = serviceUrl;

            TokenManager tokenManager = new TokenManager(options);
            
            Assert.IsNotNull(tokenManager);
            Assert.IsNotNull(tokenManager.GetToken());
        }
    }
}
