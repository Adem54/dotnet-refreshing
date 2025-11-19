
using Microsoft.EntityFrameworkCore;

namespace CollegaApp.Data.Repostory
{
    public class StudentRepostory : EntityRepostory<Student>,  IStudentRepostory
    {
        private readonly CollegeDBContext _dbContext;

        public StudentRepostory(CollegeDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public Task<List<Student>> GetStudentsByFeeStatusAsync(int feeStatus)
        {
            //Write code to return students having fee status pending
            return null;
        }

        /*
         * Simdi artik StudentController da, da IStudentRepostory kullanabilirz ve bu zaten Studente Spesifik olan bir repostory interface i oldugu icin hatirlayaagimz uzere yaninda ayria almiyordu..IEntityRepostory<Student> gibi..onun yerine direk IStudentRepostory olarak kullanabiiyorduk...
         
         //  private readonly IEntityRepostory<Student> _studentRepostory;
        private readonly IStudentRepostory _studentRepostory;
        //IEntityRepostory we are using application level repostory pattern..not studentlevel.

        public StudentController(ILogger<StudentController> logger, CollegeDBContext dbContext, IMapper mapper, IStudentRepostory studentRepostory)
         
         */
    }
}
