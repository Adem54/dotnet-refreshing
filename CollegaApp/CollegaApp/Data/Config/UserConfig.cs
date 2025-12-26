using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegaApp.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //ONModelCreating de boyle idi modelBuilder.Entity<User>().ToTable("Users");
            builder.ToTable("Users");//Table name is Users
            //primary key atamasi yapalim bu tablo iciin
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.UserName).IsRequired();//parameter is true by default..
            builder.Property(n => n.Password).IsRequired();
            builder.Property(n => n.PasswordSalt).IsRequired();
            builder.Property(n => n.IsDeleted).IsRequired();
            builder.Property(n => n.IsActive).IsRequired();
            builder.Property(n => n.CreatedAt).IsRequired();
            builder.Property(n => n.ModifiedAt).IsRequired();
            //builder.Property(n => n.UserName).HasMaxLength(250);
            //builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");//Bu sql icine gidecek..ondan dolayi boyle olmali...
            //builder.Property(m => m.ModifiedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
