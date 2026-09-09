/**
* Copyright 2026 IBM Corp. All Rights Reserved.
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
*/

using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Http.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class RequestFilterTests
    {
        [TestMethod]
        public async Task RequestFiltersRunBeforeDispatch()
        {
            var events = new List<string>();
            var filter = new CallbackFilter(
                message =>
                {
                    events.Add("request");
                    message.Headers.Add("X-Prepared", "true");
                },
                () => events.Add("response"));

            using (var message = new HttpRequestMessage(HttpMethod.Post, "http://localhost/test"))
            using (var response = new HttpResponseMessage(HttpStatusCode.OK))
            {
                bool preparedAtDispatch = false;
                var request = new Request(message, new MediaTypeFormatterCollection(), current =>
                {
                    events.Add("dispatch");
                    preparedAtDispatch = current.Message.Headers.Contains("X-Prepared");
                    return Task.FromResult(response);
                }, new[] { filter });

                Assert.AreSame(response, await request.AsMessage());
                Assert.IsTrue(preparedAtDispatch);
                CollectionAssert.AreEqual(new[] { "request", "dispatch", "response" }, events);
            }
        }

        [TestMethod]
        public async Task FailingRequestFilterPreventsDispatch()
        {
            int dispatchCount = 0;
            int responseCount = 0;
            var failure = new InvalidOperationException("Request preparation failed.");
            var filter = new CallbackFilter(message => { throw failure; }, () => responseCount++);

            using (var message = new HttpRequestMessage(HttpMethod.Post, "http://localhost/test"))
            using (var response = new HttpResponseMessage(HttpStatusCode.OK))
            {
                var request = new Request(message, new MediaTypeFormatterCollection(), current =>
                {
                    dispatchCount++;
                    return Task.FromResult(response);
                }, new[] { filter });

                var actual = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => request.AsMessage());
                Assert.AreSame(failure, actual);
                Assert.AreEqual(0, dispatchCount);
                Assert.AreEqual(0, responseCount);
            }
        }

        [TestMethod]
        public async Task ConcurrentAndRepeatedReadsPrepareAndDispatchOnce()
        {
            int preparationCount = 0;
            int dispatchCount = 0;
            int startedReads = 0;
            var filter = new CallbackFilter(message => Interlocked.Increment(ref preparationCount), () => { });
            var responseSource = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (var message = new HttpRequestMessage(HttpMethod.Post, "http://localhost/test"))
            using (var response = new HttpResponseMessage(HttpStatusCode.OK))
            {
                var request = new Request(message, new MediaTypeFormatterCollection(), current =>
                {
                    Interlocked.Increment(ref dispatchCount);
                    return responseSource.Task;
                }, new[] { filter });

                Assert.AreEqual(0, preparationCount);
                Assert.AreEqual(0, dispatchCount);
                var reads = Enumerable.Range(0, 8).Select(index => Task.Run(async () =>
                {
                    var pending = request.AsMessage();
                    if (Interlocked.Increment(ref startedReads) == 8)
                        responseSource.SetResult(response);
                    return await pending;
                })).ToArray();

                var responses = await Task.WhenAll(reads);
                foreach (var actual in responses)
                    Assert.AreSame(response, actual);
                Assert.AreSame(response, await request.AsMessage());
                Assert.AreEqual(1, preparationCount);
                Assert.AreEqual(1, dispatchCount);
            }
        }

        [TestMethod]
        public async Task RepeatedReadsDoNotRetryFailedPreparation()
        {
            int preparationCount = 0;
            int dispatchCount = 0;
            var failure = new InvalidOperationException("Request preparation failed.");
            var filter = new CallbackFilter(message =>
            {
                preparationCount++;
                throw failure;
            }, () => { });

            using (var message = new HttpRequestMessage(HttpMethod.Post, "http://localhost/test"))
            using (var response = new HttpResponseMessage(HttpStatusCode.OK))
            {
                var request = new Request(message, new MediaTypeFormatterCollection(), current =>
                {
                    dispatchCount++;
                    return Task.FromResult(response);
                }, new[] { filter });

                for (int i = 0; i < 2; i++)
                {
                    var actual = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => request.AsMessage());
                    Assert.AreSame(failure, actual);
                }
                Assert.AreEqual(1, preparationCount);
                Assert.AreEqual(0, dispatchCount);
            }
        }

        private sealed class CallbackFilter : IHttpFilter
        {
            private readonly Action<HttpRequestMessage> onRequest;
            private readonly Action onResponse;

            public CallbackFilter(Action<HttpRequestMessage> onRequest, Action onResponse)
            {
                this.onRequest = onRequest;
                this.onResponse = onResponse;
            }

            public void OnRequest(IRequest request, HttpRequestMessage message)
            {
                onRequest(message);
            }

            public void OnResponse(IResponse response, HttpResponseMessage message)
            {
                onResponse();
            }
        }
    }
}
