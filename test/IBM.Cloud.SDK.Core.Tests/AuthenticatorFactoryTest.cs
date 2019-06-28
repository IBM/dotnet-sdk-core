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


using IBM.Cloud.SDK.Core.Authentication;
using IBM.Cloud.SDK.Core.Authentication.BasicAuth;
using IBM.Cloud.SDK.Core.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class AuthenticatorFactoryTest
    {
        public void TestGetAuthenticatorBasicAuth()
        {
            IClient client = CreateClient();
            BasicAuthConfig config = new BasicAuthConfig("username", "password");

            Authenticator authenticator = AuthenticatorFactory.GetAuthenticator(config);
            authenticator.Authenticate(client);

            Assert.IsNotNull(client.BaseClient.DefaultRequestHeaders.Authorization);
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
