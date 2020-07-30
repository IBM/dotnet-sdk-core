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

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using IBM.Cloud.SDK.Core.Http.Exceptions;
using Newtonsoft.Json;

namespace IBM.Cloud.SDK.Core.Http.Filters
{
    public class ErrorFilter : IHttpFilter
    {
        public void OnRequest(IRequest request, HttpRequestMessage requestMessage) { }

        public void OnResponse(IResponse response, HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                HttpHeaders responseHeaders = responseMessage.Headers;

                IEnumerable<string> globalTransactionId;
                string globalTransactionIdString = string.Empty;
                if (responseHeaders.TryGetValues("x-global-transaction-id", out globalTransactionId))
                {
                    globalTransactionIdString = string.Join(", ", globalTransactionId);
                }

                ServiceResponseException exception =
                    new ServiceResponseException(response, responseMessage, $"The API query failed with status code {responseMessage.StatusCode}: {responseMessage.ReasonPhrase} | x-global-transaction-id: {globalTransactionIdString} | error: {responseMessage.Content.ReadAsStringAsync().Result}");

                var error = responseMessage.Content.ReadAsStringAsync().Result;

                if (responseMessage.Content.Headers?.ContentType?.MediaType == HttpMediaType.ApplicationJson)
                {
                    exception.Error = JsonConvert.DeserializeObject<IBMError>(error);
                }
                else
                {
                    exception.Error = new IBMError()
                    {
                        CodeDescription = responseMessage.StatusCode.ToString(),
                        Message = error,
                    };
                }

                throw exception;
            }
        }
    }
}
