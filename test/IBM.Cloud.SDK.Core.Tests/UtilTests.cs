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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Cloud.SDK.Core.Util;
using System.Collections.Generic;
using System.IO;
using System;

namespace IBM.Cloud.SDK.Core.Tests.Util
{
    [TestClass]
    public class UtilTests
    {
        [TestMethod]
        public void GetCredentialPaths()
        {
            List<string> paths = Utility.GetCredentialsPaths();
            Assert.IsNotNull(paths);
            Assert.IsTrue(paths.Count > 0);
        }

        [TestMethod]
        public void LoadEnvFileSuccess()
        {
            List<string> paths = Utility.GetCredentialsPaths();

            bool success = false;
            if (paths.Count > 0)
                success = Utility.LoadEnvFile(paths[0]);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void LoadIBMCredentialsFileFromEnv()
        {
            List<string> paths = Utility.GetCredentialsPaths();
            Environment.SetEnvironmentVariable("IBM_CREDENTIALS_FILE", paths[0]);
            paths = Utility.GetCredentialsPaths();

            Assert.IsNotNull(paths);
            Assert.IsTrue(paths.Count > 0);
        }

        [TestMethod]
        public void LoadEnvFileFail()
        {
            List<string> paths = new List<string>();
            bool success = Utility.LoadEnvFile("bogus-filepath");
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void HasBadFirstOrLastCharacterSuccessStartingBracket()
        {
            bool success = Utility.HasBadFirstOrLastCharacter("{bogus-string");
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void HasBadFirstOrLastCharacterSuccessStartingQuote()
        {
            bool success = Utility.HasBadFirstOrLastCharacter("\"bogus-string");
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void HasBadFirstOrLastCharacterSuccessEndingBracket()
        {
            bool success = Utility.HasBadFirstOrLastCharacter("bogus-string}");
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void HasBadFirstOrLastCharacterSuccessEndingQuote()
        {
            bool success = Utility.HasBadFirstOrLastCharacter("bogus-string\"");
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void HasBadFirstOrLastCharacterSuccessStartingEndingBracket()
        {
            bool success = Utility.HasBadFirstOrLastCharacter("{bogus-string}");
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void HasBadFirstOrLastCharacterSuccessStartingEndingQuote()
        {
            bool success = Utility.HasBadFirstOrLastCharacter("\"bogus-string\"");
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void AddTopLevelObjectToJsonSuccess()
        {
            string json = "{\"json\": \"value\"}";
            string updatedJson = Utility.AddTopLevelObjectToJson(json, "top");

            Assert.IsTrue(updatedJson == "{\"top\": {\"json\": \"value\"}}");
        }

        [TestMethod]
        public void CopyStreamSuccess()
        {
            Stream inputStream = new MemoryStream(new byte[4]);
            Stream outputStream = new MemoryStream();

            Utility.CopyStream(inputStream, outputStream);

            Assert.IsNotNull(outputStream);
            Assert.IsTrue(outputStream.Length == 4);
        }

        [TestMethod]
        public void SimpleGetSuccess()
        {
            string response = Utility.SimpleGet("https://jsonplaceholder.typicode.com/users").Result;
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SimpleGetWithCredentialsSuccess()
        {
            string response = Utility.SimpleGet("https://jsonplaceholder.typicode.com/users", "bogus-username", "bogus-password").Result;
            Assert.IsNotNull(response);
        }
    }
}
