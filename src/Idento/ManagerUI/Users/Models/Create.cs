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

using System.ComponentModel.DataAnnotations;

namespace Idento.ManagerUI.Users.Models
{
    public class Create
    {
        [MaxLength(256), Required]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [MaxLength(256), Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(256)]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [MaxLength(256)]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
