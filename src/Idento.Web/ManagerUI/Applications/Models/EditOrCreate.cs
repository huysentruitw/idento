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

using System.ComponentModel.DataAnnotations;

namespace Idento.Web.ManagerUI.Applications.Models
{
    public class EditOrCreate
    {
        [MaxLength(256), Required]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; }
        
        [MaxLength(256), Required]
        [Display(Name = "Client secret")]
        public string ClientSecret { get; set; }

        [MaxLength(256), Required]
        [Display(Name = "Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Display(Name = "Application URL")]
#if !DEBUG
        [Uri] // Doesn't allow localhost
#endif
        public string Uri { get; set; }

        [Required]
        [Display(Name = "Allowed Redirect URLs", Description = "After the user authenticates we will only redirect to any of these URLs. You can specify multiple valid URLs by comma-separating them (typically to handle different environments like QA or testing).")]
        public string RedirectUris { get; set; }

        [Required]
        [Display(Name = "JWT Expiration (in minutes)")]
        [Range(1, 50000)]
        public int AccessTokenLifetimeInMinutes { get; set; }

        [Required]
        [Display(Name = "Require Consent")]
        public bool RequireConsent { get; set; }

        [Display(Name = "Allowed Scopes", Description = "Allowed scopes for this app. You can specify multiple values by comma- or space-separating them.")]
        public string AllowedScopes { get; set; }

        [Display(Name = "Allowed CORS Origins", Description = "Allowed Origins are URLs that will be allowed to make requests from JavaScript to IdentO API (typically used with CORS). By default, all your callback URLs will be allowed. This field allows you to enter other origins if you need to. You can specify multiple valid URLs by comma-separating them or one by line, and also use wildcards (e.g.: https://*.contoso.com).")]
        public string AllowedCorsOrigins { get; set; }

        [Display(Name = "Allowed External Login Providers", Description = "Allowed external login providers for this app. You can specify multiple values by comma- or space-separating them. Leave empty to allow all.")]
        public string AllowedExternalLoginProviders { get; set; }

        [Required]
        [Display(Name = "OAuth2 Grant Type")]
        public string GrantType { get; set; }
    }
}
