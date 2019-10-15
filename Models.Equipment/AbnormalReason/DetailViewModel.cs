using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Models.Maintenance.AbnormalReason
{
    public class DetailViewModel
    {
        public string AbnormalReasonId { get; set; }

        [Display(Name ="ARId",ResourceType =typeof(Resources.Resource))]
        public string ARId { get; set; }

        [Display(Name ="AbnormalReasonName",ResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name ="AbnormalReasonType",ResourceType =typeof(Resources.Resource))]
        public string Type { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name ="ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public List<string> SolutionNames { get; set; }

        [Display(Name ="Solution",ResourceType =typeof(Resources.Resource))]
        public string SolutionNamesString
        {
            get
            {
                if (SolutionNames != null && SolutionNames.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach(var name in SolutionNames)
                    {
                        sb.Append(name);
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
            SolutionNames = new List<string>();
        }
    }
}
