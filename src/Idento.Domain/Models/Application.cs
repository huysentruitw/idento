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
    /// Enum of known OAuth2 flows.
    /// </summary>
    /// <remarks>
    /// DO NOT change the enumeration name of these providers as the name is stored in the database. You can change the Display Name of course.
    /// </remarks>
    public enum OAuth2Flow
    {
        [Display(Name = "Unknown")]
        Unknown,
        [Display(Name = "Authorization code")]
        AuthorizationCode,
        [Display(Name = "Implicit")]
        Implicit,
        [Display(Name = "Hybrid")]
        Hybrid,
        [Display(Name = "Client credentials")]
        ClientCredentials,
        [Display(Name = "Resource owner password credential")]
        ResourceOwner
    }

    [Table("Applications", Schema = "Security")]
    public class Application
    {
        public Application()
        {
            this.Id = Guid.NewGuid();
            this.RequireConsent = true;
            this.Flow = OAuth2Flow.Implicit;
        }

        [Key, Required]
        public Guid Id { get; set; }
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
        [Required]
        public bool AllowAllScopes { get; set; }
        public string AllowedCorsOrigins { get; set; }
        public string AllowedExternalLoginProviders { get; set; }
        [MaxLength(64), Required]
        [Obsolete("This value is only used to store the name of the enum value. Use Flow instead.", true)]
        internal string FlowName
        {
            get { return Flow.ToString(); }
            set
            {
                OAuth2Flow flow;
                this.Flow = Enum.TryParse(value, out flow) ? flow : OAuth2Flow.Unknown;
            }
        }
        [NotMapped]
        public OAuth2Flow Flow { get; set; }
    }
}
