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
                if (await _store.FindByName(model.Name) == null)
                {
                    await _store.Create(new Domain.Entities.Tenant
                    {
                        Name = model.Name
                    });

                    return RedirectToAction(nameof(List));
                }

                ModelState.AddModelError("Name", "Name already in use");
            }

            // If we got this far, something failed, redisplay form
            return View("CreateOrUpdate", model);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tenants = await _store.GetAll();
            return View(tenants);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tenant = await _store.FindById(id);
            return View("CreateOrUpdate", new CreateOrUpdateTenantViewModel { Id = id, Name = tenant.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Guid id, CreateOrUpdateTenantViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Id.HasValue || model.Id.Value != id) throw new ArgumentException("Invalid Id in model");

                var modelWithSameName = await _store.FindByName(model.Name);
                if (modelWithSameName == null || modelWithSameName.Id == id)
                {
                    await _store.Update(id, x =>
                    {
                        x.Name = model.Name;
                    });

                    return RedirectToAction(nameof(List));
                }

                ModelState.AddModelError("Name", "Name already in use");
            }

            // If we got this far, something failed, redisplay form
            return View("CreateOrUpdate", model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var tenant = await _store.FindById(id);
            return View(new ConfirmDeleteTenantViewModel { Id = id, Name = tenant.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _store.Delete(id);
            return RedirectToAction(nameof(List));
        }
    }
}
