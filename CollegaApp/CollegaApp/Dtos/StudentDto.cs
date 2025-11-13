namespace CollegaApp.Dtos
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DOB { get; set; } = DateTime.Now.ToString();
    }
}