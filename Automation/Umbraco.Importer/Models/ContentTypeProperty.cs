namespace Umbraco.SiteBuilder.Models
{
    public class ContentTypeProperty : SortedCMSEntity
    {
        public PropertyDataType DataType { get; set; }
        public bool IsMandatory { get; set; }
        public string Description { get; set; }
    }
}
