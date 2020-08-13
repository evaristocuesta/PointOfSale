using System.ComponentModel.DataAnnotations;

namespace PointOfSale.WebAPI.ViewModels.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
