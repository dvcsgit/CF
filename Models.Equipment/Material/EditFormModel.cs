using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Maintenance.Material
{
    public class EditFormModel
    {
        public string MaterialId { get; set; }
        public string OrganizationId { get; set; }
        public string Extension { get; set; }
        public string Photo
        {
            get
            {
                if (!string.IsNullOrEmpty(Extension))
                {
                    return string.Format("{0}.{1}", MaterialId, Extension);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [Display(Name ="ParentOrganizationName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<SelectListItem> MaterialTypeSelectItems { get; set; }

        public List<MaterialSpecificationModel> MaterialSpecificationModels { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();
            MaterialTypeSelectItems = new List<SelectListItem>();
            MaterialSpecificationModels = new List<MaterialSpecificationModel>();
        }
    }
}
