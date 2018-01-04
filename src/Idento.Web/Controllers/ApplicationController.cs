/*
 * Copyright 2016-2018 Wouter Huysentruit
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
    [Route("Application")]
    public class ApplicationController : Controller
    {
        private readonly IApplicationStore _store;

        public ApplicationController(IApplicationStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> List()
        {
            var applications = await _store.GetAll();
            return View(applications);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new CreateOrUpdateApplicationViewModel());
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrUpdateApplicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _store.FindByName(model.Name) == null)
                {
                    await _store.Create(new Domain.Entities.Application
                    {
                        Name = model.Name,
                        ClientId = model.ClientId,
                        ClientSecret = model.ClientSecret,
                        RedirectUri = model.RedirectUri
                    });

                    return RedirectToAction(nameof(List));
                }

                ModelState.AddModelError("Name", "Name already in use");
            }

            // If we got this far, something failed, redisplay form
            return View("CreateOrUpdate", model);
        }

        [HttpGet]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var application = await _store.FindById(id);
            if (application == null) return NotFound();
            return View("CreateOrUpdate", new CreateOrUpdateApplicationViewModel { Id = id, Name = application.Name , ClientId = application.ClientId, ClientSecret = application.ClientSecret, RedirectUri = application.RedirectUri });
        }

        [HttpPost]
        [Route("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, CreateOrUpdateApplicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Id.HasValue || model.Id.Value != id) throw new ArgumentException("Invalid Id in model");

                var modelWithSameName = await _store.FindByName(model.Name);
                if (modelWithSameName == null || modelWithSameName.Id == id)
                {
                    await _store.Update(id, x =>
                    {
                        x.ClientId = model.ClientId;
                        x.ClientSecret = model.ClientSecret;
                        x.RedirectUri = model.RedirectUri;
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
        [Route("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var application = await _store.FindById(id);
            if (application == null) return NotFound();
            return View(new ConfirmDeleteApplicationViewModel { Id = id, Name = application.Name });
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _store.Delete(id);
            return RedirectToAction(nameof(List));
        }
    }
}
