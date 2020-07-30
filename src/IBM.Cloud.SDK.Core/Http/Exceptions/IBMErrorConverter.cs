/**
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
using IBM.Cloud.SDK.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBM.Cloud.SDK.Core.Http.Exceptions
{
    public class IBMErrorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IBMError);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IBMError err = new IBMError();

            err.Message = Utility.GetErrorMessage(jo.ToString());
            err.Response = jo.ToString();

            foreach (var property in jo.Properties())
            {
                switch (property.Name)
                {
                    case "error_code":
                    case "code":
                        int code;
                        int.TryParse(property.Value.ToString(), out code);
                        err.Code = code;
                        break;
                    case "help":
                        err.Help = property.Value.ToString();
                        break;
                    case "description":
                    case "code_description":
                        err.CodeDescription = property.Value.ToString();
                        break;
                    case "session_closed":
                        err.SessionClosed = (bool)property.Value;
                        break;
                    default:
                        break;
                }
            }

            return err;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}