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
    public static class LoginProviderGroupNames
    {
        public const string OAuth2 = "OAuth2";
        public const string WsFederation = "WsFederation";
    }

    /// <summary>
    /// Enum of known external login providers.
    /// </summary>
    /// <remarks>
    /// DO NOT change the enumeration name of these providers as the name is stored in the database. You can change the Display Name of course.
    /// </remarks>
    public enum LoginProvider
    {
        [Display(Name = "Unknown")]
        Unknown,
        [Display(Name = "Facebook", GroupName = LoginProviderGroupNames.OAuth2)]
        Facebook,
        [Display(Name = "Google", GroupName = LoginProviderGroupNames.OAuth2)]
        Google,
        [Display(Name = "LinkedIn", GroupName = LoginProviderGroupNames.OAuth2)]
        LinkedIn,
        [Display(Name = "Microsoft Account", GroupName = LoginProviderGroupNames.OAuth2)]
        MicrosoftAccount,
        [Display(Name = "Twitter", GroupName = LoginProviderGroupNames.OAuth2)]
        Twitter,
        [Display(Name = "WS-Federation", GroupName = LoginProviderGroupNames.WsFederation)]
        WsFederation
    }

    [Table("ExternalLoginProviders", Schema = "Security")]
    public class ExternalLoginProvider
    {
        public ExternalLoginProvider()
        {
            Id = Guid.NewGuid();
        }

        [Key, Required]
        public Guid Id { get; set; }
        [MaxLength(256), Required]
        public string Name { get; set; }
        public bool Enabled { get; set; }
        [MaxLength(64), Required]
        [Obsolete("This value is only used to store the name of the enum value. Use Provider instead.", true)]
        internal string ProviderName
        {
            get { return Provider.ToString(); }
            set
            {
                LoginProvider provider;
                Provider = Enum.TryParse(value, out provider) ? provider : LoginProvider.Unknown;
            }
        }
        [MaxLength(256)]
        public string OAuth2ClientId { get; set; }
        [MaxLength(256)]
        public string OAuth2ClientSecret { get; set; }
        [MaxLength(1024)]
        public string WsFederationMetadataAddress { get; set; }
        [MaxLength(256)]
        public string WsFederationRealm { get; set; }
        [NotMapped]
        public LoginProvider Provider { get; set; }
    }
}
