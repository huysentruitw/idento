using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Idento.Domain.Entities;

namespace Idento.Web.Models
{
    public class RegisterViewModel
    {
        public Guid? Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Application> AvailableApplications { get; set; }
        public ICollection<Guid> SelectedApplications { get; set; } = new List<Guid>();
    }
}