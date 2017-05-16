using System.ComponentModel.DataAnnotations;

namespace Idento.Web.Models
{
    public class CreateOrUpdateAccountViewModel
    {
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}