namespace CollegaApp.Data
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; } //LastUpdatedAt de olablir
        public virtual ICollection<RolePrivilege> RolePrivileges { get; set; } = null!;
    }
}
