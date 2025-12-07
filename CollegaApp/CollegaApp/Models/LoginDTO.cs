using System.ComponentModel.DataAnnotations;

namespace CollegaApp.Models
{
    public class LoginDTO
    {
        //LoginDTO bnu once kullanici bilgilerini validate etmek icn DB de var mi yok mu onun icin kullanilir
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string? Password { get; set; }
    }
}
