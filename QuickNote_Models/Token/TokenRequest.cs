using System.ComponentModel.DataAnnotations;

namespace QuickNote_Models.Token
{
    public class TokenRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}