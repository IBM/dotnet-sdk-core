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
using IBM.Cloud.SDK.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IBM.Cloud.SDK.Core.Tests.ConfigBasedAuthenticatorFactoryTests
{
    [TestClass]
    public class ConfigBasedAuthenticatorFactoryTests
    {
        [TestMethod]
        public void GetAuthenticator()
        {
            var apikey = "bogus-apikey";
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = apikey,
                },
            };
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("assistant");
            Assert.IsNotNull(authenticator);
            Assert.IsNotNull(authenticator.AuthenticationType);
        }

        [TestMethod]
        public void TestAuthTypeCase()
        {
            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", Authenticator.AuthTypeIam);
            Environment.SetEnvironmentVariable("TEST_SERVICE_APIKEY", "bogus_apikey");
            Environment.SetEnvironmentVariable("TEST_SERVICE_USERNAME", "bogus_username");
            Environment.SetEnvironmentVariable("TEST_SERVICE_PASSWORD", "bogus_password");
            Environment.SetEnvironmentVariable("TEST_SERVICE_BEARER_TOKEN", "bogus_bearer_token");
            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_URL", "http://bogus-auth-url.com");
            Authenticator authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "iAm");
            authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeIam);

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "bAsIc");
            authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBasic);

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "Noauth");
            authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeNoAuth);

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "cP4d");
            authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeCp4d);

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "bEaReRtOkEn");
            authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeBearer);

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", null);
            Environment.SetEnvironmentVariable("TEST_SERVICE_APIKEY", null);
            Environment.SetEnvironmentVariable("TEST_SERVICE_USERNAME", null);
            Environment.SetEnvironmentVariable("TEST_SERVICE_PASSWORD", null);
            Environment.SetEnvironmentVariable("TEST_SERVICE_BEARER_TOKEN", null);
            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_URL", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadAuthType()
        {
            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "badAuthType");
            Authenticator authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", null);
        }

        [TestMethod]
        public void TestBadAuthTypeMessage()
        {
            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", "badAuthType");
            try
            {
                Authenticator authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("test_service");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual(ae.Message, "Unrecognized authentication type: badAuthType");
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message));
            }

            Environment.SetEnvironmentVariable("TEST_SERVICE_AUTH_TYPE", null);
        }
    }
}
