using Utility;

namespace Models.Maintenance.MSpecification
{
    public class GridItem
    {
        public string MaterialSpecificationId { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string MaterialType { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
