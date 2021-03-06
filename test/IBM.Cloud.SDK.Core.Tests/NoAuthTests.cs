﻿/**
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
using IBM.Cloud.SDK.Core.Authentication.NoAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBM.Cloud.SDK.Core.Tests.Authentication.NoAuth
{
    [TestClass]
    public class NoAuthTests
    {
        [TestMethod]
        public void TestConstruction()
        {
            NoAuthAuthenticator authenticator = new NoAuthAuthenticator();

            Assert.IsNotNull(authenticator);
            Assert.IsTrue(authenticator.AuthenticationType == Authenticator.AuthTypeNoAuth);
        }
    }
}
