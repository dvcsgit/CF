using System.Collections.Generic;
using Utility;

namespace Models.Maintenance.ESpecification
{
    public class GridViewModel
    {
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string FullOrganizationName { get; set; }
        public string EquipmentType { get; set; }
        public Define.EnumOrganizationPermission Permission{ get; set; }
        public List<GridItem> Items { get; set; }

        public GridViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            Items = new List<GridItem>();
        }
    }
}
