using System.Collections.Generic;
using Utility;

namespace Models.Maintenance.AbnormalReason
{
    public class GridViewModel
    {
        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string FullOrganizationName { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string Type { get; set; }

        public List<GridItem> GridItems { get; set; }

        public GridViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            GridItems = new List<GridItem>();
        }
    }
}
