using System.Collections.Generic;

namespace Umbraco.SiteBuilder.Models
{
    public class ContentTree
    {
        public List<SimpleContentType> Compositions { get; set; }
        public List<ComposedContentType> ContentTypes { get; set; }
        public List<SupportingFile> SupportingFiles { get; set; }
        public List<Page> Content { get; set; }
    }
}
