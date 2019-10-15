using System.Collections.Generic;
using Utility;

namespace Models.Maintenance.Checkpoint
{
    public class GridViewModel
    {
        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string FullOrganizationName { get; set; }

        public List<GridItem> GridItems { get; set; }

        public GridViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            GridItems = new List<GridItem>();
        }
    }
}
