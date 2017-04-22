/*
 * Copyright 2016-2017 Wouter Huysentruit
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

using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Idento.Core.Cryptography
{
    public static class Certificate
    {
        public static X509Certificate2 FromStream(Stream stream, string password = null)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return string.IsNullOrEmpty(password)
                    ? new X509Certificate2(memoryStream.ToArray())
                    : new X509Certificate2(memoryStream.ToArray(), password);
            }
        }
    }
}
