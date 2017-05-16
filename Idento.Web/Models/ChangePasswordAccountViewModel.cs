using System;
using System.ComponentModel.DataAnnotations;

namespace Idento.Web.Models
{
    public class ChangePasswordAccountViewModel
    {
        [Required]
        public Guid Id { get; set; }

        public string Email { get; set; }

        [Required, MaxLength(256)]
        public string CurrentPassword { get; set; }

        [Required, MaxLength(256)]
        public string NewPassword { get; set; }
    }
}