using EPiServer.Core;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Azure.AI.ContentSafety;
using ImageData = EPiServer.Core.ImageData;
using AzureAIContentSafety.Helpers;
using AzureAIContentSafety.ContentSafety.Attributes;
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
                                    foreach(var severityLevel in retrieveSeverityLevelValues)
                                    {
                                        var getValueCMS = severityLevel.Property.GetValue(severityLevel.Content);
                                        int severityLevelCMS = Int32.Parse(getValueCMS.ToString());
                                        if (severityLevel.Property.Name.Contains("Sexual"))
                                        {
                                            if (analyseImage.SexualResult.Severity > severityLevelCMS)
                                            {
                                                e.CancelReason = string.Format("Unable to publish - Azure AI Content Safety has decteted Sexual content. Severity Level Detected is : {0} and the Level allowed is : {1} , ", analyseImage.SexualResult.Severity, severityLevelCMS);
                                                e.CancelAction = true;
                                            }
                                        }
                                        if (severityLevel.Property.Name.Contains("SelfHarm"))
                                        {
                                            if (analyseImage.SelfHarmResult.Severity > severityLevelCMS)
                                            {
                                                e.CancelReason = string.Format("Unable to publish Image - Azure AI Content Safety has decteted Self Harm content. Severity Level Detected is : {0} and the Level allowed is : {1} , ", analyseImage.HateResult.Severity, severityLevelCMS);
                                                e.CancelAction = true;
                                            }
                                        }
                                        if (severityLevel.Property.Name.Contains("Violence"))
                                        {
                                            if (analyseImage.ViolenceResult.Severity > severityLevelCMS)
                                            {
                                                e.CancelReason = string.Format("Unable to publish - Azure AI Content Safety has decteted Violent content in the Main Body property. Severity Level Detected is : {0} and the Severity Level allowed is : {1} , ", analyseImage.ViolenceResult.Severity, severityLevelCMS);
                                                e.CancelAction = true;
                                            }
                                            
                                        }
                                        if (severityLevel.Property.Name.Contains("Hate"))
                                        {
                                            if (analyseImage.HateResult.Severity > severityLevelCMS)
                                            {
                                                e.CancelReason = string.Format("Unable to publish - Azure AI Content Safety has dectected Hate content in the Main Body property. Severity Level Detected is : {0} and the Level allowed is : {1} , ", analyseImage.SelfHarmResult.Severity, severityLevelCMS);
                                                e.CancelAction = true;
                                            }
                                        }
                                    }
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
                    var listTextAnalysed = new List<AnalyzeTextResult>();
                    var getTextAnalysisAttributesBlocklist = OptimizelyCmsHelpers.GetPropertiesWithAttribute(content, typeof(TextAnalysisAttribute));
                    if (getTextAnalysisAttributesBlocklist.Any() && getTextAnalysisAttributesBlocklist != null)
                    {
                        foreach (var attribute in getTextAnalysisAttributesBlocklist)
                        {
                            var getTextAnalysisValue = attribute.Property.GetValue(attribute.Content).ToString();
                            var analyseText = ContentSafetyServiceAnalyser.AnalyseText(getTextAnalysisValue, content);
                            listTextAnalysed.Add(analyseText);
                        }
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
