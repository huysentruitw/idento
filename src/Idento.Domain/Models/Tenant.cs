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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idento.Domain.Models
{
    [Table("Tenant", Schema = "Security")]
    public class Tenant
    {
        public Tenant()
        {
            Id = Guid.NewGuid();
        }

        [Key, Required]
        public Guid Id { get; set; }
        [MaxLength(256), Required]
        public string Name { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<ExternalLoginProvider> ExternalLoginProviders { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
