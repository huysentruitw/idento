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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Idento.Domain.Models;
using Idento.Web.Extensions;
using Idento.Web.ManagerUI.LoginProviders.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idento.Web.ManagerUI.LoginProviders
{
    // TODO [Authorize(Roles = "idento_administrator")]
    [Authorize]
    public class LoginProvidersController : Controller
    {
        private readonly LoginProvidersService _service;
        private readonly IMapper _mapper;

        public LoginProvidersController(LoginProvidersService service, IMapper mapper)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var providers = (await _service.GetAll<ExternalLoginProvider>()).ToList();
            var oauth2Providers = providers.Where(x => x.Provider.GetDisplayGroupName() == LoginProviderGroupNames.OAuth2);
            var wsFederationProviders = providers.Where(x => x.Provider.GetDisplayGroupName() == LoginProviderGroupNames.WsFederation);
            return View(new List
            {
                OAuth2Providers = _mapper.Map<IEnumerable<OAuth2ListItem>>(oauth2Providers).OrderBy(x => x.Name),
                WsFederationProviders = _mapper.Map<IEnumerable<WsFederationListItem>>(wsFederationProviders).OrderBy(x => x.Name)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid id)
        {
            var model = await this._service.Create();
            return EditOrCreateView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditOrCreate model)
        {
            if (!ModelState.IsValid)
                return EditOrCreateView();

            await _service.Insert(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetById<EditOrCreate>(id);
            if (model == null)
                return NotFound();

            return EditOrCreateView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditOrCreate model)
        {
            if (!ModelState.IsValid)
                return EditOrCreateView(model);

            if (!await _service.Update(id, model))
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
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
