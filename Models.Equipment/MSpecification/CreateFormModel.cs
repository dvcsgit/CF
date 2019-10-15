using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models.Maintenance.MSpecification
{
    public class CreateFormModel
    {
        public string OrganizationId { get; set; }

        [Display(Name ="ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }
        public FormInput FormInput { get; set; }
        public List<SelectListItem> MaterialTypeSelectItems { get; set; }
        public List<MaterialSpecificationOptionModel> MaterialSpecificationOptionModels { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            MaterialTypeSelectItems = new List<SelectListItem>();
            MaterialSpecificationOptionModels = new List<MaterialSpecificationOptionModel>();
        }
    }
}
