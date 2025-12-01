namespace CollegaApp.Models
{
    //User validate edilip dogrulandiktan sonra boyle bir kullanici var ve passwordde ok o zaman, da artik token urettikten sonra response icin kullanilacak dto
    public class LoginResponseDTO
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
