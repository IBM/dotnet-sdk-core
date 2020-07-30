/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
using Newtonsoft.Json;

namespace IBM.Cloud.SDK.Core.Util
{
    /// <summary>
    /// Vcap credentials object.
    /// </summary>
    public class VcapCredentials
    {
        /// <summary>
        /// List of credentials by service name.
        /// </summary>
        [JsonProperty("VCAP_SERVICES", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<VcapCredential>> VCAP_SERVICES { get; set; }
    }

    /// <summary>
    /// The Credential to a single service.
    /// </summary>
    public class VcapCredential
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("plan", NullValueHandling = NullValueHandling.Ignore)]
        public string Plan { get; set; }

        [JsonProperty("credentials", NullValueHandling = NullValueHandling.Ignore)]
        public Credential Credentials { get; set; }
    }

    /// <summary>
    /// The Credentials.
    /// </summary>
    public class Credential
    {
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("api_key", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiKey { get; set; }

        [JsonProperty("apikey", NullValueHandling = NullValueHandling.Ignore)]
        public string IamApikey { get; set; }

        [JsonProperty("workspace_id", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkspaceId { get; set; }

        [JsonProperty("environment_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentId { get; set; }

        [JsonProperty("classifier_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ClassifierId { get; set; }

        [JsonProperty("assistant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string AssistantId { get; set; }
    }
}
