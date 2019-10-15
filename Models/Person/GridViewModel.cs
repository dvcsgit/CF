using Models.Shared;
using System.Collections.Generic;
using Utility;

namespace Models.Person
{
    public class GridViewModel
    {
        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string FullOrganizationName { get; set; }

        public List<GridItem> Items { get; set; }

        public List<MoveToTarget> MoveToTargets { get; set; }

        public GridViewModel()
        {
            MoveToTargets = new List<MoveToTarget>();
            Permission = Define.EnumOrganizationPermission.None;
            Items = new List<GridItem>();
        }
    }
}
