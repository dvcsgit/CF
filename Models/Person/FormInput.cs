using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Person
{
    public class FormInput
    {
        [Display(Name = "PId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "PIdRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Id { get; set; }

        [Display(Name = "PersonName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "PersonNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Resources.Resource))]
        public string Title { get; set; }

        [Display(Name = "EMail", ResourceType = typeof(Resources.Resource))]
        public string EMail { get; set; }

        //[Display(Name = "UID", ResourceType = typeof(Resources.Resource))]
        //public string UID { get; set; }

        [Display(Name = "IsMobilePerson", ResourceType = typeof(Resources.Resource))]
        public bool IsMobilePerson { get; set; }

        [Display(Name = "Role", ResourceType = typeof(Resources.Resource))]
        public string RoleIdsString { get; set; }

        public List<string> RoleIds
        {
            get
            {
                var roleIds = new List<string>();

                if (!string.IsNullOrEmpty(RoleIdsString))
                {
                    roleIds = JsonConvert.DeserializeObject<List<string>>(RoleIdsString);
                }

                return roleIds;
            }
        }
    }
}
