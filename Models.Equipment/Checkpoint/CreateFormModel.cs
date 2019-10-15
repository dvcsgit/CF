using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Checkpoint
{
    public class CreateFormModel
    {
        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<CheckItemModel> CheckItemModels { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            CheckItemModels = new List<CheckItemModel>();
        }
    }
}
