using System;
using System.ComponentModel.DataAnnotations;

namespace Idento.Web.Models
{
    public class ConfirmDeleteAccountViewModel
    {
        public Guid Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}