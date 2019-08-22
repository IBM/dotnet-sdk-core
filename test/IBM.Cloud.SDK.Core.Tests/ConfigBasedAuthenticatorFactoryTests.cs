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
using IBM.Cloud.SDK.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
                    ApiKey = apikey
                }
            };
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var authenticator = ConfigBasedAuthenticatorFactory.GetAuthenticator("assistant");
            Assert.IsNotNull(authenticator);
            Assert.IsNotNull(authenticator.AuthenticationType);
        }
    }
}
