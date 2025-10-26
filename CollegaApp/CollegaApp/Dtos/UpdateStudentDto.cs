using System.ComponentModel.DataAnnotations;

namespace CollegaApp.Dtos
{
    public record class UpdateStudentDto
    {
        public int Id { get; set; }
        [Required] //Name is required
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
