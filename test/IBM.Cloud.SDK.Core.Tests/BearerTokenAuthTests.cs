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

using System;
using System.Collections.Generic;
using IBM.Cloud.SDK.Core.Authentication;
using IBM.Cloud.SDK.Core.Authentication.Bearer;
using IBM.Cloud.SDK.Core.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBM.Cloud.SDK.Core.Tests.Authentication.BearerTokenAuth
{
    [TestClass]
    public class BearerTokenAuthTests
    {
        [TestMethod]
        public void TestConstructionBearerToken()
        {
            var bearerToken = "bearerToken";
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(bearerToken);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBearer);
            Assert.IsTrue(authenticator.BearerToken == bearerToken);
        }

        [TestMethod]
        public void TestUpdateTokenToken()
        {
            var bearerToken0 = "bearerToken0";
            var bearerToken1 = "bearerToken1";
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(bearerToken0);
            authenticator.BearerToken = bearerToken1;

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBearer);
            Assert.IsTrue(authenticator.BearerToken == bearerToken1);
        }

        [TestMethod]
        public void TestConstructionDictionary()
        {
            var bearerToken = "bearerToken";

            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add(Authenticator.PropNameBearerToken, bearerToken);

            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(config);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBearer);
            Assert.IsTrue(authenticator.BearerToken == bearerToken);
        }

        [TestMethod]
        public void TestAuthenticate()
        {
            var bearerToken = "bearerToken";

            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(bearerToken);
            IClient client = new IBMHttpClient();
            authenticator.Authenticate(client);

            Assert.IsNotNull(client);
            Assert.IsNotNull(client.BaseClient);
            Assert.IsNotNull(client.BaseClient.DefaultRequestHeaders.Authorization);
            Assert.IsTrue(client.BaseClient.DefaultRequestHeaders.Authorization.ToString() == "Bearer " + bearerToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullBearerToken()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(bearerToken: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadBearerTokenBracketBeginning()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator("{username");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadBearerTokenBracketEnd()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator("username}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadBearerTokenBeginningBracketEnd()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator("{username}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadBearerTokenQuoteBeginning()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator("\"username");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadBearerTokenQuoteEnd()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator("username\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadBearerTokenBeginningQuoteEnd()
        {
            BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator("\"username\"");
        }
    }
}
