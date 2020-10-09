using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.SiteBuilder.Models;
using Umbraco.Web;

namespace Umbraco.SiteBuilder.Services
{
    public class ContentTreeParser
    {

        private readonly IContentService _ContentService;
        private readonly IContentTypeService _ContentTypeService;
        private readonly IDataTypeService _DataTypeService;
        private readonly IFileService _FileService;
        private readonly IUmbracoContextFactory _ContextFactory;

        public ContentTreeParser(IContentService contentService, IContentTypeService contentTypeService, IDataTypeService dataTypeService, IFileService fileService, IUmbracoContextFactory contextFactory)
        {
            _ContentService = contentService;
            _ContentTypeService = contentTypeService;
            _DataTypeService = dataTypeService;
            _FileService = fileService;
            _ContextFactory = contextFactory;
        }

        public void Parse(ContentTree contentTree)
        {
            //var json = Json.Encode(contentTree);

            foreach (var composition in contentTree.Compositions)
            {
                CreateContentType(composition);
            }

            foreach (var contentType in contentTree.ContentTypes)
            {
                CreateContentType(contentType);
            }

            foreach (var file in contentTree.SupportingFiles)
            {
                CreateFile(file);
            }

            foreach(var page in contentTree.Content)
            {
                if(page.Parent == 0)
                {
                    page.Parent = contentTree.Content.Single(p => p.Name == page.ParentName).Id;
                }

                if (page.Parent != 0)
                {
                    CreatePage(page);
                }
            }
        }

        private int GetContainerId(string containerName)
        {
            var container = _ContentTypeService.GetContainers(containerName, 1).FirstOrDefault();

            if (container != null)
            {
                return container.Id;
            }

            var attempt = _ContentTypeService.CreateContainer(-1, containerName);
            if (attempt.Success)
            {
                return attempt.Result.Entity.Id;
            }

            throw new InvalidOperationException($"Unable to create container: {containerName}");
        }

        private IContentType GetContentType(string alias)
        {
            return _ContentTypeService.Get(alias);
        }

        private IContentType GetContentType(Models.SimpleContentType docType)
        {
            var contentType = GetContentType(docType.Alias);

            if (contentType != null)
            {
                return contentType;
            }

            return new ContentType(GetContainerId(docType.ContainerName))
            {
                Name = docType.Name,
                Alias = docType.Alias,
                Description = docType.Description,
                Icon = docType.Icon,
                AllowedAsRoot = docType.AllowAtRoot
            };
        }

        private void AddCompositions(ComposedContentType docType, IContentType contentType)
        {
            var compositions = new List<IContentTypeComposition>();
            foreach (var compositionAlias in docType.CompositionAliases)
            {
                compositions.Add(_ContentTypeService.Get(compositionAlias));
            }

            contentType.ContentTypeComposition = compositions;
        }

        private void AddTemplate(Models.SimpleContentType docType, IContentType contentType)
        {
            if (docType.Template == null)
            {
                return;
            }

            var template = _FileService.GetTemplate(docType.Template.Alias);

            if (template == null)
            {
                template = new Template(docType.Template.Name, docType.Template.Alias)
                {
                    Content = docType.Template.Html
                };

                _FileService.SaveTemplate(template);
            }

            contentType.AllowedTemplates = new List<ITemplate> { template };
            contentType.SetDefaultTemplate(template);
        }

        private void AddAllowedContentTypes(Models.SimpleContentType docType, IContentType contentType)
        {
            if (docType.AllowedContentTypes == null)
            {
                return;
            }

            var sortOrder = 0;
            var allowedContentTypes = new List<ContentTypeSort>();
            foreach (var alias in docType.AllowedContentTypes)
            {
                var allowedContentType = GetContentType(alias);
                allowedContentTypes.Add(new ContentTypeSort(allowedContentType.Id, sortOrder++));
            }

            contentType.AllowedContentTypes = allowedContentTypes;
        }

        private void AddTabs(Models.SimpleContentType docType, IContentType contentType)
        {
            if (docType.Tabs == null)
            {
                return;
            }

            foreach (var tab in docType.Tabs)
            {
                contentType.AddPropertyGroup(tab.Name);
                AddProperties(tab, contentType);
            }
        }

        private IDataType GetDataType(PropertyDataType dataType)
        {
            return _DataTypeService.GetDataType((int)dataType);
        }

        private void AddProperties(Tab tab, IContentType contentType)
        {
            foreach (var property in tab.Properties)
            {
                contentType.AddPropertyType(new PropertyType(GetDataType(property.DataType), property.Alias)
                {
                    Name = property.Name,
                    Description = property.Description,
                    Mandatory = property.IsMandatory,
                    SortOrder = property.SortOrder,
                }, tab.Name);
            }
        }

        private void CreateContentType(Models.SimpleContentType docType)
        {
            var contentType = GetContentType(docType);

            if (docType is ComposedContentType)
            {
                AddCompositions((ComposedContentType)docType, contentType);
            }

            AddTabs(docType, contentType);
            AddAllowedContentTypes(docType, contentType);
            AddTemplate(docType, contentType);

            _ContentTypeService.Save(contentType);
        }

        private void CreateFile(SupportingFile file)
        {
            var root = HttpRuntime.AppDomainAppPath;
            var path = Path.Combine(root, file.Path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.Combine(path, file.FileName);
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.WriteAllText(fileName, file.Content);
            }
        }

        private void CreatePage(Page page)
        {
            var node = _ContentService.Create(page.Name, page.Parent, page.ContentTypeAlias);
            foreach(var prop in page.DocumentProperties)
            {
                node.SetValue(prop.Key, prop.Value);
            }


            // This is an issue with Umbraco 8 publishing nodes on start-up
            // https://github.com/umbraco/Umbraco-CMS/issues/5281
            using (_ContextFactory.EnsureUmbracoContext())
            {
                _ContentService.SaveAndPublish(node, raiseEvents: false);
                page.Id = node.Id;
            }
            
        }
    }
}
