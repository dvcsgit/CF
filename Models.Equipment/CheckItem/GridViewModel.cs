using System.Collections.Generic;
using Utility;

namespace Models.Maintenance.CheckItem
{
    public class GridViewModel
    {
        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationId { get; set; }

        public string Type { get; set; }

        public string OrganizationName { get; set; }

        public string FullOrganizationName { get; set; }

        public List<GridItem> Items { get; set; }

        public GridViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            Items = new List<GridItem>();
        }
    }
}
