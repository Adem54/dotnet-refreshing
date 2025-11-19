namespace CollegaApp.Data.Repostory
{
    //Simdi isimlere dikkat edelim...biz 
    public interface IStudentRepostory:IEntityRepostory<Student>
    {
        //Task<List<Student>> GetAllAsync();
        //Task<Student?> GetByIdAsync(int id, bool isNoTracking= false);
        //Task<Student?> GetByNameAsync(string name);
        //Task<Student?> GetByEmailAsync(string email);
        //Task<int> CreateAsync(Student student);
        //Task<int> UpdateAsync(Student student);//
        //Task<bool> DeleteAsync(Student studentToDelete);

        //Bu yoruma aldgimz methodlarin hepsi zaten artik common repostory den gelecek buraya..Biz IEntityRepostory yi Student tablosu icin StudentReposotry icin implemente ediyoruz kullanabilmek icin

        //Add the specific signature for Student...and also implement to the common repostory  
        Task<List<Student>> GetStudentsByFeeStatusAsync(int feeStatus);
    }
}
