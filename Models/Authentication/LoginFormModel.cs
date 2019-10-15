using System.ComponentModel.DataAnnotations;
using Resources;

namespace Models.Authentication
{
    public class LoginFormModel
    {
        [Display(Name = "PId",ResourceType =typeof(Resource))]
        [Required(ErrorMessageResourceName = "PIdRequired",ErrorMessageResourceType =typeof(Resource))]
        public string LoginId { get; set; }

        [Display(Name ="Password",ResourceType =typeof(Resource))]
        [Required(ErrorMessageResourceName ="PasswordRequired",ErrorMessageResourceType =typeof(Resource))]
        public string Password { get; set; }
    }
}