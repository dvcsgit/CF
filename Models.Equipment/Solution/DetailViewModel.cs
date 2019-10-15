using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.Solution
{
    public class DetailViewModel
    {
        public string SolutionId { get; set; }

        [Display(Name ="SId",ResourceType =typeof(Resources.Resource))]
        public string SId { get; set; }

        [Display(Name ="SolutionName",ResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name ="SolutionType",ResourceType =typeof(Resources.Resource))]
        public string SolutionType { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name= "ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public DetailViewModel()
        {
            Permission = Define.EnumOrganizationPermission.None;
        }
    }
}
