using System.Collections.Generic;

namespace Models.Role
{
    public class EditFormModel
    {
        public string AncestorOrganizationId { get; set; }

        public string RoleId { get; set; }

        public FormInput FormInput { get; set; }

        public WebPermissionFunctionModel WebPermissionFunction { get; set; }

        public List<PersonModel> People { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            WebPermissionFunction = new WebPermissionFunctionModel();
            People = new List<PersonModel>();
        }
    }
}
