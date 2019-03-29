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
using IBM.Cloud.SDK.Core.Http;
using NSubstitute;
using System;
using System.Reflection;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class TokenManagerTests
    {
        [TestMethod]
        public void ConstructorIamAccessToken()
        {
            TokenOptions options = new TokenOptions()
            {
                IamAccessToken = "bogus-access-token"
            };
            TokenManager tokenManager = new TokenManager(options);

            string accessToken = tokenManager.GetToken();

            Assert.IsNotNull(tokenManager);
            Assert.IsNotNull(accessToken);
        }

        [TestMethod]
        public void ConstructorIamApiKey()
        {
            TokenOptions options = new TokenOptions()
            {
                IamApiKey = "bogus-iam-apikey",
                IamUrl = "http://bogus-iam-url.com",
                ServiceUrl = "http://bogus-service-url.com"
            };
            TokenManager tokenManager = new TokenManager(options);

            Assert.IsNotNull(tokenManager);
            Assert.IsNotNull(options.ServiceUrl);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ConstructorIamApiKeyFail()
        {
            TokenOptions options = new TokenOptions()
            {
                IamApiKey = "{bogus-iam-apikey",
                IamUrl = "{http://bogus-iam-url.com",
                ServiceUrl = "{http://bogus-service-url.com"
            };
            TokenManager tokenManager = new TokenManager(options);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ConstructorServiceUrlFail()
        {
            TokenOptions options = new TokenOptions()
            {
                ServiceUrl = "{http://bogus-service-url.com"
            };
            TokenManager tokenManager = new TokenManager(options);
        }

        [TestMethod]
        public void SetAccessToken()
        {
            TokenOptions options = new TokenOptions();
            TokenManager tokenManager = new TokenManager(options);
            tokenManager.SetAccessToken("bogus-access-token");
            Assert.IsNotNull(tokenManager);
        }

        [TestMethod]
        public void RequestToken()
        {
            TokenOptions options = new TokenOptions()
            {
                IamApiKey = "bogus-iam-apikey"
            };

            TokenManager tokenManager = new TokenManager(options);
            tokenManager.Client = CreateClient();
            string token = tokenManager.GetToken();

            Assert.IsNotNull(tokenManager);
        }

        [TestMethod]
        public void TimeToLive()
        {
            TokenOptions options = new TokenOptions()
            {
                IamApiKey = "bogus-iam-apikey"
            };

            TokenManager tokenManager = new TokenManager(options);
            MethodInfo methodInfo = typeof(TokenManager).GetMethod("IsTokenExpired", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { };
            bool isTokenExpired = (bool)methodInfo.Invoke(tokenManager, parameters);

            Assert.IsTrue(isTokenExpired);
        }

        #region Create Client
        private IClient CreateClient()
        {
            IClient client = Substitute.For<IClient>();

            client.WithAuthentication(Arg.Any<string>(), Arg.Any<string>())
                .Returns(client);

            return client;
        }
        #endregion
    }
}
