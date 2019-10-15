using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Models.Maintenance.CheckItem
{
    public class GridItem
    {
        public string CheckItemId { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationName { get; set; }

        public string Type { get; set; }

        public string CIId { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public string Display
        {
            get
            {
                if (!string.IsNullOrEmpty(Unit))
                {
                    return string.Format("{0}({1})", Name, Unit);
                }
                else
                {
                    return Name;
                }
            }
        }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
