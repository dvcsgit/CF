using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.Equipment
{
    public class DetailViewModel
    {
        public string EquipmentId { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "ParentOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        [Display(Name = "MaintenanceOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string MaintenanceOrganizationFullName { get; set; }

        [Display(Name = "EId", ResourceType = typeof(Resources.Resource))]
        public string EId { get; set; }

        [Display(Name = "EquipmentName", ResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

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

        public List<ESpecificationModel> ESpecificationModels { get; set; }

        public List<MaterialModel>  MaterialModels { get; set; }

        public List<EPartModel> EPartModels { get; set; }

        public DetailViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            ESpecificationModels = new List<ESpecificationModel>();
            MaterialModels = new List<MaterialModel>();
            EPartModels = new List<EPartModel>();
        }
    }
}
