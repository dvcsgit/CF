using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.MSpecification
{
    public class FormInput
    {
        [Display(Name ="MaterialSpecificationName",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName ="MaterialSpecificationNameRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name ="MaterialType",ResourceType =typeof(Resources.Resource))]
        public string MaterialType { get; set; }
        public string MaterialSpecificationOptionsString { get; set; }

        public List<MaterialSpecificationOptionModel> MaterialSpecificationOptionModels
        {
            get
            {
                var options = new List<MaterialSpecificationOptionModel>();
                var temp = JsonConvert.DeserializeObject<List<string>>(MaterialSpecificationOptionsString);
                int seq = 1;

                foreach(var t in temp)
                {
                    string[] x = t.Split(Define.Seperators, StringSplitOptions.None);

                    options.Add(new MaterialSpecificationOptionModel()
                    {
                        MaterialSpecificationOptionId = x[0],
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
