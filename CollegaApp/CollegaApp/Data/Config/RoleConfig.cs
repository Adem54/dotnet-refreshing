using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegaApp.Data.Config
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            //ONModelCreating de boyle idi modelBuilder.Entity<User>().ToTable("Roles");
            builder.ToTable("Roles");//Table name is Roles
            //primary key atamasi yapalim bu tablo iciin
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.RoleName).HasMaxLength(250).IsRequired();
            builder.Property(n => n.Description);
            builder.Property(n => n.IsDeleted).IsRequired();
            builder.Property(n => n.IsActive).IsRequired();
            builder.Property(n => n.CreatedAt).IsRequired();
            //builder.Property(n => n.ModifiedAt).IsRequired();
            //builder.Property(n => n.UserName).HasMaxLength(250);
            //builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");//Bu sql icine gidecek..ondan dolayi boyle olmali...
            //builder.Property(m => m.ModifiedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
