using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Equipment
{
    public class EditFormModel
    {
        public string EquipmentId { get; set; }

        public string AncestorOrganizationId { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganization", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public string MaintenanceOId { get; set; }

        public string MaintenanceOrganizationName { get; set; }

        public string Extension { get; set; }

        public string Photo
        {
            get
            {
                if (!string.IsNullOrEmpty(Extension))
                {
                    return string.Format("{0}.{1}", EquipmentId, Extension);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string MaintenanceOrganization
        {
            get
            {
                if (!string.IsNullOrEmpty(MaintenanceOrganizationName))
                {
                    return string.Format("{0}/{1}", MaintenanceOId, MaintenanceOrganizationName);
                }
                else
                {
                    return MaintenanceOId;
                }
            }
        }

        public FormInput FormInput { get; set; }

        public List<ESpecificationModel> ESpecificationModels { get; set; }

        public List<MaterialModel> MaterialModels { get; set; }

        public List<EPartModel> EPartModels { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            ESpecificationModels = new List<ESpecificationModel>();
            MaterialModels = new List<MaterialModel>();
            EPartModels = new List<EPartModel>();
        }
    }
}
