using System.Collections.Generic;

namespace Umbraco.SiteBuilder.Models
{
    public class Tab
    {
        public string Name { get; set; }
        public List<ContentTypeProperty> Properties { get; set; }
    }
}
