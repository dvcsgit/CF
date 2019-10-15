using System.Collections.Generic;

namespace Models.Role
{
    public class WebPermissionModel
    {
        public string WebPermissionId { get; set; }

        public Dictionary<string,string> PermissionName { get; set; }

        public List<WebFunctionModel> WebFunctions { get; set; }

        public List<WebPermissionModel> SubItems { get; set; }

        public WebPermissionModel()
        {
            PermissionName = new Dictionary<string, string>();
            WebFunctions = new List<WebFunctionModel>();
            SubItems = new List<WebPermissionModel>();
        }
    }
}
