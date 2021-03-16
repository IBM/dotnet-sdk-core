using System;
namespace IBM.Cloud.SDK.Core.Sockets
{
    public class SynthesizeCallback
    {
        public Action OnOpen = () => { };
        public Action<byte[]> OnMessage = (message) => { };
        public Action<string> OnContentType = (contentType) => { };
        public Action<MarkTiming[]> OnMarks = (marks) => { };
        public Action<WordTiming[]> onTimings = (timings) => { };
        public Action<Exception> OnError = (ex) => { };
        public Action OnClose = () => { };
    }
}
