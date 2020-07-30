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
using IBM.Cloud.SDK.Core.Authentication.Iam;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBM.Cloud.SDK.Core.Tests.Authentication.IamAuth
{
    [TestClass]
    public class IamAuthTests
    {
        [TestMethod]
        public void TestConstructionApikey()
        {
            var apikey = "apikey";
            IamAuthenticator authenticator = new IamAuthenticator(apikey);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
        }

        [TestMethod]
        public void TestConstructorUrl()
        {
            var apikey = "apikey";
            var url = "http://service-endpoint.com";

            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: apikey,
                url: url);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
            Assert.IsTrue(authenticator.Url == url);
        }

        [TestMethod]
        public void TestConstructorClientIdAndSecret()
        {
            var apikey = "apikey";
            var clientId = "clientId";
            var clientSecret = "clientSecret";

            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: apikey,
                clientId: clientId,
                clientSecret: clientSecret);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
            Assert.IsTrue(authenticator.ClientId == clientId);
            Assert.IsTrue(authenticator.ClientSecret == clientSecret);
        }

        [TestMethod]
        public void TestConstructorDisableSslVerification()
        {
            var apikey = "apikey";
            var disableSslVerification = true;

            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: apikey,
                disableSslVerification: disableSslVerification);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
            Assert.IsTrue(authenticator.DisableSslVerification == disableSslVerification);
        }

        [TestMethod]
        public void TestConstructorHeaders()
        {
            var apikey = "apikey";
            var headerName = "headerName";
            var headervalue = "headerValue";
            var headers = new Dictionary<string, string>();
            headers.Add(headerName, headervalue);

            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: apikey,
                headers: headers);

            authenticator.Headers.TryGetValue(headerName, out string retrievedHeaderValue);
            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
            Assert.IsTrue(authenticator.Headers.ContainsKey(headerName));
            Assert.IsTrue(authenticator.Headers.ContainsValue(headervalue));
            Assert.IsTrue(retrievedHeaderValue == headervalue);
        }

        [TestMethod]
        public void TestConstructionDictionary()
        {
            var apikey = "apikey";
            var url = "http://www.service-endpoint.com";
            var clientId = "clientId";
            var clientSecret = "clientSecret";
            var disableSsl = true;

            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add(Authenticator.PropNameApikey, apikey);
            config.Add(Authenticator.PropNameUrl, url);
            config.Add(Authenticator.PropNameClientId, clientId);
            config.Add(Authenticator.PropNameClientSecret, clientSecret);
            config.Add(Authenticator.PropNameDisableSslVerification, disableSsl.ToString());

            IamAuthenticator authenticator = new IamAuthenticator(config);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
            Assert.IsTrue(authenticator.Url == url);
            Assert.IsTrue(authenticator.ClientId == clientId);
            Assert.IsTrue(authenticator.ClientSecret == clientSecret);
            Assert.IsTrue(authenticator.DisableSslVerification == disableSsl);
        }

        [TestMethod]
        public void TestConstructionDictionaryMissingProperty()
        {
            var apikey = "apikey";

            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add(Authenticator.PropNameApikey, apikey);

            IamAuthenticator authenticator = new IamAuthenticator(config);

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);
            Assert.IsTrue(authenticator.Apikey == apikey);
            Assert.IsTrue(authenticator.ClientId == null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorNoApikey()
        {
            IamAuthenticator authenticator = new IamAuthenticator(
                apikey: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadApikeyBracketBeginning()
        {
            IamAuthenticator authenticator = new IamAuthenticator("{apikey", "http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBracketBeginning()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey", "{http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadApikeyBracketEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey}", "http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBracketEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey", "http://www.service-endpoint.com}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadApikeyBeginningBracketEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("{apikey}", "http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBeginningBracketEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey", "{http://www.service-endpoint.com}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadApikeyQuoteBeginning()
        {
            IamAuthenticator authenticator = new IamAuthenticator("\"apikey", "http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlQuoteBeginning()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey", "\"http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadApikeyQuoteEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey\"", "http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlQuoteEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey", "http://www.service-endpoint.com\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadApikeyBeginningQuoteEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("\"apikey\"", "http://www.service-endpoint.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUrlBeginningQuoteEnd()
        {
            IamAuthenticator authenticator = new IamAuthenticator("apikey", "\"http://www.service-endpoint.com\"");
        }
    }
}
