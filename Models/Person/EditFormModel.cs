using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Person
{
    public class EditFormModel
    {
        public string PId { get; set; }

        [Display(Name = "ParentOrganization", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<RoleModel> RoleModels { get; set; }

        public List<string> PersonRoleIds { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            RoleModels = new List<RoleModel>();
            PersonRoleIds = new List<string>();
        }
    }
}
