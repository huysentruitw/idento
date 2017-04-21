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
using Microsoft.AspNetCore.Mvc;

namespace Idento.Web.Controllers
{
    [Route("Certificate")]
    public class CertificateController : Controller
    {
        private ICertificateStore _store;

        public CertificateController(ICertificateStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> List()
        {
            var certificates = await _store.GetAll();
            return View(certificates);
        }
    }
}
