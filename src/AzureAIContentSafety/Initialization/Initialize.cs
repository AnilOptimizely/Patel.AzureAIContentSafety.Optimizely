using EPiServer.Framework.Initialization;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer;
using EPiServer.Web;
using AzureAIContentSafety.Helpers;
using AzureAIContentSafety.Interfaces;
using AzureAIContentSafety.Services;
using Microsoft.Extensions.DependencyInjection;
using EPiServer.Core;
using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Helpers;
using EPiServer.Framework.Blobs;

namespace AzureAIContentSafety.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public sealed class Initialize : IConfigurableModule
    {
        private IContentEvents _contentEvents = null;
        private IContentLoader _contentLoader = null;

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IAzureAIContentSafetyService, AzureAIContentSafetyService>();
            context.Services.AddScoped<OptimizelyCmsHelpers>();
        }

        void Events_PublishingContent(object sender, ContentEventArgs e)
        {
            var getIContentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var getStartPage = getIContentLoader.Get<IContent>(ContentReference.StartPage);

            if (e.Content is EPiServer.Core.ImageData image)
            {
                var detectIfImageAnalysisAllowed = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(ImageAnalysisAllowedAttribute));
                if (detectIfImageAnalysisAllowed.Any() && detectIfImageAnalysisAllowed.Count == 1)
                {
                    var getImageAnalysisAllowedProperty = detectIfImageAnalysisAllowed.FirstOrDefault();
                    var getAllowedValue = getImageAnalysisAllowedProperty.Property.GetValue(getImageAnalysisAllowedProperty.Content);
                    if (getAllowedValue is bool)
                    {
                        bool value = (bool)getAllowedValue;
                        if (value)
                        {
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
                                            foreach (var severityLevel in retrieveSeverityLevelValues)
                                            {
                                                var getValueCMS = severityLevel.Property.GetValue(severityLevel.Content);
                                                int severityLevelCMS = int.Parse(getValueCMS.ToString());
                                                if (severityLevel.Property.Name.Contains("Sexual"))
                                                {
                                                    if (analyseImage.SexualResult.Severity > 1)
                                                    {
                                                        e.CancelReason = string.Format("Unable to publish - Azure AI Content Safety has decteted Sexual content. Severity Level Detected is : {0} and the Level allowed is : {1} , ", analyseImage.SexualResult.Severity, severityLevelCMS);
                                                        e.CancelAction = true;
                                                    }
                                                }
                                                if (severityLevel.Property.Name.Contains("SelfHarm"))
                                                {
                                                    if (analyseImage.SelfHarmResult.Severity > 1)
                                                    {
                                                        e.CancelReason = string.Format("Unable to publish Image - Azure AI Content Safety has decteted Self Harm content. Severity Level Detected is : {0} and the Level allowed is : {1} , ", analyseImage.HateResult.Severity, severityLevelCMS);
                                                        e.CancelAction = true;
                                                    }
                                                }
                                                if (severityLevel.Property.Name.Contains("Violence"))
                                                {
                                                    if (analyseImage.ViolenceResult.Severity > 1)
                                                    {
                                                        e.CancelReason = string.Format("Unable to publish - Azure AI Content Safety has decteted Violent content in the Main Body property. Severity Level Detected is : {0} and the Severity Level allowed is : {1} , ", analyseImage.ViolenceResult.Severity, severityLevelCMS);
                                                        e.CancelAction = true;
                                                    }

                                                }
                                                if (severityLevel.Property.Name.Contains("Hate"))
                                                {
                                                    if (analyseImage.HateResult.Severity > 1)
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
                    else
                    {
                        e.CancelReason = string.Format("Unable to publish - Azure AI Content Safety has decteted Sexual content. Severity Level Detected is : {0} and the Level allowed is : {1} , ", 2, 4);
                        e.CancelAction = true;
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

        void IInitializableModule.Initialize(InitializationEngine initializationEngine)
        {
            ServiceProviderHelper serviceLocationHelper = initializationEngine.Locate;
            _contentEvents = initializationEngine.Locate.Advanced.GetInstance<IContentEvents>();
            _contentLoader = serviceLocationHelper.ContentLoader();
            _contentEvents.PublishingContent += Events_PublishingContent;
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {
        }
}
}
