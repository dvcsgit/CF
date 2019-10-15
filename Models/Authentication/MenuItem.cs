using System.Collections.Generic;

namespace Models.Authentication
{
    public class MenuItem
    {
        public MenuItem()
        {
            Name = new Dictionary<string, string>();
            SubItemList = new List<MenuItem>();
        }
        public string Id { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public int Seq { get; set; }
        public List<MenuItem> SubItemList { get; set; }
    }
}
