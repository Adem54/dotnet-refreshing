namespace CollegaApp.Models
{
    //In memory repostory
    public static class CollegeRepostory
    {
        public static List<Student> Students  { get; set; } = new List<Student>(){
                new() { Id = 1, Name="Adem", Email = "adem@gmail.com", Address="Skien" },
                new() { Id = 2, Name="Zehra", Email = "zehra@gmail.com", Address="Skien" },
                new Student { Id = 3, Name="Zeynep", Email = "zeynep@gmail.com", Address="Skien" },
                new() { Id = 4, Name="Leyla", Email = "leyla@gmail.com", Address="Sorgun" }
            };

    }
}
