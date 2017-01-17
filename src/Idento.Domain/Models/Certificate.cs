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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Idento.Domain.Models
{
    /// <summary>
    /// Enum of known certificate purposes.
    /// </summary>
    /// <remarks>
    /// DO NOT change the enumeration name of these purposes as the name is stored in the database. You can change the Display Name of course.
    /// </remarks>
    public enum CertificatePurpose
    {
        [Display(Name = "Unknown")]
        Unknown,
        [Display(Name = "Primary signing certificate")]
        PrimarySigning,
    }

    [Table("Certificates", Schema = "Security")]
    public class Certificate : ITenantChild
    {
        public Certificate()
        {
            Id = Guid.NewGuid();
            Purpose = CertificatePurpose.Unknown;
        }

        [Key, Required]
        public Guid Id { get; set; }
        [Required]
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        [MaxLength(256), Required]
        public string DisplayName { get; set; }
        [Required]
        public byte[] Data { get; set; }
        [MaxLength(64), Required]
        [Obsolete("This value is only used to store the name of the enum value. Use Purpose instead.", true)]
        internal string PurposeName
        {
            get { return Purpose.ToString(); }
            set
            {
                CertificatePurpose purpose;
                this.Purpose = Enum.TryParse(value, out purpose) ? purpose : CertificatePurpose.Unknown;
            }
        }
        [NotMapped]
        public CertificatePurpose Purpose { get; set; }
    }
}