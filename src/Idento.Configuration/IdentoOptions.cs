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

using System;
using System.Security.Cryptography.X509Certificates;

namespace Idento.Configuration
{
    public class IdentoOptions
    {
        public IdentoOptions()
        {
            this.RequireSsl = true;
            this.EnableLogging = false;
        }

        public bool RequireSsl { get; set; }
        public bool EnableLogging { get; set; }
        public X509Certificate2 SigningCertificate { get; set; }
        public string ConnectionString { get; set; }
        public string Title { get; set; }
        public string LogoFilePath { get; set; }

        public void Validate()
        {
            if (this.SigningCertificate == null)
                throw new ArgumentNullException("SigningCertificate", "SigningCertificate not set");

            if (string.IsNullOrWhiteSpace(this.ConnectionString))
                throw new ArgumentNullException("ConnectionString", "ConnectionString not set");

            if (string.IsNullOrWhiteSpace(this.Title))
                throw new ArgumentNullException("Title", "Title not set");
        }
    }
}
