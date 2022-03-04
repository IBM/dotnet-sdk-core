/**
* Copyright 2022 IBM Corp. All Rights Reserved.
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
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Cloud.SDK.Core.Model;
using IBM.Cloud.SDK.Core.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class HttpConfigTest
    {
        readonly string serviceUrl = "https://api.us-south.assistant.watson.cloud.ibm.com";

        public class FakeService : IBMService
        {
            public FakeService(string serviceName, IAuthenticator authenticator) : base(serviceName, authenticator)
            {
            }
        }

        [TestMethod]
        public void testHttpConfigOptions()
        {
            HttpConfigOptions httpConfigOptions = new HttpConfigOptions();

            Assert.IsFalse(httpConfigOptions.DisableSslVerification);
            Assert.IsNull(httpConfigOptions.Proxy);

            WebProxy webProxy = new WebProxy("http://localhost:8080");

            httpConfigOptions = httpConfigOptions
                .SetDisableSslVerification(true)
                .SetProxy(webProxy)
                .Build();

            Assert.IsTrue(httpConfigOptions.DisableSslVerification);
            Assert.IsNotNull(httpConfigOptions.Proxy);
            Assert.AreEqual(webProxy, httpConfigOptions.Proxy);

            var apikey = "apikey";
            var serviceName = "fakeservice";

            IamAuthenticator authenticator = new IamAuthenticator(apikey);
            IBMService service = new FakeService(serviceName, authenticator);

            service.SetServiceUrl(serviceUrl);

            Assert.IsNull(service.Client.WebProxy);
            Assert.AreEqual(serviceName, service.ServiceName);
            Assert.AreEqual(serviceUrl, service.Client.ServiceUrl);

            service.ConfigureClient(httpConfigOptions);
            Assert.AreEqual(webProxy, service.Client.WebProxy);
            Assert.AreEqual(serviceName, service.ServiceName);
            Assert.AreEqual(serviceUrl, service.Client.ServiceUrl);
        }
    }
}
