using System.Collections.Generic;

namespace CF.Models
{
    public class WebFunction
    {
        public WebFunction()
        {
            //WebPermissions = new HashSet<WebPermission>();
            WebFunctionNames = new HashSet<WebFunctionName>();
        }

        public string WebFunctionId { get; set; }        

        public virtual ICollection<WebFunctionName> WebFunctionNames { get; set; }
        //public virtual ICollection<WebPermission> WebPermissions { get; set; }
        public virtual ICollection<RolePermissionFunction> WebPermissionWebFunctions { get; set; }//Many to many relationship middle table
    }
}
