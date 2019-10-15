using Utility;

namespace Models.Maintenance.AbnormalReason
{
    public class GridItem
    {
        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }

        public string AbnormalReasonId { get; set; }
        public string ARId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string OrganizationName { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }        
    }
}
