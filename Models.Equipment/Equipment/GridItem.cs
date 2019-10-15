using Utility;

namespace Models.Maintenance.Equipment
{
    public class GridItem
    {
        public string EquipmentId { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationName { get; set; }

        public string EId { get; set; }

        public string Name { get; set; }

        public string MaintenanceOrganization { get; set; }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
