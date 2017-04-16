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

using System;
using System.Threading.Tasks;
using Idento.Domain.Stores;
using Idento.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Idento.Web.Controllers
{
    [Route("Tenant/{tenantId}/Certificate")]
    public class CertificateController : Controller
    {
        private ICertificateStore _store;
        private ITenantStore _tenantStore;

        public CertificateController(ICertificateStore store, ITenantStore tenantStore)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _tenantStore = tenantStore ?? throw new ArgumentNullException(nameof(tenantStore));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> List(Guid tenantId)
        {
            ViewData["Tenant"] = await _tenantStore.FindById(tenantId) ?? throw new TenantNotFoundException(tenantId);
            var certificates = await _store.GetAll(tenantId);
            return View(certificates);
        }
    }
}
