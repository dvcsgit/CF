using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.Checkpoint
{
    public class DetailViewModel
    {
        public string CheckpointId { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationUniqueID { get; set; }

        [Display(Name = "ParentOrganizationFullName", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        [Display(Name = "CId", ResourceType = typeof(Resources.Resource))]
        public string CId { get; set; }

        [Display(Name = "CheckpointName", ResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = "TagId", ResourceType = typeof(Resources.Resource))]
        public string TagId { get; set; }

        [Display(Name = "IsFeelItemDefaultNormal", ResourceType = typeof(Resources.Resource))]
        public bool IsFeelItemDefaultNormal { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Resources.Resource))]
        public string Remark { get; set; }

        public List<CheckItemModel> CheckItemModels { get; set; }

        public DetailViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            CheckItemModels = new List<CheckItemModel>();
        }
    }
}
