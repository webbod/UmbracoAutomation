using System.Collections.Generic;

namespace Umbraco.SiteBuilder.Models
{
    public class ComposedContentType : SimpleContentType
    {
        public List<string> CompositionAliases { get; set; }
    }
}
