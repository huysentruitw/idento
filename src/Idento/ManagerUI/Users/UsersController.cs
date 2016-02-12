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
using System.Threading.Tasks;
using Idento.ManagerUI.Users.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Idento.ManagerUI.Users
{
    // TODO [Authorize(Roles = "idento_administrator")]
    [Authorize]
    public class UsersController : Controller
    {
        private UsersService service;

        public UsersController(UsersService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? pageIndex, int? pageSize)
        {
            var model = await service.GetAll<ListItem>();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await this.service.Create();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Create model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await service.EmailInUse(model.Email))
            {
                ModelState.AddModelError("Error", "Email address in use");
                return View(model);
            }

            var result = await service.Insert(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Error", error.Description);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await service.GetById<Create>(id);
            if (model == null)
                return HttpNotFound();

            return EditOrCreateView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Create model)
        {
            if (!ModelState.IsValid)
                return EditOrCreateView(model);

            if (!await service.Update(id, model))
                return HttpNotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await service.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        #region Helpers

        private IActionResult EditOrCreateView(Create model = null)
        {
            return model != null ? View("EditOrCreate", model) : View("EditOrCreate");
        }

        #endregion
    }
}
