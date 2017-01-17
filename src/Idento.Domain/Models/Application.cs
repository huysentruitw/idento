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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idento.Domain.Models
{
    /// <summary>
    /// Enum of known OAuth2 grant types.
    /// </summary>
    /// <remarks>
    /// DO NOT change the enumeration name of these grant types as the name is stored in the database. You can change the Display Name of course.
    /// </remarks>
    public enum OAuth2GrantType
    {
        [Display(Name = "Unknown")]
        Unknown,
        [Display(Name = "Implicit")]
        Implicit,
        [Display(Name = "Implicit and client credentials")]
        ImplicitAndClientCredentials,
        [Display(Name = "Code")]
        Code,
        [Display(Name = "Code and client credentials")]
        CodeAndClientCredentials,
        [Display(Name = "Hybrid")]
        Hybrid,
        [Display(Name = "Hybrid and client credentials")]
        HybridAndClientCredentials,
        [Display(Name = "Client credentials")]
        ClientCredentials,
        [Display(Name = "Resource owner password credential")]
        ResourceOwnerPassword,
        [Display(Name = "Resource owner password and client credentials")]
        ResourceOwnerPasswordAndClientCredentials
    }

    [Table("Applications", Schema = "Security")]
    public class Application : ITenantChild
    {
        public Application()
        {
            Id = Guid.NewGuid();
            AccessTokenLifetimeInMinutes = 600;
            RequireConsent = true;
            GrantType = OAuth2GrantType.Implicit;
            RedirectUris = string.Empty;
        }

        [Key, Required]
        public Guid Id { get; set; }
        [Required]
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        [MaxLength(256), Required]
        public string ClientId { get; set; }
        [MaxLength(256), Required]
        public string ClientSecret { get; set; }
        [MaxLength(256), Required]
        public string DisplayName { get; set; }
        [Required]
        public bool Enabled { get; set; }
        public string Uri { get; set; }
        [Required]
        public string RedirectUris { get; set; }
        [Required]
        public int AccessTokenLifetimeInMinutes { get; set; }
        [Required]
        public bool RequireConsent { get; set; }
        public string AllowedScopes { get; set; }
        public string AllowedCorsOrigins { get; set; }
        public string AllowedExternalLoginProviders { get; set; }
        [MaxLength(64), Required]
        [Obsolete("This value is only used to store the name of the enum value. Use Flow instead.", true)]
        internal string GrantTypeName
        {
            get { return GrantType.ToString(); }
            set
            {
                OAuth2GrantType grantType;
                this.GrantType = Enum.TryParse(value, out grantType) ? grantType : OAuth2GrantType.Unknown;
            }
        }
        [NotMapped]
        public OAuth2GrantType GrantType { get; set; }
    }
}