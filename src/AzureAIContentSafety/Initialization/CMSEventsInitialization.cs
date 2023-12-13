using EPiServer.Core;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Azure.AI.ContentSafety;
using ImageData = EPiServer.Core.ImageData;
using AzureAIContentSafety.Helpers;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.Interfaces;
using EPiServer.Framework.Blobs;
using AzureAIContentSafety.ContentSafety.Helpers;

namespace AzureAIContentSafety.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CMSEventsInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.CreatingContent += CreatingContent;
        }

        private void CreatingContent(object sender, ContentEventArgs e)
        {
            var getIContentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var getStartPage = getIContentLoader.Get<IContent>(ContentReference.StartPage);

            if (e.Content is ImageData image)
            {
                var detectIfImageAnalysisAllowed = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(ImageAnalysisAllowedAttribute));
                if (detectIfImageAnalysisAllowed.Any() && detectIfImageAnalysisAllowed.Count == 1)
                {
                    var getImageAnalysisAllowedProperty = detectIfImageAnalysisAllowed.FirstOrDefault().Property;
                    var mediaData = image;
                    if (mediaData != null)
                    {
                        var fileBlob = mediaData.BinaryData as FileBlob;
                        var path = fileBlob.FilePath;
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            var analyseImage = ContentSafetyServiceAnalyser.AnalyzeImage(path, image);
                            if (analyseImage != null)
                            {
                                var retrieveSeverityLevelValues = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(SeverityLevelAttribute));
                                if (retrieveSeverityLevelValues != null && retrieveSeverityLevelValues.Any())
                                {

                                }
                            }
                        }
                    }
                }
            }

            if (e.Content is IContent content)
            {
                var detectIfTextAnalysisAllowed = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(TextAnalysisAllowedAttribute));
                if (detectIfTextAnalysisAllowed.Any())
                {
                    var getTextAnalysisAttributesBlocklist = OptimizelyCmsHelpers.GetPropertiesWithAttribute(content, typeof(TextAnalysisAttribute));
                    if (getTextAnalysisAttributesBlocklist.Any() && getTextAnalysisAttributesBlocklist != null)
                    {

                    }
                }

                var detectIfTextAnalysisBlocklistAllowed = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(TextAnalysisBlocklistAllowedAttribute));
                if (detectIfTextAnalysisBlocklistAllowed.Any() && detectIfTextAnalysisBlocklistAllowed != null)
                {
                    var getTextAnalysisAttributesBlocklist = OptimizelyCmsHelpers.GetPropertiesWithAttribute(content, typeof(TextAnalysisBlocklist));
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}
