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
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class HttpTests
    {
        [TestMethod]
        public void TestArgEscape()
        {
            IClient client = new IBMHttpClient()
            {
                ServiceUrl = "http://baseuri.com",
            };

            var restRequest = client.GetAsync("/v1/operation");

            restRequest.WithArgument("myArg", "Is this a valid arg?");

            Assert.IsTrue(restRequest.Message.RequestUri == new Uri("http://baseuri.com/v1/operation?myArg=Is+this+a+valid+arg%3F"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NoUrlTest()
        {
            IClient client = new IBMHttpClient();
            var restRequest = client.GetAsync("/v1/operation");
        }

        [TestMethod]
        public void SetServiceUrlTest()
        {
            IClient client = new IBMHttpClient()
            {
                ServiceUrl = "http://www.service-url.com",
            };

            var restRequest = client.GetAsync("/v1/operation");
            Assert.IsTrue(restRequest.Message.RequestUri.AbsoluteUri == "http://www.service-url.com/v1/operation");
        }

        [TestMethod]
        public void SetServiceUrlPropertyTest()
        {
            IClient client = new IBMHttpClient();
            client.ServiceUrl = "http://www.service-url.com";

            var restRequest = client.GetAsync("/v1/operation");
            Assert.IsTrue(restRequest.Message.RequestUri.AbsoluteUri == "http://www.service-url.com/v1/operation");
        }
    }
}
