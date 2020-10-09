using System.Collections.Generic;

namespace Umbraco.SiteBuilder.Models
{
    public class Page
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public string ParentName { get; set; }
        public string Name { get; set; }
        public string ContentTypeAlias { get; set; }
        public Dictionary<string, object> DocumentProperties { get; set; }
    }
}
