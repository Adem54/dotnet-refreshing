using CollegaApp.Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CollegaApp.Data
{
    public class CollegeDBContext:DbContext
    {
        //DbContext is combination of Unitof work and repostory pattern

        //options demek connection string demektir, connection stringi bizim DbContextin constructor paramtresine gondermemiz gerekyor bunu da DbContextOptions i kullanarak asagidaki gibi yapiyoruz
        //I have simply created constructor for this context class. So in this constructor we will receive the sql server connection related details. We are passing those details as is to the base class, entityframework baseclass. HOw can we pass the parameter to baseclass, using the base keyword
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options):base(options)
        {
           
        }
        public DbSet<Student> Students { get; set; } = null!;

        public DbSet<Department> Depertments { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<RolePrivilege> RolePrivileges { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Studenttable-config 
            modelBuilder.ApplyConfiguration(new StudentConfig());

            //Table-2
            modelBuilder.ApplyConfiguration(new DepartmentConfig());

            //Table-3
            modelBuilder.ApplyConfiguration(new UserConfig());

            //Table-4
            modelBuilder.ApplyConfiguration(new RoleConfig());

            //Table-5
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfig());
        }

    }
}
