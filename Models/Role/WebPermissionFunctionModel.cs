using System.Collections.Generic;

namespace Models.Role
{
    public class WebPermissionFunctionModel
    {
        public List<WebPermissionModel> WebPermissions { get; set; }

        public List<RoleWebPermissionFunctionModel> RoleWebPermissionFunctions { get; set; }

        public WebPermissionFunctionModel()
        {
            WebPermissions = new List<WebPermissionModel>();
            RoleWebPermissionFunctions = new List<RoleWebPermissionFunctionModel>();
        }
    }
}
