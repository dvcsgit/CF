using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Maintenance.ESpecification
{
    public class EditFormModel
    {
        public string EquipmentSpecificationId { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> EquipmentTypeSelectItems { get; set; }

        public List<EquipmentSpecificationOptionModel> EquipmentSpecificationOptionModels { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            EquipmentTypeSelectItems = new List<SelectListItem>();
            EquipmentSpecificationOptionModels = new List<EquipmentSpecificationOptionModel>();
        }
    }
}
