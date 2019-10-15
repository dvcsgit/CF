using System.Collections.Generic;

namespace Models.Role
{
    public class WebFunctionModel
    {
        public string WebFunctionId { get; set; }

        public Dictionary<string, string> FunctionName { get; set; }

        public WebFunctionModel()
        {
            FunctionName = new Dictionary<string, string>();
        }
    }
}
