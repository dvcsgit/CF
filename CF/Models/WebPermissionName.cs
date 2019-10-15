using System;

namespace CF.Models
{
    public class WebPermissionName
    {
        public string WebPermissionId { get; set; }
        public virtual WebPermission WebPermission { get; set; }

        public string Language { get; set; }
        public string Name { get; set; }        
    }
}
