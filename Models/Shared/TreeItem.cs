using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.Shared
{
    public class TreeItem
    {
        [JsonProperty("data")]
        public string Title { get; set; }

        [JsonProperty("attr")]
        public Dictionary<string,string> Attributes { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("children")]
        public List<TreeItem> Children { get; set; }

        public TreeItem()
        {
            Attributes = new Dictionary<string, string>();
            Children = new List<TreeItem>();
        }
    }
}
