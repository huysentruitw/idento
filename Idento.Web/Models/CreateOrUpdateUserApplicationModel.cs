using System;
using System.Collections.Generic;
using Idento.Domain.Entities;

namespace Idento.Web.Models
{
    public class CreateOrUpdateUserApplicationModel
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public UserApplications UserApplications { get; set; }
        public ICollection<Application> AvailableApplications { get; set; }
        public ICollection<Guid> SelectedApplications { get; set; }
    }
}