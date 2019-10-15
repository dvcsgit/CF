using Models.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utility;

namespace Models.Organization
{
    public class DetailViewModel
    {
        public string OrganizationId { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        [Display(Name = "ParentOrganization", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        [Display(Name = "OId", ResourceType = typeof(Resources.Resource))]
        public string OId { get; set; }

        [Display(Name = "OrganizationName", ResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }        

        public List<PersonModel> ManagerList { get; set; }

        [Display(Name = "Manager", ResourceType = typeof(Resources.Resource))]
        public string Managers
        {
            get
            {
                if (ManagerList != null && ManagerList.Count > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var manager in ManagerList)
                    {
                        sb.Append(manager.Person);
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

        public List<string> EditableOrganizations { get; set; }

        public List<string> QueryableOrganizations { get; set; }

        public DetailViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
            //ManagerList = new List<PersonModel>();
            EditableOrganizations = new List<string>();
            QueryableOrganizations = new List<string>();
        }
    }
}
