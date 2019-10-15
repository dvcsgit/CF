using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Maintenance.AbnormalReason
{
    public class EditFormModel
    {
        public string AbnormalReasonId { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> TypeSelectListItems { get; set; }

        public List<SolutionModel> SolutionModels { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            SolutionModels = new List<SolutionModel>();
            TypeSelectListItems = new List<SelectListItem>();
        }
    }
}
