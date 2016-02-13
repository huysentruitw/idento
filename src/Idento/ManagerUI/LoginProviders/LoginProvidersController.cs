﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Idento.Domain.Models;
using Idento.Helpers;
using Idento.ManagerUI.LoginProviders.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Idento.ManagerUI.LoginProviders
{
    // TODO [Authorize(Roles = "idento_administrator")]
    [Authorize]
    public class LoginProvidersController : Controller
    {
        private LoginProvidersService service;
        private IMapper mapper;

        public LoginProvidersController(LoginProvidersService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var providers = await service.GetAll<ExternalLoginProvider>();
            var oauth2Providers = providers.Where(x => x.Provider.GetDisplayGroupName() == LoginProviderGroupNames.OAuth2);
            var wsFederationProviders = providers.Where(x => x.Provider.GetDisplayGroupName() == LoginProviderGroupNames.WsFederation);
            return View(new List
            {
                OAuth2Providers = mapper.Map<IEnumerable<OAuth2ListItem>>(oauth2Providers).OrderBy(x => x.Name),
                WsFederationProviders = mapper.Map<IEnumerable<WsFederationListItem>>(wsFederationProviders).OrderBy(x => x.Name)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid id)
        {
            var model = await this.service.Create();
            return EditOrCreateView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditOrCreate model)
        {
            if (!ModelState.IsValid)
                return EditOrCreateView();

            await service.Insert(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await service.GetById<EditOrCreate>(id);
            if (model == null)
                return HttpNotFound();

            return EditOrCreateView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditOrCreate model)
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

        private IActionResult EditOrCreateView(EditOrCreate model = null)
        {
            ViewBag.AvailableProviders = EnumExtensions.ToSelectList<LoginProvider>(true);
            return model != null ? View("EditOrCreate", model) : View("EditOrCreate");
        }

        #endregion
    }
}
