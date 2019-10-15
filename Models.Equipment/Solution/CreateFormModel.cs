using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models.Maintenance.Solution
{
    public class CreateFormModel
    {
        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> SolutionTypes { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            SolutionTypes = new List<SelectListItem>();
        }
    }
}
