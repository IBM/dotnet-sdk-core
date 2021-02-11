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
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Http.Formatting;
using IBM.Cloud.SDK.Core.Http.Filters;

namespace IBM.Cloud.SDK.Core.Http
{
    public interface IClient : IDisposable
    {
        HttpClient BaseClient { get; set; }

        WebProxy WebProxy { get; set; }

        MediaTypeFormatterCollection Formatters { get; }

        List<IHttpFilter> Filters { get; }

        string ServiceUrl { get; set; }

        IClient WithAuthentication(string userName, string password);

        IClient WithAuthentication(string bearerToken);

        IRequest DeleteAsync(string resource);

        IRequest GetAsync(string resource);

        IRequest PostAsync(string resource);

        IRequest PostAsync<TBody>(string resource, TBody body);

        IRequest PutAsync(string resource);

        IRequest PutAsync<TBody>(string resource, TBody body);

        IRequest SendAsync(HttpMethod method, string resource);

        IRequest SendAsync(HttpRequestMessage message);

        void DisableSslVerification(bool insecure);
    }
}
