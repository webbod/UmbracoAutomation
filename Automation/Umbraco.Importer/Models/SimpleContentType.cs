using System.Collections.Generic;

namespace Umbraco.SiteBuilder.Models
{
    public class SimpleContentType : SortedCMSEntity
    {
        public string ContainerName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public List<Tab> Tabs { get; set; }
        public bool AllowAtRoot { get; set; }
        public TemplateDoc Template { get; set; }
        public List<string> AllowedContentTypes { get; internal set; }
    }
}
