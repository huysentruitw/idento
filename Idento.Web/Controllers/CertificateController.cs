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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Idento.Core.Cryptography;
using Idento.Domain.Stores;
using Idento.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Idento.Web.Controllers
{
    [Route("Certificate")]
    public class CertificateController : Controller
    {
        private const int MaximumCertificateSizeInBytes = 100 * 1024;
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
            return View(new CreateCertificateViewModel());
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCertificateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _store.FindByName(model.Name) != null)
            {
                ModelState.AddModelError("Name", "Name already in use");
                return View(model);
            }

            if (model.File.Length > MaximumCertificateSizeInBytes)
            {
                ModelState.AddModelError("File", $"Certificate file too large (limited to {MaximumCertificateSizeInBytes / 1024} kB");
                return View(model);
            }

            X509Certificate2 certificate;
            try
            {
                using (var stream = model.File.OpenReadStream())
                    certificate = Certificate.FromStream(stream, model.Password);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", $"Invalid certificate file: {ex.Message}");
                return View(model);
            }
            
            await _store.Create(new Domain.Entities.Certificate
            {
                Name = model.Name,
                Data = certificate.RawData,
                OriginalFileName = model.File.FileName,
                Subject = certificate.Subject,
                ValidFrom = certificate.NotBefore,
                ValidTo = certificate.NotAfter
            });

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var certificate = await _store.FindById(id);
            return View(new UpdateCertificateViewModel
            {
                Id = certificate.Id,
                Name = certificate.Name
            });
        }

        [HttpPost]
        [Route("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, UpdateCertificateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Id != id) throw new ArgumentException("Invalid Id in model");

            var modelWithSameName = await _store.FindByName(model.Name);
            if (modelWithSameName != null && modelWithSameName.Id != id)
            {
                ModelState.AddModelError("Name", "Name already in use");
                return View(model);
            }

            X509Certificate2 certificate = null;
            if (model.File != null)
            {
                if (model.File.Length > MaximumCertificateSizeInBytes)
                {
                    ModelState.AddModelError("File", $"Certificate file too large (limited to {MaximumCertificateSizeInBytes / 1024} kB");
                    return View(model);
                }

                try
                {
                    using (var stream = model.File.OpenReadStream())
                        certificate = Certificate.FromStream(stream, model.Password);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("File", $"Invalid certificate file: {ex.Message}");
                    return View(model);
                }
            }

            await _store.Update(id, x =>
            {
                x.Name = model.Name;
                if (certificate != null)
                {
                    x.Data = certificate.RawData;
                    x.OriginalFileName = model.File.FileName;
                    x.Subject = certificate.Subject;
                    x.ValidFrom = certificate.NotBefore;
                    x.ValidTo = certificate.NotAfter;
                }
            });

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        [Route("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var certificate = await _store.FindById(id);
            if (certificate == null) return NotFound();
            return View(new ConfirmDeleteCertificateViewModel { Id = id, Name = certificate.Name });
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
