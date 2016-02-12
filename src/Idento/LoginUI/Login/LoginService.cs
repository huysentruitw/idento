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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Core;
using Idento.Domain.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;

namespace Idento.LoginUI.Login
{
    public class LoginService
    {
        private UserManager<User> userManager;
        private IHttpContextAccessor httpContextAccessor;

        public LoginService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<SignInResult> PasswordSignIn(string userName, string password, bool rememberMe, bool lockoutOnFailure)
        {
            var user = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
            if (user == null)
                return SignInResult.Failed;

            if (!await userManager.CheckPasswordAsync(user, password))
                return SignInResult.Failed;

            var ci = CreateClaimsIdentity(user, "password");
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, cp);
            return SignInResult.Success;
        }

        private HttpContext HttpContext
        {
            get { return httpContextAccessor.HttpContext; }
        }

        private ClaimsIdentity CreateClaimsIdentity(User user, string authenticationType)
        {
            var name = user.Claims.FirstOrDefault(x => x.ClaimType.Equals(Constants.ClaimTypes.Name))?.ClaimValue
                ?? user.Claims.FirstOrDefault(x => x.ClaimType.Equals(ClaimTypes.Name))?.ClaimValue
                ?? user.UserName
                ?? user.Email;

            var claims = new List<Claim>(new[]
            {
                new Claim(Constants.ClaimTypes.Subject, user.Id.ToString("N")),
                new Claim(Constants.ClaimTypes.Name, name),
                new Claim(Constants.ClaimTypes.IdentityProvider, "idento"),
                new Claim(Constants.ClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
            });

            return new ClaimsIdentity(claims, authenticationType, Constants.ClaimTypes.Name, Constants.ClaimTypes.Role);
        }
    }
}
