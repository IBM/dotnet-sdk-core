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
using System.IO;
using System.Text;

namespace IBM.Cloud.SDK.Core.Tests.CredentialUtilsTests
{
    [TestClass]
    public class CredentialUtilsTests
    {
        [TestMethod]
        public void TestHasBadStartOrEndChar()
        {
            var bracketBegin = "{value";
            var bracketEnd = "value}";
            var quoteBegin = "\"value";
            var quoteEnd = "value\"";
            var goodValue = "value";

            var testBracketBegin = CredentialUtils.HasBadStartOrEndChar(bracketBegin);
            var testBracketEnd = CredentialUtils.HasBadStartOrEndChar(bracketEnd);
            var testQuoteBegin = CredentialUtils.HasBadStartOrEndChar(quoteBegin);
            var testQuoteEnd = CredentialUtils.HasBadStartOrEndChar(quoteEnd);
            var testGoodValue = CredentialUtils.HasBadStartOrEndChar(goodValue);

            Assert.IsTrue(testBracketBegin);
            Assert.IsTrue(testBracketEnd);
            Assert.IsTrue(testQuoteBegin);
            Assert.IsTrue(testQuoteEnd);
            Assert.IsFalse(testGoodValue);
        }

        [TestMethod]
        public void TestGetFileCredentialsAsMap()
        {
            var fileCredentialsAsMap = CredentialUtils.GetFileCredentialsAsMap("assistant");
            Assert.IsNotNull(fileCredentialsAsMap);
        }

        [TestMethod]
        public void TestGetEnvCredentialsAsMap()
        {
            var apikey = "bogus-apikey";
            Environment.SetEnvironmentVariable(
                "ASSISTANT_" + Authenticator.PropNameApikey,
                apikey);
            var envCredentialsAsMap = CredentialUtils.GetEnvCredentialsAsMap("assistant");
            Assert.IsNotNull(envCredentialsAsMap);
            Assert.IsTrue(envCredentialsAsMap.ContainsKey(Authenticator.PropNameApikey));
            Assert.IsTrue(envCredentialsAsMap.ContainsValue(apikey));
            envCredentialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == apikey);
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMap()
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

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistant");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == apikey);
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapFromInnerEntry()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "fakeapikey"
                }
            };
            vcapCredential.Name = "assistant1";

            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "fakeapikey2"
                }
            };
            vcapCredential2.Name = "assistant2";

            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "fakeapikey3"
                }
            };
            vcapCredential3.Name = "assistant3";
            //map to a single key
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential, vcapCredential2, vcapCredential3 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistant2");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "fakeapikey2");
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapInnerEntryMultKeys()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikeyCopy"
                }
            };
            vcapCredential2.Name = "assistantV1Copy";
            //map to creds to first service
            tempVcapCredential.Add("someService", new List<VcapCredential>() { vcapCredential, vcapCredential2 });

            //create credential entries for second service entry
            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikey"
                }
            };
            vcapCredential3.Name = "assistantV2";

            var vcapCredential4 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikeyCopy"
                }
            };
            vcapCredential4.Name = "assistantV2Copy";

            //map creds to second service
            tempVcapCredential.Add("someOtherService", new List<VcapCredential>() { vcapCredential3, vcapCredential4 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            //should match with inner entry with name "assistantV1Copy"
            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistantV1Copy");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikeyCopy");
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapDuplicateName()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikeyCopy"
                }
            };
            vcapCredential2.Name = "assistantV1Copy";
            //map to creds to first service
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential, vcapCredential2 });

            //create credential entries for second service entry
            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikey"
                }
            };
            vcapCredential3.Name = "assistantV2";

            var vcapCredential4 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikeyCopy"
                }
            };
            vcapCredential4.Name = "assistantV2Copy";

            //map creds to second service
            tempVcapCredential.Add("assistantV1", new List<VcapCredential>() { vcapCredential3, vcapCredential4 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            //should match with inner entry with name "assistantV1"
            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistantV1");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikey");
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapNoMatchingName()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikeyCopy"
                }
            };
            vcapCredential2.Name = "assistantV1Copy";
            //map to creds to first service
            tempVcapCredential.Add("no_matching_name", new List<VcapCredential>() { vcapCredential, vcapCredential2 });

            //create credential entries for second service entry
            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikey"
                }
            };
            vcapCredential3.Name = "assistantV2";

            var vcapCredential4 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikeyCopy"
                }
            };
            vcapCredential4.Name = "assistantV2Copy";

            //map to second service
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential3, vcapCredential4 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("no_matching_name");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikey");
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapMissingNameField()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistant");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikey");
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapEntryNotFound()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("fake_entry");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapVcapNotSet()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("fake_entry");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapEmptySvcName()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [TestMethod]
        public void TestGetVcapCredentialsAsMapNoCreds()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {

            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("no-creds");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [TestMethod]
        public void TestGetServiceProperties()
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

            var serviceProperties = CredentialUtils.GetServiceProperties("assistant");

            Assert.IsNotNull(serviceProperties);
        }

        [TestMethod]
        public void TestCredentialOrder()
        {
            // store and clear user set env variable
            string ibmCredFile = Environment.GetEnvironmentVariable("IBM_CREDENTIALS_FILE");
            Environment.SetEnvironmentVariable("IBM_CREDENTIALS_FILE", "");

            //  create .env file in current directory
            string[] linesWorking = { "TEST_SERVICE_LOCATION=working-directory" };
            var directoryPathWorking = Directory.GetCurrentDirectory();
            var docPathWorking = Path.Combine(directoryPathWorking, "ibm-credentials.env");

            using (StreamWriter outputFile = new StreamWriter(docPathWorking))
            {
                foreach (string line in linesWorking)
                {
                    outputFile.WriteLine(line);
                }
            }

            //  get props
            Dictionary<string, string> propsWorking = CredentialUtils.GetFileCredentialsAsMap("test_service");

            Assert.IsTrue(propsWorking.ContainsKey("LOCATION"));
            propsWorking.TryGetValue("LOCATION", out string envWorkingLocation);
            Assert.IsTrue(envWorkingLocation == "working-directory");

            //  create .env file in user set directory
            string[] lines = { "TEST_SERVICE_LOCATION=user-set-location" };

            var directoryPath = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
                );

            var docPath = Path.Combine(directoryPath, "test-credentials.env");

            using (StreamWriter outputFile = new StreamWriter(docPath))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
            }

            //  set user set env file path variable
            Environment.SetEnvironmentVariable("IBM_CREDENTIALS_FILE", docPath);

            //  get props
            Dictionary<string, string> props = CredentialUtils.GetFileCredentialsAsMap("test_service");

            Assert.IsTrue(props.ContainsKey("LOCATION"));
            props.TryGetValue("LOCATION", out string envLocation);
            Assert.IsTrue(envLocation == "user-set-location");

            //  delete created env files
            if (File.Exists(docPath))
            {
                File.Delete(docPath);
            }

            if (File.Exists(docPathWorking))
            {
                File.Delete(docPathWorking);
            }

            //  reset env variable
            Environment.SetEnvironmentVariable("IBM_CREDENTIALS_FILE", ibmCredFile);
        }
    }
}
