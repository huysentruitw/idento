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
using System.IO;
using System.Threading.Tasks;
using Idento.Domain.Stores;
using Idento.Web.Models;
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

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new CreateOrUpdateCertificateViewModel());
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrUpdateCertificateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _store.FindByName(model.Name) == null)
                {
                    using (var stream = model.File.OpenReadStream())
                    using (var reader = new BinaryReader(stream))
                    {
                        await _store.Create(new Domain.Entities.Certificate
                        {
                            Name = model.Name,
                            Data = reader.ReadBytes((int)stream.Length),
                            OriginalFileName = model.File.FileName
                        });
                    }

                    return RedirectToAction(nameof(List));
                }

                //ModelState.AddModelError("Name", "Name already in use");
            }

            // If we got this far, something failed, redisplay form
            return View("CreateOrUpdate", model);
        }
    }
}
