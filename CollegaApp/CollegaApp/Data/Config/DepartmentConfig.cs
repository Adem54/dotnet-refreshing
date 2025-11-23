using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegaApp.Data.Config
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            //Burdaki builder DbContextimiz icindeki OnMOdelCreating deki modelBuilder.Entity<Student>() a karsilik geliyor unutmayalim...
            //ONModelCreating de boyle idi modelBuilder.Entity<Student>().ToTable("StudentsDb");
            builder.ToTable("Departments");

            //primary key atamasi yapalim bu tablo iciin
            builder.HasKey(x => x.Id);//Hatirlayalim, Student modelimiz veya entitymizde [Key] tanimlamasi ile yapmistik(Data-annotation ile) bunu artik orda yapmamiza gerek yok burda yaptigimiz zaman
            builder.Property(x => x.Id).UseIdentityColumn();//Id kolonumuz artik Identtiy columndur yine biz bunu da Student entity veya modelinde attribute ile data-annotation ile yapmistik burda yaptigimz icin orda ona da gerek kalmamis oluyor([DatabaseGenerated(DatabaseGeneratedOption.Identity)])
            //Evet, UseIdentityColumn() pratikte [DatabaseGenerated(DatabaseGeneratedOption.Identity)] ile aynı davranışı (auto-increment) sağlar—hatta SQL Server için daha net.

            builder.Property(n => n.DepartmentName).IsRequired().HasMaxLength(200);//parameter is true by default..
           // builder.Property(n => n.DepartmentName).HasMaxLength(250);

            builder.Property(n => n.Description).HasMaxLength(500).IsRequired(false);
            //it is optional, not required, can be null


            builder.HasData(
                new Department { Id = 1, DepartmentName = "ECE",Description= "ECE Department" },
                new Department { Id = 2, DepartmentName = "CSE" , Description = "ECE Department" });
        }

      
    }
}
