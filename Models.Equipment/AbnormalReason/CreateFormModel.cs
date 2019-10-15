using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models.Maintenance.AbnormalReason
{
    public class CreateFormModel
    {
        public string OrganizationId { get; set; }

        [Display(Name ="ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> Types { get; set; }

        public List<SolutionModel> SolutionModels { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            Types = new List<SelectListItem>();
            SolutionModels = new List<SolutionModel>();
        }
    }
}
