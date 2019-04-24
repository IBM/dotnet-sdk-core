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

namespace IBM.Cloud.SDK.Core.Model
{
    /// <summary>
    /// This class is the base class for generated models with additional properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicModel<T>
    {
        /// <summary>
        /// A Dictionary to keep additional properties.
        /// </summary>
        public Dictionary<string, T> AdditionalProperties { get; } = new Dictionary<string, T>();

        /// <summary>
        /// Add a property to the AdditionalProperties dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, T value)
        {
            AdditionalProperties.Add(key, value);
        }

        /// <summary>
        /// Remove a property from the AdditionalProperties dictionary.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            AdditionalProperties.Remove(key);
        }

        /// <summary>
        /// Get a single property from the AdditionalProperties dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(string key)
        {
            AdditionalProperties.TryGetValue(key, out T value);
            return value;
        }
    }
}
