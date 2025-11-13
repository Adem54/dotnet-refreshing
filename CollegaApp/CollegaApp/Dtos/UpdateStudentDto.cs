using System.ComponentModel.DataAnnotations;

namespace CollegaApp.Dtos
{
    public record class UpdateStudentDto
    {
        //public int Id { get; set; }Burasi yanlis cunku gidip biz _dbContext.Students.Entry(existingStudent).CurrentValues.SetValues(updateStudentDto) denildiginde, existingStudent update de id ayri gonderilyor ve o id uzernden entiyt bulunur ve sonra da iste bu islemde SetValues existingStudent ile updateStudentDto nun eselesen tum kolonlarini updateStudentDto dan existingStudent e kopyalamaya calisir...id de var updateStudentDto da bu sakincalidir...dikkat edelim...id update edilmez...
        [Required] //Name is required
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public DateTime DOB { get; set; }
    }
}
