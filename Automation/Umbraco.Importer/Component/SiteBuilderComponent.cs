using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.SiteBuilder.Services;
using Umbraco.Web;

namespace Umbraco.SiteBuilder.Component
{
    public class SiteBuilderComponent : IComponent
    {
        private readonly IContentService _ContentService;
        private readonly IContentTypeService _ContentTypeService;
        private readonly IDataTypeService _DataTypeService;
        private readonly IFileService _FileService;
        private readonly IUmbracoContextFactory _ContextFactory;

        public SiteBuilderComponent(IContentService contentService, IContentTypeService contentTypeService, IDataTypeService dataTypeService, IFileService fileService, IUmbracoContextFactory contextFactory)
        {
            _ContentService = contentService;
            _ContentTypeService = contentTypeService;
            _DataTypeService = dataTypeService;
            _FileService = fileService;
            _ContextFactory = contextFactory;
        }

        public void Initialize()
        {
            try
            {
                RunOnce.CheckIfAlreadyRun();

                // this does all the work of creating compositions, content types, etc.
                var contentTreeParser = new ContentTreeParser(_ContentService, _ContentTypeService, _DataTypeService, _FileService, _ContextFactory);

                // this is simply for the purposes of the test, you could just as easily us a blob of json
                var contentTree = ContentTreeFactory.GetContentTree();

                contentTreeParser.Parse(contentTree);

                RunOnce.RecordFirstRun();
            }
            catch (OperationCanceledException)
            {
                // this just means that the dll has already been run once
                // you can remove the semaphore file from App_Data\TEMP\SiteBuilder.AlreadyRun
                // if you want to run the code again, but this might create 
                // extra nodes in the CMS or overwrite files
            }
        }

        public void Terminate() { }
    }
}
