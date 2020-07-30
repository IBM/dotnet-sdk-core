﻿/**
* Copyright 2017 IBM Corp. All Rights Reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace IBM.Cloud.SDK.Core.Util
{
    /// <summary>
    /// Utility classes for the IBM Cloud SDKs.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Async method for making a simple get request.
        /// </summary>
        /// <param name="url">The URL to make the request.</param>
        /// <param name="username">The username for request authentication.</param>
        /// <param name="password">The password for request authentication.</param>
        /// <returns>A string response from the request.</returns>
        public static async Task<string> SimpleGet(string url, string username = null, string password = null)
        {
            HttpClientHandler handler = null;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var credentials = new NetworkCredential(username, password);
                handler = new HttpClientHandler()
                {
                    Credentials = credentials,
                };
            }
            else
            {
                handler = new HttpClientHandler();
            }

            var client = new HttpClient(handler == null ? null : handler);
            var stringTask = client.GetStringAsync(url);
            var msg = await stringTask;

            return msg;
        }

        /// <summary>
        /// Copies an input stream to an output stream.
        /// </summary>
        /// <param name="input">The input stream to copy.</param>
        /// <param name="output">The output stream to copy to.</param>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        /// <summary>
        /// Add a top level object to a json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <param name="objectName">The name of the top level object.</param>
        /// <returns></returns>
        public static string AddTopLevelObjectToJson(string json, string objectName)
        {
            string convertedJson = json.Insert(1, "\"" + objectName + "\": {");
            return convertedJson.Insert(convertedJson.Length - 1, "}");
        }

        /// <summary>
        /// Loads environment variables from an external file.
        /// </summary>
        /// <param name="filepath">The location in the file system from which to load environment variables.</param>
        public static Dictionary<string, string> LoadEnvFile(string filepath, string serviceName)
        {
            List<string> lines = new List<string>();
            string[] rawLines = { };

            try
            {
                rawLines = File.ReadAllLines(filepath);
            }
            catch
            {
                return null;
            }

            foreach (string line in rawLines)
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("#") && line.Contains("="))
                {
                    lines.Add(line);
                }
            }

            Dictionary<string, string> envDict = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                string[] kvp = line.Split(new char[] { '=' }, 2);
                if (kvp[0].StartsWith(serviceName.ToUpper()))
                {
                    string propName = kvp[0].ToUpper().Substring(serviceName.Length + 1);
                    envDict.Add(kvp[0], kvp[1]);
                }
            }

            return envDict;
        }

        public static string GetErrorMessage(string input)
        {
            JObject o = JObject.Parse(input);

            if (o["errors"] != null)
            {
                JToken errorsArray = o["errors"];
                return errorsArray[0]["message"].ToString();
            }

            if (o["error"] != null)
            {
                return o["error"].ToString();
            }

            if (o["message"] != null)
            {
                return o["message"].ToString();
            }

            if (o["errorMessage"] != null)
            {
                return o["errorMessage"].ToString();
            }

            return "unknown error";
        }

        public static string ParseCultureInvariantFloatToString(float value)
        {
            return value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Is only one of the supplied values set?
        /// </summary>
        /// <param name="a">Value A</param>
        /// <param name="b">Value B</param>
        /// <returns></returns>
        public static bool OnlyOne(string a, string b)
        {
            return (string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b)) || (!string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b));
        }

        #region ConvertToUtf8

        /// <summary>
        /// Convert a string to a UTF-8 encoded string
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <returns>UTF-8 encoded string</returns>
        public static string ConvertToUtf8(string input)
        {
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Text.Encoding.UTF8.GetString(utf8Bytes);
        }
        #endregion
    }
}
