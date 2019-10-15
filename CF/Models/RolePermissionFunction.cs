namespace CF.Models
{
    public class RolePermissionFunction
    {
        public string WebPermissionId { get; set; }
        public virtual WebPermission WebPermission { get; set; }

        public string WebFunctionId { get; set; }
        public virtual WebFunction WebFunction { get; set; }
        //One to many relationship with Role.
        public string RoleId { get; set; }
        public virtual Role Role { get; set; }        
    }
}
