using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models.Maintenance.Material
{
    public class CreateFormModel
    {
        public string OrganizationId { get; set; }
        
        [Display(Name = "ParentOrganizationName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> MaterialTypeSelectItems { get; set; }

        public List<MaterialSpecificationModel> MaterialSpecificationModels { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            MaterialTypeSelectItems = new List<SelectListItem>();
            MaterialSpecificationModels = new List<MaterialSpecificationModel>();
        }
    }
}
