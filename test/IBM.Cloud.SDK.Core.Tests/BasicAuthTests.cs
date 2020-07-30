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
using System.Text;
using IBM.Cloud.SDK.Core.Authentication;
using IBM.Cloud.SDK.Core.Authentication.BasicAuth;
using IBM.Cloud.SDK.Core.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBM.Cloud.SDK.Core.Tests.Authentication.BasicAuth
{
    [TestClass]
    public class BasicAuthTests
    {
        [TestMethod]
        public void TestConstructionUsernamePassword()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBasic);
            Assert.IsTrue(authenticator.Username == "username");
            Assert.IsTrue(authenticator.Password == "password");
        }

        [TestMethod]
        public void TestConstructionDictionary()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add(Authenticator.PropNameUsername, "username");
            config.Add(Authenticator.PropNamePassword, "password");

            BasicAuthenticator authenticator = new BasicAuthenticator(config);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBasic);
            Assert.IsTrue(authenticator.Username == "username");
            Assert.IsTrue(authenticator.Password == "password");
        }

        [TestMethod]
        public void TestAuthenticate()
        {
            var username = "username";
            var password = "password";
            var auth = string.Format("{0}:{1}", username, password);
            var auth64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));

            BasicAuthenticator authenticator = new BasicAuthenticator(username, password);
            IClient client = new IBMHttpClient();
            authenticator.Authenticate(client);

            Assert.IsNotNull(client);
            Assert.IsNotNull(client.BaseClient);
            Assert.IsNotNull(client.BaseClient.DefaultRequestHeaders.Authorization);
            Assert.IsTrue(client.BaseClient.DefaultRequestHeaders.Authorization.ToString() == "Basic " + auth64);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullUsername()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator(null, "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPassword()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBracketBeginning()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("{username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBracketBeginning()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "{pasword");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBracketEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username}", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBracketEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "pasword}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBeginningBracketEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("{username}", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBeginningBracketEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "{pasword}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameQuoteBeginning()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("\"username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordQuoteBeginning()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "\"pasword");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameQuoteEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username\"", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordQuoteEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "pasword\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBeginningQuoteEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("\"username\"", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBeginningQuoteEnd()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "\"pasword\"");
        }
    }
}
