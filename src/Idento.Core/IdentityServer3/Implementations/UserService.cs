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
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Idento.Domain.AspNetIdentity;
using Idento.Domain.Models;
using Microsoft.AspNet.Identity;
using Constants = IdentityServer3.Core.Constants;

namespace Idento.Core.IdentityServer3.Implementations
{
    internal class UserService : IUserService
    {
        private const string SecurityStampClaimType = "security_stamp";

        private UserManager userManager;

        public UserService(UserManager userManager)
        {
            if (userManager == null)
                throw new ArgumentNullException("userManager");

            this.userManager = userManager;
        }

        #region IUserService implementation

        public async Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            if (context.ExternalIdentity == null)
                throw new ArgumentNullException("ExternalIdentity");

            var userLoginInfo = new UserLoginInfo(context.ExternalIdentity.Provider, context.ExternalIdentity.ProviderId);
            var user = await this.userManager.FindAsync(userLoginInfo);
            if (user == null)
            {
                // TODO log.WarnFormat("Unknown user '{0}' tries to login with external provider '{1}'", externalUser.ProviderId, externalUser.Provider);
                return;
            }

            var claims = await this.GetClaimsFromAccountAsync(user);
            var name = await this.GetDisplayNameForAccountAsync(user);
            context.AuthenticateResult = new AuthenticateResult(
                subject: user.Id.ToString(),
                name: name,
                claims: claims,
                identityProvider: context.ExternalIdentity.Provider,
                authenticationMethod: Constants.AuthenticationMethods.External);
        }

        public async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            if (!this.userManager.SupportsUserPassword)
            {
                // log.ErrorFormat("User '{0}' tries to authenticate, but SupportsUserPassword is set to false on UserManager", context.UserName);
                return;
            }

            var user = await this.userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                // log.WarnFormat("User '{0}' tries to authenticate but was not found", context.UserName);
                return;
            }

            if (this.userManager.SupportsUserLockout && await this.userManager.IsLockedOutAsync(user.Id))
            {
                // log.WarnFormat("User '{0}' tries to authenticate but is locked out", context.UserName);
                return;
            }
            if (!await this.userManager.CheckPasswordAsync(user, context.Password))
            {
                if (this.userManager.SupportsUserLockout)
                {
                    // log.WarnFormat("User '{0}' tries to log in with wrong password '{1}'", context.UserName, context.Password);
                    await this.userManager.AccessFailedAsync(user.Id);
                }

                return;
            }

            if (this.userManager.SupportsUserLockout)
                await this.userManager.ResetAccessFailedCountAsync(user.Id);

            var claims = await this.GetClaimsForAuthenticateResultAsync(user);
            var name = await this.GetDisplayNameForAccountAsync(user);
            context.AuthenticateResult = new AuthenticateResult(
                subject: user.Id.ToString(),
                name: name,
                claims: claims);
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Subject == null)
                throw new ArgumentNullException("Subject");

            var userId = Guid.Parse(context.Subject.GetSubjectId());
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = await this.GetClaimsFromAccountAsync(user);
            if (context.RequestedClaimTypes != null && context.RequestedClaimTypes.Any())
                claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null)
                throw new ArgumentNullException("Subject");

            var userId = Guid.Parse(context.Subject.GetSubjectId());
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                context.IsActive = false;
                return;
            }

            if (this.userManager.SupportsUserSecurityStamp)
            {
                var securityStamp = context.Subject.Claims.Where(x => x.Type == SecurityStampClaimType).Select(x => x.Value).SingleOrDefault();
                if (securityStamp != null && securityStamp != user.SecurityStamp)
                {
                    context.IsActive = false;
                    return;
                }
            }

            context.IsActive = true;
        }

        public Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public Task SignOutAsync(SignOutContext context)
        {
            return Task.FromResult(0);
        }

        #endregion

        #region Helpers

        private async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResultAsync(User user)
        {
            var claims = new List<Claim>();
            if (userManager.SupportsUserSecurityStamp)
            {
                string stamp = await userManager.GetSecurityStampAsync(user.Id);
                if (!string.IsNullOrWhiteSpace(stamp))
                    claims.Add(new Claim(SecurityStampClaimType, stamp));
            }

            return claims;
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromAccountAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.ClaimTypes.Subject, user.Id.ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, user.UserName)
            };

            if (userManager.SupportsUserEmail && !string.IsNullOrWhiteSpace(user.Email))
            {
                claims.AddRange(new[]
                {
                    new Claim(Constants.ClaimTypes.Email, user.Email),
                    new Claim(Constants.ClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false")
                });
            }

            if (userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(Constants.ClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(Constants.ClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false")
                });
            }

            if (userManager.SupportsUserClaim)
                claims.AddRange(await userManager.GetClaimsAsync(user.Id));

            if (userManager.SupportsUserRole)
                claims.AddRange((await userManager.GetRolesAsync(user.Id)).Select(x => new Claim(Constants.ClaimTypes.Role, x)));

            return await Task.FromResult(claims);
        }

        private async Task<string> GetDisplayNameForAccountAsync(User user)
        {
            IEnumerable<Claim> claims = await GetClaimsFromAccountAsync(user);
            return claims.Where(x => x.Type == Constants.ClaimTypes.Name).Select(x => x.Value).FirstOrDefault()
                ?? claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault()
                ?? user.UserName;
        }

        #endregion
    }
}
