using System.ComponentModel.DataAnnotations;

namespace Models.Authentication
{
    public class PasswordFormModel
    {
        [Display(Name ="OriginalPassword",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "OriginalPasswordRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string OriginalPassword { get; set; }

        [Display(Name ="NewPassword",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "NewPasswordRequired", ErrorMessageResourceType =typeof(Resources.Resource))]
        public string NewPassword { get; set; }

        [Display(Name ="RepeatPassword",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "RepeatPasswordRequired", ErrorMessageResourceType =typeof(Resources.Resource))]
        [Compare("NewPassword",ErrorMessageResourceName = "PasswordDifferent", ErrorMessageResourceType =typeof(Resources.Resource))]
        public string RepeatPassword { get; set; }
    }
}
