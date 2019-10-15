using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utility;

namespace Models.Maintenance.ESpecification
{
    public class DetailViewModel
    {
        public string EquipmentSpecificationId { get; set; }

        [Display(Name ="EquipmentSpecificationName",ResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name ="EquipmentType",ResourceType =typeof(Resources.Resource))]
        public string EquipmentType { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }
        public string OrganizationId { get; set; }

        [Display(Name="ParentOrganizationFullName",ResourceType =typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public List<string> EquipmentSpecificationOptonNameList { get; set; }

        [Display(Name ="EquipmentSpecificationOptionNames",ResourceType =typeof(Resources.Resource))]
        public string EquipmentSpecificationOptionNames
        {
            get
            {
                if(EquipmentSpecificationOptonNameList != null&& EquipmentSpecificationOptonNameList.Count > 0)
                {
                    var stringBuilder = new StringBuilder();

                    foreach(var option in EquipmentSpecificationOptonNameList)
                    {
                        stringBuilder.Append(option);
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
    }
}
