using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.Role
{
    public class FormInput
    {
        [Display(Name="RoleId",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName ="RoleIdRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string RoleId { get; set; }

        [Display(Name ="RoleName",ResourceType =typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName ="RoleNameRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string Name { get; set; }
       
        public string WebPermissionFunctions { get; set; }

        public List<WebPermissionFunction> WebPermissionFunctionList
        {
            get
            {
                var webPermissionFunctionList = new List<WebPermissionFunction>();
                var temp = JsonConvert.DeserializeObject<List<string>>(WebPermissionFunctions);

                foreach(var t in temp)
                {
                    string[] x = t.Split(Define.Seperators, StringSplitOptions.None);

                    webPermissionFunctionList.Add(new WebPermissionFunction()
                    {
                        WebPermissionId = x[0],
                        WebFunctionId = x[1]
                    });
                }

                return webPermissionFunctionList;
            }
        }
    }
}
