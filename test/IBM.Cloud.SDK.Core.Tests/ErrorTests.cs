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

using IBM.Cloud.SDK.Core.Http.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        public void GetErrorMessageFromErrors()
        {
            string json = "{\"errors\":[{\"code\":\"missing_field\",\"message\":\"The request path is not valid. Make sure that the endpoint is correct.\",\"more_info\":\"https://cloud.ibm.com/apidocs/visual-recognition-v4\",\"target\":{\"type\":\"field\",\"name\":\"URL path\"}}],\"trace\":\"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "The request path is not valid. Make sure that the endpoint is correct.");
        }

        [TestMethod]
        public void GetErrorMessageFromError()
        {
            string json = "{\"code\":\"400\",\"error\":\"Error: Too many images in collection\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "Error: Too many images in collection");
        }

        [TestMethod]
        public void GetErrorMessageFromMessage()
        {
            string json = "{\"code\":\"string\",\"message\":\"string\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "string");
        }

        [TestMethod]
        public void GetErrorMessageFromErrorMessage()
        {
            string json = "{\"errorCode\": 404, \"errorMessage\": \"Provided API key could not be found\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "Provided API key could not be found");
        }

        [TestMethod]
        public void GetErrorMessageFromErrorsWithErrorAndMessageAndErrorMessage()
        {
            string json = "{\"errors\":[{\"code\":\"missing_field\",\"message\":\"The request path is not valid. Make sure that the endpoint is correct.\",\"more_info\":\"https://cloud.ibm.com/apidocs/visual-recognition-v4\",\"target\":{\"type\":\"field\",\"name\":\"URL path\"}}],\"error\": \"Error: Too many images in collection\", \"message\": \"string\", \"trace\": \"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\", \"errorMessage\": \"Provided API key could not be found\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "The request path is not valid. Make sure that the endpoint is correct.");
        }

        [TestMethod]
        public void GetErrorMessageWithErrorAndMessageAndErrorMessage()
        {
            string json = "{\"error\": \"Error: Too many images in collection\", \"message\": \"string\", \"trace\": \"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\", \"errorMessage\": \"Provided API key could not be found\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "Error: Too many images in collection");
        }

        [TestMethod]
        public void GetErrorMessageWithMessageAndErrorMessage()
        {
            string json = "{\"message\": \"string\", \"trace\": \"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\", \"errorMessage\": \"Provided API key could not be found\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "string");
        }

        [TestMethod]
        public void GetGenericErrorMessage()
        {
            string json = "{\"msg\": \":(\"}";
            Error error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsTrue(error.Message == "unknown error");
        }
    }
}
