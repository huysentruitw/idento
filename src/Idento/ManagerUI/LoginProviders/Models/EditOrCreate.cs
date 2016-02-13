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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Idento.Domain.Models;
using Idento.Helpers;

namespace Idento.ManagerUI.LoginProviders.Models
{
    public class EditOrCreate : IValidatableObject
    {
        [Required]
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Provider")]
        public LoginProvider Provider { get; set; }

        [Display(Name = "OAuth2 Client Id")]
        public string OAuth2ClientId { get; set; }

        [Display(Name = "OAuth2 Client Secret")]
        public string OAuth2ClientSecret { get; set; }

        [Display(Name = "WS-Federation Metadata Address")]
        public string WsFederationMetadataAddress { get; set; }

        [Display(Name = "WS-Federation Realm")]
        public string WsFederationRealm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as EditOrCreate;
            if (model == null)
            {
                yield return new ValidationResult("Invalid model");
                yield break;
            }
            var groupName = EnumExtensions.GetDisplayGroupName(model.Provider);
            if (groupName == LoginProviderGroupNames.OAuth2)
            {
                if (string.IsNullOrWhiteSpace(model.OAuth2ClientId))
                    yield return new ValidationResult("OAuth2 Client Id is required");
                if (string.IsNullOrWhiteSpace(model.OAuth2ClientSecret))
                    yield return new ValidationResult("OAuth2 Client Secret is required");
            }
            else if (groupName == LoginProviderGroupNames.WsFederation)
            {
                if (string.IsNullOrWhiteSpace(model.WsFederationMetadataAddress))
                    yield return new ValidationResult("WS-Federation Metadata Address is required");
                if (string.IsNullOrWhiteSpace(model.WsFederationRealm))
                    yield return new ValidationResult("WS-Federation Realm is required");
            }
        }
    }
}