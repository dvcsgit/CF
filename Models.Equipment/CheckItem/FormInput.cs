using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Maintenance.CheckItem
{
    public class FormInput
    {
        [Display(Name = "CheckItemType", ResourceType = typeof(Resources.Resource))]
        public string Type { get; set; }

        [Display(Name = "CIId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "CIIdRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string CIId { get; set; }

        [Display(Name = "CheckItemName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "CheckItemNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = "IsFeelItem", ResourceType = typeof(Resources.Resource))]
        public string IsFeelItem { get; set; }

        public int? TextValueType { get; set; }

        [Display(Name = "UpperLimit", ResourceType = typeof(Resources.Resource))]
        public string UpperLimit { get; set; }

        [Display(Name = "UpperAlertLimit", ResourceType = typeof(Resources.Resource))]
        public string UpperAlertLimit { get; set; }

        [Display(Name = "LowerAlertLimit", ResourceType = typeof(Resources.Resource))]
        public string LowerAlertLimit { get; set; }

        [Display(Name = "LowerLimit", ResourceType = typeof(Resources.Resource))]
        public string LowerLimit { get; set; }

        [Display(Name = "IsAccumulation", ResourceType = typeof(Resources.Resource))]
        public bool IsAccumulation { get; set; }

        [Display(Name = "AccumulationBase", ResourceType = typeof(Resources.Resource))]
        public string AccumulationBase { get; set; }

        [Display(Name = "Unit", ResourceType = typeof(Resources.Resource))]
        public string Unit { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Resources.Resource))]
        public string Remark { get; set; }

        public string JsonFeelOptions { get; set; }

        public List<FeelOptionModel> FeelOptionList
        {
            get
            {
                var feelOptionList = new List<FeelOptionModel>();

                var temp = JsonConvert.DeserializeObject<List<string>>(JsonFeelOptions);

                int seq = 1;

                foreach (var t in temp)
                {
                    string[] x = t.Split(Define.Seperators, StringSplitOptions.None);

                    feelOptionList.Add(new FeelOptionModel()
                    {
                        FeelOptionId = x[0],
                        Name = x[1],
                        IsAbnormal = x[2] == "Y",
                        Seq = seq
                    });

                    seq++;
                }

                return feelOptionList;
            }
        }
    }
}
