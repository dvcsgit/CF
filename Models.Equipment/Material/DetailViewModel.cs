using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.Material
{
    public class DetailViewModel
    {
        public string MaterialId { get; set; }
        public string OrganizationId { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }
        public string Extension { get; set; }
        public string Photo
        {
            get
            {
                if (!string.IsNullOrEmpty(Extension))
                {
                    return string.Format("{0}.{1}", MId, Extension);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        
        [Display(Name="ParentOrganizationName",ResourceType=typeof(Resources.Resource))]
        public string ParentOrganizationName { get; set; }

        [Display(Name ="MaterialType",ResourceType =typeof(Resources.Resource))]
        public string MaterialType { get; set; }

        [Display(Name ="MId",ResourceType =typeof(Resources.Resource))]
        public string MId { get; set; }

        [Display(Name = "MaterialName", ResourceType = typeof(Resources.Resource))]
        public string MaterialName { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resources.Resource))]
        public int Quantity { get; set; }

        public List<MaterialSpecificationModel> MaterialSpecificationModels { get; set; }

        public List<EquipmentModel> EquipmentModels { get; set; }

        public DetailViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            MaterialSpecificationModels = new List<MaterialSpecificationModel>();
            EquipmentModels = new List<EquipmentModel>();
        }
    }
}
