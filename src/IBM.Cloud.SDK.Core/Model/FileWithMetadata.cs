﻿/**
* Copyright 2019 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

namespace IBM.Cloud.SDK.Core.Model
{
    /// <summary>
    /// A file and its associated metadata.
    /// </summary>
    public class FileWithMetadata
    {
        /// <summary>
        /// The file data
        /// </summary>
        public System.IO.MemoryStream Data { get; set; }

        /// <summary>
        /// The filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The file contentType
        /// </summary>
        public string ContentType { get; set; }
    }
}
