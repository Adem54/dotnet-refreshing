using Microsoft.EntityFrameworkCore;

namespace CollegaApp.Data.Repostory
{
    //Simdi isimlere dikkat edelim...biz 
    public interface IStudentRepostory:IEntityRepostory<Student>
    {
        private readonly CollegeDBContext _dbContext;

        public StudentRepostory(CollegeDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<int> CreateAsync(Student student)
        {
            await _dbContext.Students.AddAsync(student);//unit of work
            await _dbContext.SaveChangesAsync();//saved the db now...
            return student.Id;
        }

        public async Task<bool> DeleteAsync(Student studentToDelete)
        {
            _dbContext.Students.Remove(studentToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _dbContext.Students.AsNoTracking().ToListAsync();
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Student?> GetByIdAsync(int id, bool isNoTracking = false)
        {
            if (isNoTracking) return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id) ?? null;
            return await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id) ?? null;
        }

        public async Task<Student?> GetByNameAsync(string name)
        {
            // return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Name == name) ?? null;
            //return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Name.ToLower().Equals(name));
            //return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Name.ToLower().Contains(name.ToLower()));

            return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => EF.Functions.Collate(s.Name, "Norwegian_100_CI_AS") == name) ?? null;


            //        var list = await _dbContext.Students.AsNoTracking()
            //.Where(x => EF.Functions.Like(
            //    EF.Functions.Collate(x.Name, "Turkish_CI_AS"),
            //    name + "%"))
            //.ToListAsync();

        }

        public async Task<int> UpdateAsync(Student student)
        {

            _dbContext.Students.Update(student);
            await _dbContext.SaveChangesAsync();
            return student.Id;
        }


    }
}
