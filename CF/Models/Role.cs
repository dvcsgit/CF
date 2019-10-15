using System.Collections.Generic;

namespace CF.Models
{
    public class Role
    {
        public Role()
        {
            People = new HashSet<Person>();
            //WebPermissions = new HashSet<WebPermission>();            
            WebPermissionWebFunctions = new HashSet<RolePermissionFunction>();
        }
        
        public string RoleId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Person> People { get; set; }
        //public virtual ICollection<WebPermission> WebPermissions { get; set; }    
        public virtual ICollection<RolePermissionFunction> WebPermissionWebFunctions { get; set; }//One to many relationship.
    }
}
