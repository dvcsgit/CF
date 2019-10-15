using System.Collections.Generic;

namespace Models.Role
{
    public class DetailViewModel
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public WebPermissionFunctionModel WebPermissionFunction { get; set; }

        public List<PersonModel> People { get; set; }

        public DetailViewModel()
        {
            WebPermissionFunction = new WebPermissionFunctionModel();
            People = new List<PersonModel>();
        }
    }
}
