namespace CollegaApp.Data
{
    public class RolePrivilege
    {
        public int Id { get; set; }
        public string RolePrivilegeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; } //LastUpdatedAt de olablir
    }
    //One to many=>One role can have multiple role privileges
}
