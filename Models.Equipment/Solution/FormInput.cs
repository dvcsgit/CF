using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Solution
{
    public class FormInput
    {
        [Display(Name ="SolutionType",ResourceType =typeof(Resources.Resource))]
        public string SId { get; set; }

        [Display(Name ="SId",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName ="SIdRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name ="SolutionName",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName ="SolutionNameRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string Type { get; set; }
    }
}
