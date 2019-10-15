using Utility;

namespace Models.Maintenance.ESpecification
{
    public class GridItem
    {
        public string EquipmentSpecificationId { get; set; }
        public string Name { get; set; }
        public string EquipmentType { get; set; }
        public string OrganizationName { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
