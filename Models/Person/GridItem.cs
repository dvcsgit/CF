using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.Person
{
    public class GridItem
    {
        public string OrganizationName { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsMobilePerson { get; set; }

        public List<string> RoleNameList { get; set; }

        public string Roles
        {
            get
            {
                if (RoleNameList != null && RoleNameList.Count > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var roleId in RoleNameList)
                    {
                        sb.Append(roleId);
                        sb.Append("、");
                    }

                    sb.Remove(sb.Length - 1, 1);

                    return sb.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public GridItem()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
