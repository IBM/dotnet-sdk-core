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
using IBM.Cloud.SDK.Core.Authentication.Cp4d;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBM.Cloud.SDK.Core.Tests.Authentication.CloudPak4DataAuth
{
    [TestClass]
    public class CloudPak4DataAuthTests
    {
        [TestMethod]
        public void TestConstructionRequried()
        {
            var url = "http://www.service-endpoint.com";
            var username = "username";
            var password = "password";
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(
                url: url,
                username: username,
                password: password);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeCp4d);
            Assert.IsTrue(authenticator.Url == url);
            Assert.IsTrue(authenticator.Username == username);
            Assert.IsTrue(authenticator.Password == password);
        }

        [TestMethod]
        public void TestConstructionDisableSslVerification()
        {
            var url = "http://www.service-endpoint.com";
            var username = "username";
            var password = "password";
            var disableSslVerification = true;
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(
                url: url,
                username: username,
                password: password,
                disableSslVerification: disableSslVerification);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeCp4d);
            Assert.IsTrue(authenticator.Url == url);
            Assert.IsTrue(authenticator.Username == username);
            Assert.IsTrue(authenticator.Password == password);
            Assert.IsTrue(authenticator.DisableSslVerification == disableSslVerification);
        }

        [TestMethod]
        public void TestConstructorHeaders()
        {
            var url = "http://www.service-endpoint.com";
            var username = "username";
            var password = "password";
            var headerName = "headerName";
            var headervalue = "headerValue";
            var headers = new Dictionary<string, string>();
            headers.Add(headerName, headervalue);

            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(
                url: url,
                username: username,
                password: password,
                headers: headers);

            authenticator.Headers.TryGetValue(headerName, out string retrievedHeaderValue);
            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeCp4d);
            Assert.IsTrue(authenticator.Username == username);
            Assert.IsTrue(authenticator.Password == password);
            Assert.IsTrue(authenticator.Headers.ContainsKey(headerName));
            Assert.IsTrue(authenticator.Headers.ContainsValue(headervalue));
            Assert.IsTrue(retrievedHeaderValue == headervalue);
        }

        [TestMethod]
        public void TestConstructionDictionary()
        {
            var url = "http://www.service-endpoint.com";
            var username = "username";
            var password = "password";
            var disableSslVerification = true;

            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add(Authenticator.PropNameUrl, url);
            config.Add(Authenticator.PropNameUsername, username);
            config.Add(Authenticator.PropNamePassword, password);
            config.Add(Authenticator.PropNameDisableSslVerification, disableSslVerification.ToString());

            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(config);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeCp4d);
            Assert.IsTrue(authenticator.Url == url);
            Assert.IsTrue(authenticator.Username == username);
            Assert.IsTrue(authenticator.Password == password);
            Assert.IsTrue(authenticator.DisableSslVerification == disableSslVerification);
        }

        [TestMethod]
        public void TestConstructionDictionaryMissingProperty()
        {
            var url = "http://www.service-endpoint.com";
            var username = "username";
            var password = "password";

            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add(Authenticator.PropNameUrl, url);
            config.Add(Authenticator.PropNameUsername, username);
            config.Add(Authenticator.PropNamePassword, password);

            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(config);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeCp4d);
            Assert.IsTrue(authenticator.Url == url);
            Assert.IsTrue(authenticator.Username == username);
            Assert.IsTrue(authenticator.Password == password);
            Assert.IsTrue(authenticator.DisableSslVerification == false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullUrl()
        {
            var url = default(string);
            var username = "username";
            var password = "password";
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(url, username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullUsername()
        {
            var url = "http://www.service-endpoint.com";
            var username = default(string);
            var password = "password";
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(url, username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPassword()
        {
            var url = "http://www.service-endpoint.com";
            var username = "username";
            var password = default(string);
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(url, username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBracketBeginning()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("{url", "username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBracketBeginning()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "{username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBracketBeginning()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username", "{pasword");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBracketEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url}", "username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBracketEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username}", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBracketEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username", "pasword}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBeginningBracketEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("{serviceUrl}", "username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBeginningBracketEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "{username}", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBeginningBracketEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username", "{pasword}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlQuoteBeginning()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("\"url", "username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameQuoteBeginning()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "\"username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordQuoteBeginning()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username", "\"pasword");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlQuoteEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url\"", "username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameQuoteEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username\"", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordQuoteEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username", "pasword\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBeginningQuoteEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("\"url\"", "username", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUsernameBeginningQuoteEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "\"username\"", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadPasswordBeginningQuoteEnd()
        {
            CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("url", "username", "\"pasword\"");
        }
    }
}
