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

        public int? DepartmentId { get; set; }
        //One student can belong to just 1 Department, not more
        public virtual Department? Department { get; set; }
        //If you want to hold the reference of the department we need to have the department Id in student table, for that we need to add a property of type int DepartmentId
        //DepartmantId and Deparment in Student entity, will be created as a foreign key..
        //Ayrica Deparment e DeparmentId yi nullable yaparak, Student in 1 tane Department i de olabilir ya da hic olmayadabilir demis oluyoruz
        //We have added to the required columns or references

    }


    public interface IEntity
    {
        int Id { get; set; }

         string Name { get; set; } 
         string Email { get; set; }
    }
}