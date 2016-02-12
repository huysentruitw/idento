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

using System.Threading.Tasks;
using IdentityServer4.Core;
using IdentityServer4.Core.Services;
using Idento.LoginUI.Login.Models;
using Idento.ManagerUI.Home;
using Idento.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;

namespace Idento.LoginUI.Login
{
    public class LoginController : Controller
    {
        private readonly LoginService loginService;
        private readonly SignInInteraction signInInteraction;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;

        public LoginController(
            LoginService loginService,
            SignInInteraction signInInteraction,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory)
        {
            this.loginService = loginService;
            this.signInInteraction = signInInteraction;
            this.emailSender = emailSender;
            this.logger = loggerFactory.CreateLogger<LoginController>();
        }

        [HttpGet(Constants.RoutePaths.Login, Name = "Login")]
        public async Task<IActionResult> Index(string id)
        {
            var vm = new LoginViewModel();
            if (id != null)
            {
                var request = await signInInteraction.GetRequestAsync(id);
                if (request != null)
                {
                    vm.Username = request.LoginHint;
                    vm.SignInId = id;
                }
            }
            return View(vm);
        }

        [HttpPost(Constants.RoutePaths.Login)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await loginService.PasswordSignIn(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: false);
                if (result == SignInResult.Success)
                {
                    logger.LogInformation(1, "User logged in.");

                    if (model.SignInId != null)
                    {
                        return new IdentityServerSignInResult(model.SignInId);
                    }

                    return RedirectToLocal(returnUrl);
                }
                // TODO
                //if (result.RequiresTwoFactor)
                //{
                //    //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberLogin });
                //}
                //if (result.IsLockedOut)
                //{
                //    logger.LogWarning(2, "User account locked out.");
                //    return View("Lockout");
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return View(model);
                //}
            }

            return View(model);
        }

        #region Helpers

        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? (IActionResult)Redirect(returnUrl)
                : RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion
    }
}
