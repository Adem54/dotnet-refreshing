using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegaApp.Data.Config
{
    public class RolePrivilegeConfig : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            //ONModelCreating de boyle idi modelBuilder.Entity<User>().ToTable("Roles");
            builder.ToTable("RolePrivileges");//Table name is Roles
            //primary key atamasi yapalim bu tablo iciin
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.RolePrivilegeName).HasMaxLength(250).IsRequired();
            builder.Property(n => n.Description);
            builder.Property(n => n.IsDeleted).IsRequired();
            builder.Property(n => n.IsActive).IsRequired();
            builder.Property(n => n.CreatedAt).IsRequired();
            //builder.Property(n => n.ModifiedAt).IsRequired();
            //builder.Property(n => n.UserName).HasMaxLength(250);
            //builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");//Bu sql icine gidecek..ondan dolayi boyle olmali...
            //builder.Property(m => m.ModifiedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(n => n.Role)
              .WithMany(n => n.RolePrivileges)
              .HasForeignKey(n => n.RoleId)
              .HasConstraintName("FK_RolePreview_Role");

            //Sadece RoleId uzerinden yapmak istese idik bu sekilde yapardik
            //builder.HasOne<Role>()
            //    .WithMany()
            //    .HasForeignKey(rp => rp.RoleId);
        }
    }
}
