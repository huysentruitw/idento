using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Idento.Domain.Entities
{
    [Table("UserApplications", Schema = "Security")]
    public class UserApplications
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid ApplicationId { get; set; }
        public virtual Application Application { get; set; }
    }
}