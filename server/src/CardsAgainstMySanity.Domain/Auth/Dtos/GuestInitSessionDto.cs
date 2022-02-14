using System.ComponentModel.DataAnnotations;

namespace CardsAgainstMySanity.Domain.Auth.Dtos
{
    public class GuestInitSessionDto
    {
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(1, ErrorMessage = "{0} must be at least {1} characters long.")]
        [MaxLength(24, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Username { get; set; }
    }
}