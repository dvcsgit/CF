using Utility;

namespace Models.Maintenance.Material
{
    public class GridItem
    {
        public string MaterialId { get; set; }
        public string OrganizationName { get; set; }
        public string MaterialType { get; set; }
        public string MId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
