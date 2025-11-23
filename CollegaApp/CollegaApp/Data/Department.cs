namespace CollegaApp.Data
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        //1 department can have any number of students
        public virtual ICollection<Student> Students { get; set; } = null!;
    }
}
