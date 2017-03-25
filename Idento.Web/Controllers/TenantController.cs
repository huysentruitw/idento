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
using Idento.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Idento.Web.Controllers
{
    public class TenantController : Controller
    {
        private readonly ITenantStore _store;

        public TenantController(ITenantStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new CreateOrUpdateTenantViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrUpdateTenantViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _store.Create(new Domain.Entities.Tenant { Name = model.Name });

                return RedirectToAction(nameof(List));
            }

            // If we got this far, something failed, redisplay form
            return View("CreateOrUpdate", model);
        }
    }
}
