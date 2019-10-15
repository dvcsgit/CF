using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models.Maintenance.Solution
{
    public class EditFormModel
    {
        public string SolutionId { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name ="ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> SolutionTypes { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            SolutionTypes = new List<SelectListItem>();
        }
    }
}
