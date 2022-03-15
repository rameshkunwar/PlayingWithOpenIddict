using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; } = "username";
        [Required]
        public string Password { get; set; } = "p@$$Wørd";
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
