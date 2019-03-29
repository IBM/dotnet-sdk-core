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

        [TestMethod]
        public void GetErrorMessageFromErrors()
        {
            string json = "{\"errors\":[{\"code\":\"missing_field\",\"message\":\"The request path is not valid. Make sure that the endpoint is correct.\",\"more_info\":\"https://cloud.ibm.com/apidocs/visual-recognition-v4\",\"target\":{\"type\":\"field\",\"name\":\"URL path\"}}],\"trace\":\"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "The request path is not valid. Make sure that the endpoint is correct.");
        }

        [TestMethod]
        public void GetErrorMessageFromError()
        {
            string json = "{\"code\":\"400\",\"error\":\"Error: Too many images in collection\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "Error: Too many images in collection");
        }

        [TestMethod]
        public void GetErrorMessageFromMessage()
        {
            string json = "{\"code\":\"string\",\"message\":\"string\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "string");
        }

        [TestMethod]
        public void GetErrorMessageFromErrorMessage()
        {
            string json = "{\"errorCode\": \"string\", \"errorMessage\": \"Provided API key could not be found\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "Provided API key could not be found");
        }

        [TestMethod]
        public void GetErrorMessageFromErrorsWithErrorAndMessageAndErrorMessage()
        {
            string json = "{\"errors\":[{\"code\":\"missing_field\",\"message\":\"The request path is not valid. Make sure that the endpoint is correct.\",\"more_info\":\"https://cloud.ibm.com/apidocs/visual-recognition-v4\",\"target\":{\"type\":\"field\",\"name\":\"URL path\"}}],\"error\": \"Error: Too many images in collection\", \"message\": \"string\", \"trace\": \"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\", \"errorMessage\": \"Provided API key could not be found\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "The request path is not valid. Make sure that the endpoint is correct.");
        }

        [TestMethod]
        public void GetErrorMessageWithErrorAndMessageAndErrorMessage()
        {
            string json = "{\"error\": \"Error: Too many images in collection\", \"message\": \"string\", \"trace\": \"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\", \"errorMessage\": \"Provided API key could not be found\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "Error: Too many images in collection");
        }

        [TestMethod]
        public void GetErrorMessageWithMessageAndErrorMessage()
        {
            string json = "{\"message\": \"string\", \"trace\": \"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\", \"errorMessage\": \"Provided API key could not be found\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "string");
        }

        [TestMethod]
        public void GetGenericErrorMessage()
        {
            string json = "{\"msg\": \":(\"}";
            string errorMessage = Utility.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "unknown error");
        }
    }
}
