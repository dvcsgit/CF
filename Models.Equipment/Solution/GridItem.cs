using Utility;

namespace Models.Maintenance.Solution
{
    public class GridItem
    {
        public string SolutionId { get; set; }
        public string SId { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string SolutionType { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
