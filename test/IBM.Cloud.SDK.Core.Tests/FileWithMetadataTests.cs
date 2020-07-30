/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using System.IO;
using System.Text;
using IBM.Cloud.SDK.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBM.Cloud.SDK.Core.Tests
{
    [TestClass]
    public class FileWithMetadataTests
    {
        [TestMethod]
        public void TestFileWithMetadata()
        {
            string fileText = "This is a test";
            string fileName = "testFile.txt";
            string fileContentType = "text/plain";

            using (FileStream fs = File.Create(fileName))
            {
                byte[] fileData = Encoding.UTF8.GetBytes(fileText);
                fs.Write(fileData, 0, fileData.Length);
            }

            FileWithMetadata fileWithMetadata = new FileWithMetadata();

            using (FileStream fs = File.OpenRead(fileName))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    fileWithMetadata.Data = ms;
                    fileWithMetadata.ContentType = fileContentType;
                    fileWithMetadata.Filename = fileName;
                }
            }

            string text = Encoding.UTF8.GetString(fileWithMetadata.Data.ToArray());

            Assert.IsTrue(fileText == text);
            Assert.IsTrue(fileWithMetadata.Filename == fileName);
            Assert.IsTrue(fileWithMetadata.ContentType == fileContentType);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
