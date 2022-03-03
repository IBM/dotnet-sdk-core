using System;
using System.Net;

namespace IBM.Cloud.SDK.Core.Model
{
    public class HttpConfigOptions
    {
        public static string ErrorMessagePropMissing = "The {0} property is required but was not specified.";
        public WebProxy Proxy { get; private set; }
        public bool DisableSslVerification { get; private set; }

        // this empty constructor will be used by the builder 
        public HttpConfigOptions()
        {
        }

        public HttpConfigOptions SetProxy(WebProxy proxy)
        {
            Proxy = proxy;
            return this;
        }

        public HttpConfigOptions SetDisableSslVerification(bool disableSslVerification)
        {
            DisableSslVerification = disableSslVerification;
            return this;
        }

        public HttpConfigOptions Build()
        {
            return this;
        }
    }
}
