using System.ComponentModel.DataAnnotations.Schema;

namespace CollegaApp.Data
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Bunu vererek id yi automatik birsekilde db de olustr her rekord olusturuldugunda diyoruyz...Gerek kalmadi StudentConfig de yapiyoruz bunu da
        public int Id { get; set; }
        public string UserName { get; set; }
     //   public byte[] PasswordHash { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public int UserType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime ModifiedAt { get; set; } //LastUpdatedAt de olablir
    }
}
