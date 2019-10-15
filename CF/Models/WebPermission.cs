using System;
using System.Collections.Generic;

namespace CF.Models
{
    public class WebPermission
    {
        public WebPermission()
        {
            //Roles = new HashSet<Role>();
            WebPermissionNames = new HashSet<WebPermissionName>();
            //WebFunctions = new HashSet<WebFunction>();
        }
        public string WebPermissionId { get; set; }
        public string ParentId { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public int Seq { get; set; }
        public bool IsEnabled { get; set; }
       
        public virtual ICollection<WebPermissionName> WebPermissionNames { get; set; }
        //public virtual ICollection<WebFunction> WebFunctions { get; set; }
        //public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<RolePermissionFunction> WebPermissionWebFunctions { get; set; }//Many to many relationship middle table
    }
}
