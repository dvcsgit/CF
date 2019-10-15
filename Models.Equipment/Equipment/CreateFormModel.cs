using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Maintenance.Equipment
{
    public class CreateFormModel
    {
        public string EquipmentId { get; set; }

        public string AncestorOrganizationId { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name ="ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public string MaintenanceOrganizationId { get; set; }

        public string MaintenanceOrganizationName { get; set; }

        public string MaintenanceOrganization
        {
            get
            {
                if (!string.IsNullOrEmpty(MaintenanceOrganizationName))
                {
                    return string.Format("{0}/{1}", MaintenanceOrganizationId, MaintenanceOrganizationName);
                }
                else
                {
                    return MaintenanceOrganizationId;
                }
            }
        }

        public FormInput FormInput { get; set; }

        public List<ESpecificationModel> ESpecificationModels { get; set; }

        public List<MaterialModel> MaterialModels { get; set; }

        public List<EPartModel> EPartModels { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            ESpecificationModels = new List<ESpecificationModel>();
            MaterialModels = new List<MaterialModel>();
            EPartModels = new List<EPartModel>();
        }
    }
}
