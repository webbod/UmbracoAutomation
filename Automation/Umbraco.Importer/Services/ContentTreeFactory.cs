using System.Collections.Generic;
using Umbraco.SiteBuilder.Models;
using Umbraco.SiteBuilder.Properties;

namespace Umbraco.SiteBuilder.Services
{
    public static class ContentTreeFactory
    {

        public const int CONTENT_ROOT = -1;

        public static ContentTree GetContentTree()
        {
            return new ContentTree
            {
                Compositions = new List<SimpleContentType>
                {
                    ContentComposition(), MetaDataComposition()
                },
                ContentTypes = new List<ComposedContentType>
                {
                    // children need to be created before their parents
                    // otherwise they won't be registered in the hierarchy correctly
                    ContentPageDocument(),
                    HomePageDocument()
                },
                SupportingFiles = new List<SupportingFile>
                {
                    new SupportingFile
                    {
                         Content = ParseResoure(Resources._Layout),
                         Path = @"views\shared",
                         FileName = "_Layout.cshtml"
                    },
                    new SupportingFile
                    {
                         Content = ParseResoure(Resources._Menu),
                         Path = @"views\partials",
                         FileName = "_Menu.cshtml"
                    },
                },
                Content = new List<Page>
                {
                    new Page
                    {
                        Parent = CONTENT_ROOT,
                        ContentTypeAlias = "pageHome",
                        Name = "Home",
                        DocumentProperties = new Dictionary<string, object>
                        {
                            {"metaDataTitle","Home Page" },
                            {"contentBody", "<p>This is the <strong>Home Page</strong> in <em>English</em></p>" }
                        }
                    },
                    new Page
                    {
                        ParentName = "Home",
                        ContentTypeAlias = "pageContent",
                        Name = "About",
                        DocumentProperties = new Dictionary<string, object>
                        {
                            {"metaDataTitle","About Page" },
                            {"contentHeadline","Find out more about us" },
                            {"contentBody", "<p>This is the <strong>About Page</strong> in <em>English</em></p>" }
                        }
                    },
                }
            };
        }


        public static string ParseResoure(byte[] resource)
        {
            return System.Text.Encoding.UTF8.GetString(resource);
        }
        public static SimpleContentType ContentComposition()
        {
            return new SimpleContentType
            {
                Name = "Content composition",
                Alias = "compoContent",
                Description = "Shared content properties",
                Icon = "icon-desktop color-blue",
                ContainerName = "Compositions",
                SortOrder = 10,
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                         Name = "Content",
                         Properties = new List<ContentTypeProperty>
                        {
                            new ContentTypeProperty
                            {
                                 Name = "Body",
                                 Alias = "contentBody",
                                 SortOrder = 10,
                                 Description = "This is the main content that will appear in the page",
                                 DataType = PropertyDataType.TinyMCE
                            }
                        }
                    }
                }
            };
        }

        public static SimpleContentType MetaDataComposition()
        {
            return new SimpleContentType
            {
                Name = "MetaData composition",
                Alias = "compoMetaData",
                Description = "Shared metadata properties",
                Icon = "icon-notepad color-blue",
                ContainerName = "Compositions",
                SortOrder = 1,
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                         Name = "MetaData",
                         Properties = new List<ContentTypeProperty>
                        {
                            new ContentTypeProperty
                            {
                                 Name = "Title",
                                 Alias = "metaDataTitle",
                                 SortOrder = 10,
                                 Description = "This is the title of the page",
                                 DataType = PropertyDataType.TextBox
                            }
                        }
                    }
                }
            };
        }

        public static ComposedContentType ContentPageDocument()
        {
            return new ComposedContentType
            {
                Name = "Content page",
                Alias = "pageContent",
                Description = "A content page",
                Icon = "icon-document color-green",
                ContainerName = "Pages",
                SortOrder = 1,
                CompositionAliases = new List<string> { "compoContent", "compoMetaData" },
                Tabs = new List<Tab>
                {
                    new Tab
                    {
                         Name = "Content",
                         Properties = new List<ContentTypeProperty>
                        {
                            new ContentTypeProperty
                            {
                                 Name = "Headline",
                                 Alias = "contentHeadline",
                                 SortOrder = 1,
                                 Description = "This is the heading for the page",
                                 DataType = PropertyDataType.TextBox
                            }
                        }
                    }
                },
                Template = new TemplateDoc
                {
                    Alias = "templateContentPage",
                    Name = "ContentPage",
                    Html = ParseResoure(Resources.templateContentPage)
                }
            };
        }

        public static ComposedContentType HomePageDocument()
        {
            return new ComposedContentType
            {
                Name = "Home page",
                Alias = "pageHome",
                Description = "The entry point for your website",
                Icon = "icon-home color-green",
                ContainerName = "Pages",
                SortOrder = 1,
                AllowAtRoot = true,
                CompositionAliases = new List<string> { "compoContent", "compoMetaData" },
                AllowedContentTypes = new List<string> { "pageContent" },
                Template = new TemplateDoc
                {
                    Alias = "templateHomePage",
                    Name = "HomePage",
                    Html = ParseResoure(Resources.templateHomePage)
                }
            };
        }
    }
}
