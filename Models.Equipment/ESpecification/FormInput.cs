using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.ESpecification
{
    public class FormInput
    {
        [Display(Name = "EquipmentType", ResourceType = typeof(Resources.Resource))]
        public string EquipmentType { get; set; }

        [Display(Name = "EquipmentSpecificationName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "EquipmentSpecificationNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        public string EquipmentSpecificationOptionJson { get; set; }

        public List<EquipmentSpecificationOptionModel> EquipmentSpecificationOptionModels
        {
            get
            {
                var options = new List<EquipmentSpecificationOptionModel>();
                var temp = JsonConvert.DeserializeObject<List<string>>(EquipmentSpecificationOptionJson);
                int seq = 1;
                foreach(var t in temp)
                {
                    string[] x = t.Split(Define.Seperators, StringSplitOptions.None);

                    options.Add(new EquipmentSpecificationOptionModel()
                    {
                        EquipmentSpecificationOptionId = x[0],
                        Name = x[1],
                        Seq = seq
                    });

                    seq++;
                }

                return options;
            }
        }
    }
}
