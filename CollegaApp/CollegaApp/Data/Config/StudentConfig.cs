using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegaApp.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            //Burdaki builder DbContextimiz icindeki OnMOdelCreating deki modelBuilder.Entity<Student>() a karsilik geliyor unutmayalim...
            //ONModelCreating de boyle idi modelBuilder.Entity<Student>().ToTable("StudentsDb");
            builder.ToTable("Students");

            //primary key atamasi yapalim bu tablo iciin
            builder.HasKey(x => x.Id);//Hatirlayalim, Student modelimiz veya entitymizde [Key] tanimlamasi ile yapmistik(Data-annotation ile) bunu artik orda yapmamiza gerek yok burda yaptigimiz zaman
            builder.Property(x => x.Id).UseIdentityColumn();//Id kolonumuz artik Identtiy columndur yine biz bunu da Student entity veya modelinde attribute ile data-annotation ile yapmistik burda yaptigimz icin orda ona da gerek kalmamis oluyor([DatabaseGenerated(DatabaseGeneratedOption.Identity)])
            //Evet, UseIdentityColumn() pratikte [DatabaseGenerated(DatabaseGeneratedOption.Identity)] ile aynı davranışı (auto-increment) sağlar—hatta SQL Server için daha net.

            /*
               modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(n => n.Name).IsRequired();//parameter is true by default..
                entity.Property(n => n.Name).HasMaxLength(250);

                entity.Property(n => n.Address).IsRequired(false).HasMaxLength(500);
                //it is optional, not required, can be null

                entity.Property(n => n.Email).IsRequired().HasMaxLength(250);
                //Bu sekilde tek satir da da yazabiliriz
            });
             */

            builder.Property(n => n.Name).IsRequired();//parameter is true by default..
            builder.Property(n => n.Name).HasMaxLength(250);

            builder.Property(n => n.Address).IsRequired(false).HasMaxLength(500);
            //it is optional, not required, can be null
            builder.Property(n => n.Email).IsRequired().HasMaxLength(250);
            //Bu sekilde tek satir da da yazabiliriz

            /*
            * ONModelCreating de boyle idi 
              modelBuilder.Entity<Student>().HasData(
               new Student {Id=1, Name = "Ada", Email = "ada@uni.edu", Address="Porsgrunn",DOB= new DateTime(2025,10,30) },
               new Student {Id=2, Name = "Linus", Email = "linus@uni.edu", Address = "Skien", DOB= new DateTime(2025, 10, 29) });
            */
            builder.HasData(
                new Student { Id = 1, Name = "Ada2", Email = "ada2@uni.edu", Address = "Porsgrunn2", DOB = new DateTime(2025, 10, 30) },
                new Student { Id = 2, Name = "Linus2", Email = "linus2@uni.edu", Address = "Skien2", DOB = new DateTime(2025, 10, 29) });

            builder.HasOne(n => n.Department)
                .WithMany(n => n.Students)
                .HasForeignKey(n => n.DepartmentId)
                .HasConstraintName("FK_Students_Department");
        }
    }
}
