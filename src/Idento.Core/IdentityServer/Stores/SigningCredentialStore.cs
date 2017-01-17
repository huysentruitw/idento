/*
 * Copyright (c) Wouter Huysentruit
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

using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Idento.Domain.Models;
using Idento.Domain.Stores;
using Microsoft.IdentityModel.Tokens;

namespace Idento.Core.IdentityServer.Stores
{
    public class SigningCredentialStore : ISigningCredentialStore
    {
        private readonly ICertificateStore certificateStore;

        public SigningCredentialStore(ICertificateStore certificateStore)
        {
            this.certificateStore = certificateStore;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var certificate = await this.certificateStore.GetFirstByPurpose(CertificatePurpose.PrimarySigning);
            if (certificate == null) return null;
            return new SigningCredentials(new X509SecurityKey(new X509Certificate2(certificate.Data)), "RS256");
        }
    }
}
