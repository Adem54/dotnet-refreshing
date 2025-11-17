using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegaApp.Data
{
    public class Student:IEntity 
    {
       // [Key]//primary key yap id yi yap diyoruz...key attribute tu bu is icin vardir..Bunu Config/StuentConfig.cs icnde yaptik burda gerek kalmadi
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Bunu vererek id yi automatik birsekilde db de olustr her rekord olusturuldugunda diyoruyz...Gerek kalmadi StudentConfig de yapiyoruz bunu da
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
    }


    public interface IEntity
    {
        int Id { get; set; }

         string Name { get; set; } 
         string Email { get; set; }
    }
}