using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Maintenance.CheckItem
{
    public class EditFormModel
    {
        public string CheckItemId { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> TypeSelectListItems { get; set; }

        public List<FeelOptionModel> FeelOptionModels { get; set; }

        public List<AbnormalReasonModel> AbnormalReasonModels { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            TypeSelectListItems = new List<SelectListItem>();
            FeelOptionModels = new List<FeelOptionModel>();
            AbnormalReasonModels = new List<AbnormalReasonModel>();
        }
    }
}
