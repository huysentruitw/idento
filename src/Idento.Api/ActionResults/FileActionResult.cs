/*
 * Copyright 2016 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//using System;
//using System.IO;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;

//namespace Idento.Api.ActionResults
//{
//    internal class FileActionResult : IHttpActionResult
//    {
//        private readonly string filePath;
//        private readonly string contentType;

//        public FileActionResult(string filePath, string contentType = null)
//        {
//            if (string.IsNullOrWhiteSpace(filePath))
//                throw new ArgumentNullException("filePath");

//            if (!File.Exists(filePath))
//                throw new FileNotFoundException();

//            this.filePath = filePath;
//            this.contentType = contentType;
//        }

//        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//        {
//            return Task.FromResult<HttpResponseMessage>(new FileHttpResponseMessage(this.filePath, this.contentType));
//        }

//        private class FileHttpResponseMessage : HttpResponseMessage
//        {
//            public FileHttpResponseMessage(string filePath, string contentType)
//            {
//                this.StatusCode = HttpStatusCode.OK;
//                this.Content = new StreamContent(File.OpenRead(filePath));
//                this.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? MimeMapping.GetMimeMapping(filePath));
//            }

//            protected override void Dispose(bool disposing)
//            {
//                base.Dispose(disposing);
//                this.Content.Dispose();
//            }
//        }
//    }
//}
