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
using IdentityServer4.Core;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using Idento.Domain.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;

namespace Idento.Core.IdentityServer.Services
{
    internal class UserService : IUserService
    {
        private const string SecurityStampClaimType = "security_stamp";

        private UserManager<User> userManager;
        private ILogger logger;

        public UserService(UserManager<User> userManager, ILogger<UserService> logger)
        {
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.userManager = userManager;
            this.logger = logger;
        }

        #region IUserService implementation

        public async Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            if (context.ExternalIdentity == null) throw new ArgumentNullException(nameof(context.ExternalIdentity));
            var user = await userManager.FindByLoginAsync(context.ExternalIdentity.Provider, context.ExternalIdentity.ProviderId);
            if (user == null)
            {
                logger.LogWarning($"Unknown user '{context.ExternalIdentity.ProviderId}' tries to login with external provider '{context.ExternalIdentity.Provider}'");
                return;
            }
            var claims = await GetClaimsFromAccountAsync(user);
            context.AuthenticateResult = new AuthenticateResult(
                user.Id.ToString(),
                await GetDisplayNameForAccountAsync(user),
                claims,
                authenticationMethod: Constants.AuthenticationMethods.External,
                identityProvider: context.ExternalIdentity.Provider);
        }

        public async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            if (!userManager.SupportsUserPassword)
            {
                logger.LogError($"User '{context.UserName}' tries to authenticate, but SupportsUserPassword is set to false on UserManager");
                return;
            }
            var user = await userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                logger.LogWarning($"User '{context.UserName}' tries to authenticate but was not found");
                return;
            }
            if (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user))
            {
                logger.LogWarning($"User '{context.UserName}' tries to authenticate but is locked out");
                return;
            }
            if (!await userManager.CheckPasswordAsync(user, context.Password))
            {
                if (userManager.SupportsUserLockout) await userManager.AccessFailedAsync(user);
                return;
            }
            if (userManager.SupportsUserLockout) await userManager.ResetAccessFailedCountAsync(user);
            var claims = await GetClaimsForAuthenticateResultAsync(user);
            context.AuthenticateResult = new AuthenticateResult(user.Id.ToString(), await GetDisplayNameForAccountAsync(user), claims);
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Subject == null) throw new ArgumentNullException(nameof(context.Subject));
            var userId = GetUserId(context.Subject);
            if (userId == null) throw new ArgumentException("No claim holing the user id");
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) throw new ArgumentException("Invalid subject identifier");
            var claims = await GetClaimsFromAccountAsync(user);
            if (context.RequestedClaimTypes?.Any() ?? false) claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type));
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;
            if (context.Subject == null) throw new ArgumentNullException(nameof(context.Subject));
            var userId = GetUserId(context.Subject);
            if (userId == null) throw new ArgumentException("No claim holing the user id");
            User user = await userManager.FindByIdAsync(userId);
            if (user == null) return;
            if (userManager.SupportsUserSecurityStamp)
            {
                var securityStamp = context.Subject.Claims.Where(x => x.Type == SecurityStampClaimType).Select(x => x.Value).SingleOrDefault();
                if (securityStamp != null && securityStamp != user.SecurityStamp) return;
            }
            context.IsActive = true;
        }

        public Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            return Task.FromResult<object>(null);
        }

        public Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            return Task.FromResult<object>(null);
        }

        public Task SignOutAsync(SignOutContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helpers

        private async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResultAsync(User user)
        {
            var claims = new List<Claim>();
            if (userManager.SupportsUserSecurityStamp)
            {
                string stamp = await userManager.GetSecurityStampAsync(user);
                if (!string.IsNullOrWhiteSpace(stamp))
                {
                    claims.Add(new Claim(SecurityStampClaimType, stamp));
                }
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
            {
                claims.AddRange(await userManager.GetClaimsAsync(user));
            }
            if (userManager.SupportsUserRole)
            {
                claims.AddRange((await userManager.GetRolesAsync(user)).Select(x => new Claim(Constants.ClaimTypes.Role, x)));
            }
            return await Task.FromResult(claims);
        }

        private async Task<string> GetDisplayNameForAccountAsync(User user)
        {
            var claims = await GetClaimsFromAccountAsync(user);
            return claims.Where(x => x.Type == Constants.ClaimTypes.Name).Select(x => x.Value).FirstOrDefault()
                ?? claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault()
                ?? user.UserName;
        }

        private string GetUserId(ClaimsPrincipal principal)
        {
            return principal.Claims.Where(x => x.Type == Constants.ClaimTypes.Subject).Select(x => x.Value).FirstOrDefault()
                ?? principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault();
        }

        #endregion
    }
}
