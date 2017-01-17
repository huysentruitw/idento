/*
 * Copyright 2016 CogniStreamer, Wouter Huysentruit
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
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Idento.Core.Cryptography
{
    public static class Certificate
    {
        public static X509Certificate2 LoadCertificateFromResource<TAnyTypeInTargetAssembly>(string resourceName, string password)
        {
            return LoadCertificateFromResource(typeof(TAnyTypeInTargetAssembly).GetTypeInfo().Assembly, resourceName, password);
        }

        public static X509Certificate2 LoadCertificateFromResource(this Assembly assembly, string resourceName, string password)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                return new X509Certificate2(ReadStream(stream), password);
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}