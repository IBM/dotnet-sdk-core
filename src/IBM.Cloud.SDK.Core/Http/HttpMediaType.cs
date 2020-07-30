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

namespace IBM.Cloud.SDK.Core.Http
{
    public static class HttpMediaType
    {
        public const string ApplicationAtomXml = "application/atom+xml";
        public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
        public const string ApplicationJson = "application/json";
        public const string ApplicationMsWord = "application/msword";
        public const string ApplicationMsWordDocx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string ApplicationOctetStream = "application/octet-stream";
        public const string ApplicationPdf = "application/pdf";
        public const string ApplicationSvgXml = "application/svg+xml";
        public const string ApplicationXhtmlXml = "application/xhtml+xml";
        public const string ApplicationZip = "application/zip";
        public const string ApplicationXml = "application/xml";
        public const string AudioOgg = "audio/ogg; codecs=opus";
        public const string AudioOggVorbis = "audio/ogg; codecs=vorbis";
        public const string AudioWav = "audio/wav";
        public const string AudioPcm = "audio/l16";
        public const string AudioBasic = "audio/basic";
        public const string AudioFlac = "audio/flac";
        public const string AudioRaw = "audio/l16";

        public const string BinaryOctetStream = "binary/octet-stream";
        public const string Json = ApplicationJson + "; charset=utf-8";
        public const string MediaTypeWildcard = "*";
        public const string MultipartFormData = "multipart/form-data";
        public const string TextCsv = "text/csv";
        public const string TextHtml = "text/html";
        public const string TextPlain = "text/plain";
        public const string Text = TextPlain + "; charset=utf-8";
        public const string TextXml = "text/xml";
        public const string Wildcard = "*/*";

        public static string CreateAudioRaw(int rate)
        {
            return AudioRaw + "; rate=" + rate;
        }
    }
}