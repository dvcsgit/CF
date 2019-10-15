using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utility;

namespace Models.Person
{
    public class DetailViewModel
    {
        public Define.EnumOrganizationPermission Permission { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "PId", ResourceType = typeof(Resources.Resource))]
        public string PId { get; set; }

        [Display(Name = "ParentOrganization", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Resources.Resource))]
        public string Title { get; set; }

        [Display(Name = "PersonName", ResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        //[Display(Name = "UID", ResourceType = typeof(Resources.Resource))]
        //public string UID { get; set; }

        [Display(Name = "EMail", ResourceType = typeof(Resources.Resource))]
        public string EMail { get; set; }

        [Display(Name = "IsMobilePerson", ResourceType = typeof(Resources.Resource))]
        public bool IsMobilePerson { get; set; }

        public List<string> RoleNames { get; set; }

        [Display(Name = "Role", ResourceType = typeof(Resources.Resource))]
        public string RoleNamesString
        {
            get
            {
                if (RoleNames != null && RoleNames.Count > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var role in this.RoleNames)
                    {
                        sb.Append(role);

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

        public DetailViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            RoleNames = new List<string>();
        }
    }
}
