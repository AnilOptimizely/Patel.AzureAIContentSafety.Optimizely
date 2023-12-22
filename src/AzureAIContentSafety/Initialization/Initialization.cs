using EPiServer.Framework.Initialization;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer;
using EPiServer.Web;
using AzureAIContentSafety.Helpers;
using AzureAIContentSafety.Services;
using Microsoft.Extensions.DependencyInjection;
using EPiServer.Core;
using Azure.AI.ContentSafety;
using EPiServer.Framework.Blobs;
using AzureAIContentSafety.Interface;
using AzureAIContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Models;
using System.Collections.Generic;
using System.Linq;

namespace AzureAIContentSafety.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public sealed class Initialization : IConfigurableModule
    {
        private IContentEvents _contentEvents = null;
        private IContentLoader _contentLoader = null;

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IAzureAIContentSafetyService, AzureAIContentSafetyService>();
            context.Services.AddScoped<OptimizelyCmsHelpers>();
            context.Services.AddMvc(options => options.EnableEndpointRouting = false);
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
                }
                else
                {
                    if (detectIfImageAnalysisAllowed.Count > 1)
                    {
                        e.CancelReason = string.Format("Please only have 1 CMS boolean property with attribute ImageAnalysisAllowed");
                        e.CancelAction = true;
                    }
                }
            }
            if (e.Content is IContent content)
            {
                var detectIfTextAnalysisAllowed = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(TextAnalysisAllowedAttribute));
                if (detectIfTextAnalysisAllowed.Any() && detectIfTextAnalysisAllowed.Count == 1)
                {
                    var getFirstDefaultChosenBlocklistOption = detectIfTextAnalysisAllowed.FirstOrDefault();
                    var getAllowedValue = getFirstDefaultChosenBlocklistOption.Property.GetValue(getFirstDefaultChosenBlocklistOption.Content);
                    if (getAllowedValue is bool)
                    {
                        bool value = (bool)getAllowedValue;
                        if (value)
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
                            if (listTextAnalysed.Any())
                            {
                                var retrieveSeverityLevelValues = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(SeverityLevelAttribute));
                                var violenceResults = new List<TextAnalyzeSeverityResult>();
                                var hateResults = new List<TextAnalyzeSeverityResult>();
                                var sexualResults = new List<TextAnalyzeSeverityResult>();
                                var selfHarmResults = new List<TextAnalyzeSeverityResult>();
                                var listSeverityModels = new List<SeverityModel>();
                                if (retrieveSeverityLevelValues.Any())
                                {
                                    foreach (var severity in retrieveSeverityLevelValues)
                                    {
                                        var severityModel = new SeverityModel();
                                        var getValueCMS = severity.Property.GetValue(severity.Content);
                                        severityModel.SeverityLevel = int.Parse(getValueCMS.ToString());
                                        if (severity.Property.Name.Contains("Sexual"))
                                        {
                                            severityModel.CategoryName = "SexualResult";
                                        }
                                        if (severity.Property.Name.Contains("SelfHarm"))
                                        {
                                            severityModel.CategoryName = "SelfHarmResult";
                                        }
                                        if (severity.Property.Name.Contains("Violence"))
                                        {
                                            severityModel.CategoryName = "ViolenceResult";
                                        }
                                        if (severity.Property.Name.Contains("Hate"))
                                        {
                                            severityModel.CategoryName = "HateResult";
                                        }
                                        listSeverityModels.Add(severityModel);
                                    }
                                }

                                foreach (var textAnalysis in listTextAnalysed)
                                {
                                    if (listSeverityModels.Any())
                                    {
                                        foreach (var severityModel in listSeverityModels)
                                        {
                                            if (severityModel.CategoryName == "HateResult")
                                            {
                                                if (textAnalysis.HateResult.Severity > severityModel.SeverityLevel)
                                                {
                                                    hateResults.Add(textAnalysis.HateResult);
                                                }
                                            }
                                            if (severityModel.CategoryName == "ViolenceResult")
                                            {
                                                if (textAnalysis.ViolenceResult.Severity > severityModel.SeverityLevel)
                                                {
                                                    violenceResults.Add(textAnalysis.HateResult);
                                                }
                                            }
                                            if (severityModel.CategoryName == "SexualResult")
                                            {
                                                if (textAnalysis.SexualResult.Severity > severityModel.SeverityLevel)
                                                {
                                                    sexualResults.Add(textAnalysis.HateResult);
                                                }
                                            }
                                            if (severityModel.CategoryName == "SelfHarmResult")
                                            {
                                                if (textAnalysis.SelfHarmResult.Severity > severityModel.SeverityLevel)
                                                {
                                                    selfHarmResults.Add(textAnalysis.HateResult);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (violenceResults.Any() || selfHarmResults.Any() || sexualResults.Any() || hateResults.Any())
                                {
                                    var startString = "Unable to publish - Azure AI Content Safety has decteted the following ";
                                    var endString = "Please review content and publish again";
                                    var violenceText = "";
                                    var sexualText = "";
                                    var hateText = "";
                                    var selfHarmText = "";

                                    if (violenceResults.Any())
                                    {
                                        if (violenceResults.Count == 1)
                                        {
                                            violenceText = string.Format("1 count of Violence related content in the content published. ");
                                        }
                                        if (violenceResults.Count > 1)
                                        {
                                            violenceText = string.Format("{0} counts of Violence related content in the content published. ", violenceResults.Count);
                                        }
                                    }
                                    if (hateResults.Any())
                                    {
                                        if (hateResults.Count == 1)
                                        {
                                            hateText = string.Format("1 count of Violence related content in the content published. ");
                                        }
                                        if (hateResults.Count > 1)
                                        {
                                            hateText = string.Format("{0} counts of Violence related content in the content published. ", hateResults.Count);
                                        }
                                    }
                                    if (sexualResults.Any())
                                    {
                                        if (sexualResults.Count == 1)
                                        {
                                            sexualText = string.Format("1 count of Violence related content in the content published. ");
                                        }
                                        if (sexualResults.Count > 1)
                                        {
                                            sexualText = string.Format("{0} counts of Violence related content in the content published. ", sexualResults.Count);
                                        }
                                    }
                                    if (selfHarmResults.Any())
                                    {
                                        if (selfHarmResults.Count == 1)
                                        {
                                            selfHarmText = string.Format("1 count of Violence related content in the content published. ");
                                        }
                                        if (selfHarmResults.Count > 1)
                                        {
                                            selfHarmText = string.Format("{0} counts of Violence related content in the content published. ", selfHarmResults.Count);
                                        }
                                    }
                                    e.CancelReason = startString + violenceText + sexualText + hateText + selfHarmText + endString;
                                    e.CancelAction = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (detectIfTextAnalysisAllowed.Count > 1)
                    {
                        e.CancelReason = string.Format("Please only have 1 CMS boolean property with attribute TextAnalysisAllowed");
                        e.CancelAction = true;
                    }
                }

                var detectIfTextAnalysisBlocklistAllowed = OptimizelyCmsHelpers.GetPagePropertiesWithAttribute(getStartPage, typeof(TextAnalysisBlocklistAllowedAttribute));

                if (detectIfTextAnalysisBlocklistAllowed.Any() && detectIfTextAnalysisBlocklistAllowed != null)
                {
                    var getFirstDefaultBlocklistAllowed = detectIfTextAnalysisBlocklistAllowed.FirstOrDefault();
                    var getAllowedValue = getFirstDefaultBlocklistAllowed.Property.GetValue(getFirstDefaultBlocklistAllowed.Content);
                    if (getAllowedValue is bool)
                    {
                        bool value = (bool)getAllowedValue;
                        if (value)
                        {
                            var textBlocklistMatchResult = new List<List<TextBlocklistMatchResult>>();
                            var chosenBlocklistOption = OptimizelyCmsHelpers.GetPropertiesWithAttribute(content, typeof(TextAnalysisBlocklistDropdown));
                            if (chosenBlocklistOption.Any() && chosenBlocklistOption != null)
                            {
                                if (chosenBlocklistOption.Count == 1)
                                {
                                    var getFirstDefaultChosenBlocklistOption = chosenBlocklistOption.FirstOrDefault();
                                    var getBlocklistOption = getFirstDefaultChosenBlocklistOption.Property.GetValue(getFirstDefaultChosenBlocklistOption.Content).ToString();
                                    if (!string.IsNullOrWhiteSpace(getBlocklistOption))
                                    {
                                        var getTextAnalysisAttributesBlocklist = OptimizelyCmsHelpers.GetPropertiesWithAttribute(content, typeof(TextAnalysisAttribute));
                                        if (getTextAnalysisAttributesBlocklist.Any() && getTextAnalysisAttributesBlocklist != null)
                                        {
                                            foreach (var attribute in getTextAnalysisAttributesBlocklist)
                                            {
                                                var getTextAnalysisValue = attribute.Property.GetValue(attribute.Content).ToString();
                                                var analyseTextBlocklist = ContentSafetyServiceAnalyser.AnalyseTextWithBlockList(getBlocklistOption, getTextAnalysisValue, content).ToList();
                                                textBlocklistMatchResult.Add(analyseTextBlocklist);
                                            }
                                        }
                                        if (textBlocklistMatchResult.Any() && textBlocklistMatchResult != null)
                                        {
                                            var startString = "Unable to publish - Azure AI Content Safety has decteted the following:  ";
                                            var endString = "Please review content and publish again";
                                            var blocklistText = "";
                                            if (textBlocklistMatchResult.Count == 1)
                                            {
                                                blocklistText = string.Format("1 Blocklist Match in the content published. ");
                                            }
                                            if (textBlocklistMatchResult.Count > 1)
                                            {
                                                blocklistText = string.Format("{0} Blocklist matches in the content published. ", textBlocklistMatchResult.Count);
                                            }
                                            e.CancelReason = startString + blocklistText + endString;
                                            e.CancelAction = true;

                                        }
                                    }
                                }
                                else
                                {
                                    e.CancelReason = string.Format("Please only have 1 CMS boolean property with attribute TextAnalysisBlocklist");
                                    e.CancelAction = true;
                                }
                            }
                        }
                    }

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
