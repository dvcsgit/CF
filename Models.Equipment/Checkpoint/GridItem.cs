using Utility;

namespace Models.Maintenance.Checkpoint
{
    public class GridItem
    {
        public string ChcekpointId { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationName { get; set; }

        public string CId { get; set; }

        public string Name { get; set; }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
