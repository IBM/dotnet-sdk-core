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

using IBM.Cloud.SDK.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class DynamicModelTests
    {
        [TestMethod]
        public void TestAdd()
        {
            DynamicModel<object> dynamicModel = new DynamicModel<object>();
            object myObject = new object();
            dynamicModel.Add("myObject", myObject);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myObject"));
            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsValue(myObject));
        }

        [TestMethod]
        public void TestRemove()
        {
            DynamicModel<object> dynamicModel = new DynamicModel<object>();
            object myObject = new object();
            dynamicModel.Add("myObject", myObject);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myObject"));
            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsValue(myObject));

            dynamicModel.Remove("myObject");
            Assert.IsFalse(dynamicModel.AdditionalProperties.ContainsKey("myObject"));
            Assert.IsFalse(dynamicModel.AdditionalProperties.ContainsValue(myObject));
        }

        [TestMethod]
        public void TestGet()
        {
            DynamicModel<object> dynamicModel = new DynamicModel<object>();
            object myObject = new object();
            dynamicModel.Add("myObject", myObject);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myObject"));
            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsValue(myObject));

            object myObject2 = dynamicModel.Get("myObject");
            Assert.IsTrue(myObject2 == myObject);
        }

        [TestMethod]
        public void TestGetAdditionalProperties()
        {
            DynamicModel<object> dynamicModel = new DynamicModel<object>();
            object myObject = new object();
            dynamicModel.Add("myObject", myObject);

            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsKey("myObject"));
            Assert.IsTrue(dynamicModel.AdditionalProperties.ContainsValue(myObject));

            Dictionary<string, object> additionalProperties = dynamicModel.AdditionalProperties;
            Assert.IsTrue(additionalProperties == dynamicModel.AdditionalProperties);
        }
    }
}
