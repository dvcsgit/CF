using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utility;

namespace Models.Maintenance.MSpecification
{
    public class DetailViewModel
    {
        public string MaterialSpecificationId { get; set; }

        [Display(Name="MaterialSpecificationName",ResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name ="ParentOrganizationName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        [Display(Name="MaterialType",ResourceType =typeof(Resources.Resource))]
        public string MaterialType { get; set; }

        public Define.EnumOrganizationPermission Permission { get; set; }

        public List<string> MaterialSpecificationOptionNames { get; set; }

        [Display(Name ="MaterialSpecificationOptionNames",ResourceType =typeof(Resources.Resource))]
        public string MaterialSpecificationOptionNamesString
        {
            get
            {
                if (MaterialSpecificationOptionNames != null && MaterialSpecificationOptionNames.Count > 0)
                {
                    var stringBuilder = new StringBuilder();
                    foreach(var name in MaterialSpecificationOptionNames)
                    {
                        stringBuilder.Append(name);
                        stringBuilder.Append("、");
                    }

                    stringBuilder.Remove(stringBuilder.Length - 1, 1);

                    return stringBuilder.ToString();
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
            MaterialSpecificationOptionNames = new List<string>();
        }
    }
}
