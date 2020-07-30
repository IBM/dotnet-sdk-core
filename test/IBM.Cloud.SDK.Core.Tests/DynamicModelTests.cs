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

using System.Collections.Generic;
using IBM.Cloud.SDK.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class DynamicModelTests
    {
        [TestMethod]
        public void TestAddRestricted()
        {
            DynamicModel<List<string>> dynamicModel = new DynamicModel<List<string>>();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList")[0] == "IBM");
        }

        [TestMethod]
        public void TestRemoveRestricted()
        {
            DynamicModel<List<string>> dynamicModel = new DynamicModel<List<string>>();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList")[0] == "IBM");

            dynamicModel.Remove("myList");
            Assert.IsFalse(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsFalse(dynamicModel.AdditionalProperties.ContainsValue(JToken.FromObject(myList)));
        }

        [TestMethod]
        public void TestGetRestricted()
        {
            DynamicModel<List<string>> dynamicModel = new DynamicModel<List<string>>();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList")[0] == "IBM");

            List<string> myList2 = dynamicModel.Get("myList");
            Assert.IsTrue(myList2[0] == myList[0]);
        }

        [TestMethod]
        public void TestGetAdditionalPropertiesRestricted()
        {
            DynamicModel<List<string>> dynamicModel = new DynamicModel<List<string>>();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList")[0] == "IBM");

            Dictionary<string, JToken> additionalProperties = dynamicModel.AdditionalProperties;
            Assert.IsTrue(additionalProperties == dynamicModel.AdditionalProperties);
        }

        [TestMethod]
        public void TestAdd()
        {
            DynamicModel dynamicModel = new DynamicModel();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList").ToObject<List<string>>()[0] == "IBM");
        }

        [TestMethod]
        public void TestRemove()
        {
            DynamicModel dynamicModel = new DynamicModel();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList").ToObject<List<string>>()[0] == "IBM");

            dynamicModel.Remove("myList");
            Assert.IsFalse(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsFalse(dynamicModel.AdditionalProperties.ContainsValue(JToken.FromObject(myList)));
        }

        [TestMethod]
        public void TestGet()
        {
            DynamicModel dynamicModel = new DynamicModel();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList").ToObject<List<string>>()[0] == "IBM");

            List<string> myList2 = dynamicModel.Get("myList").ToObject<List<string>>();
            Assert.IsTrue(myList2[0] == myList[0]);
        }

        [TestMethod]
        public void TestGetAdditionalProperties()
        {
            DynamicModel dynamicModel = new DynamicModel();
            List<string> myList = new List<string>();
            myList.Add("IBM");
            dynamicModel.Add("myList", myList);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myList"));
            Assert.IsTrue(dynamicModel.Get("myList").ToObject<List<string>>()[0] == "IBM");

            Dictionary<string, JToken> additionalProperties = dynamicModel.AdditionalProperties;
            Assert.IsTrue(additionalProperties == dynamicModel.AdditionalProperties);
        }
    }
}
