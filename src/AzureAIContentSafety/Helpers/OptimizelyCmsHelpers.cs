using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Mvc.Rendering;
using EPiServer.ServiceLocation;
using EPiServer;
using EPiServer.Core;
using System.Reflection;
using Azure;
using ImageData = Azure.AI.ContentSafety.ImageData;
using Microsoft.Extensions.Options;
using EPiServer.Logging;
using Patel.AzureAIContentSafety.Optimizely.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using EPiServer.Framework.Blobs;
using Patel.AzureAIContentSafety.Optimizely.Interface;
using Patel.AzureAIContentSafety.Optimizely;
using Patel.AzureAIContentSafety.Optimizely.Models;
using Patel.AzureAIContentSafety.Optimizely.ContentSafety.Attributes;

namespace AzureAIContentSafety.Helpers
{
    public class OptimizelyCmsHelpers
    {
        protected readonly Injected<IAzureAIContentSafetyService> _azureContentSafetyService;
        private static IOptions<ContentSafetyOptions> _configuration;
        private static IOptions<ContentSafetyOptions> Configuration => _configuration ??= ServiceLocator.Current.GetInstance<IOptions<ContentSafetyOptions>>();
        private static readonly string ContentSafetySubscriptionKey = Configuration.Value.ContentSafetySubscriptionKey;
        private static readonly string ContentSafetyEndpoint = Configuration.Value.ContentSafetyEndpoint;
        private readonly ILogger Log = LogManager.GetLogger();

        public List<SelectListItem> GetTextBlockLists()
        {
            var olps = new List<SelectListItem>();
            var getBlockLists = _azureContentSafetyService.Service.GetBlocklists();
            if (getBlockLists.Any())
            {
                foreach (var blockList in getBlockLists)
                {
                    var listItem = new SelectListItem
                    {
                        Text = blockList.BlocklistName,
                        Value = blockList.BlocklistName
                    };
                    olps.Add(listItem);
                }
            }
            return olps;
        }

        public List<SelectListItem> GetBlockListItems()
        {
            var listOfBlockListItems = new List<SelectListItem>();
            var getBlockLists = _azureContentSafetyService.Service.GetBlocklists();
            if (getBlockLists.Any())
            {
                foreach (var blockList in getBlockLists)
                {
                    var blockListItems = _azureContentSafetyService.Service.GetBlockItemsDropdown(blockList.BlocklistName);
                    if (blockListItems.BlockItems.Any() && blockListItems.BlockItems != null)
                    {
                        foreach (var blockItem in blockListItems.BlockItems)
                        {
                            var listItem = new SelectListItem
                            {
                                Text = blockListItems.BlockListName,
                                Value = blockItem
                            };
                            listOfBlockListItems.Add(listItem);
                        }
                    }
                }
            }
            return listOfBlockListItems;
        }

        public List<TextBlocklist> GetTextBlockListsCMS()
        {
            return _azureContentSafetyService.Service.GetBlocklists();
        }

        public static AnalyzeImageResult AnalyseImage(string imageFilePath)
        {
            // Example: analyze image
            ImageData image = new() { Content = BinaryData.FromBytes(File.ReadAllBytes(imageFilePath)) };
            var request = new AnalyzeImageOptions(image);

            Response<AnalyzeImageResult> response;
            try
            {
                response = GetClient().AnalyzeImageAsync(request).Result;
                Console.WriteLine("Azure AI Content Safety -  Image Analysis complete");
                Console.WriteLine("Hate severity: {0}", response.Value.HateResult?.Severity ?? 0);
                Console.WriteLine("SelfHarm severity: {0}", response.Value.SelfHarmResult?.Severity ?? 0);
                Console.WriteLine("Sexual severity: {0}", response.Value.SexualResult?.Severity ?? 0);
                Console.WriteLine("Violence severity: {0}", response.Value.ViolenceResult?.Severity ?? 0);
                return response.Value;

            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Image Analysis failed.\nStatus code: {0}, Error code: {1}, Error message: {2}", ex.Status, ex.ErrorCode, ex.Message);
                throw;
            }
        }

        public static AnalyzeTextResult AnalyseText(string textToBeAnalysed)
        {
            if (!string.IsNullOrWhiteSpace(textToBeAnalysed))
            {
                // Example: analyze text without blocklist
                var request = new AnalyzeTextOptions(textToBeAnalysed);

                Response<AnalyzeTextResult> response;
                try
                {
                    response = GetClient().AnalyzeText(request);
                    Console.WriteLine("Azure AI Content Safety -  Text Analysis complete");
                    Console.WriteLine("Hate severity: {0}", response.Value.HateResult?.Severity ?? 0);
                    Console.WriteLine("SelfHarm severity: {0}", response.Value.SelfHarmResult?.Severity ?? 0);
                    Console.WriteLine("Sexual severity: {0}", response.Value.SexualResult?.Severity ?? 0);
                    Console.WriteLine("Violence severity: {0}", response.Value.ViolenceResult?.Severity ?? 0);
                    return response.Value;
                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine("Analyze text failed.\nStatus code: {0}, Error code: {1}, Error message: {2}", ex.Status, ex.ErrorCode, ex.Message);
                    throw;
                }
            }
            return null;
        }

        public static IReadOnlyList<TextBlocklistMatchResult> AnalyseTextWithBlockList(string blockListName, string inputString)
        {
            // Example: analyze text with blocklist
            var request = new AnalyzeTextOptions(inputString);
            request.BlocklistNames.Add(blockListName);
            request.BreakByBlocklists = true;

            Response<AnalyzeTextResult> response;
            try
            {
                response = GetClient().AnalyzeText(request);

                if (response.Value.BlocklistsMatchResults != null)
                {
                    Console.WriteLine("Azure AI Content Safety -  Analysis of Text using a Blocklist operation complete");
                    Console.WriteLine("Blocklist Match Count: {0}", response.Value.BlocklistsMatchResults.Count);
                    return response.Value.BlocklistsMatchResults;
                }
            }
            catch (RequestFailedException)
            {
                throw;

            }
            return null;
        }

        public static ContentSafetyClient GetClient()
        {
            ContentSafetyClient client = new(new Uri(ContentSafetyEndpoint), new AzureKeyCredential(ContentSafetySubscriptionKey));
            return client;
        }

        public static IList<ContentProperty> GetPropertiesWithAttribute(IContent content, Type attribute)
        {
            var pageProperties = GetPagePropertiesWithAttribute(content, attribute);
            var blockProperties = GetBlockPropertiesWithAttribute(content, attribute);
            return pageProperties.Union(blockProperties).ToList();
        }

        public static IList<ContentProperty> GetPagePropertiesWithAttribute(IContent content, Type attribute)
        {
            var getList = content.GetType().GetProperties()
                .Where(pageProperty => Attribute.IsDefined(pageProperty, attribute))
                .Select(property => new ContentProperty { Content = content, Property = property });
            return getList.ToList();
        }

        public static IList<ContentProperty> GetBlockPropertiesWithAttribute(IContent content, Type attribute)
        {
            return content.GetType().GetProperties()
                .Where(pageProperty => typeof(BlockData).IsAssignableFrom(pageProperty.PropertyType))
                .Select(propertyInfo => GetBlockPropertiesWithAttributeForSingleBlock(content, propertyInfo, attribute)).SelectMany(x => x).ToList();
        }

        public static IList<ContentProperty> GetBlockPropertiesWithAttributeForSingleBlock(IContent content, PropertyInfo localBlockProperty, Type attribute)
        {
            var blockPropertiesWithAttribute = localBlockProperty.PropertyType.GetProperties().Where(blockProperty => Attribute.IsDefined(blockProperty, attribute));
            var block = content.Property[localBlockProperty.Name].GetType().GetProperties().Single(x => x.Name == "Block").GetValue(content.Property[localBlockProperty.Name]);
            return blockPropertiesWithAttribute.Select(property => new ContentProperty { Content = block, Property = property }).ToList();
        }

        public static IEnumerable<AttributeContentProperty> GetAttributeContentPropertyList(IEnumerable<ContentProperty> contentProperties)
        {
            if (contentProperties.Any() && contentProperties != null)
            {
                foreach (var contentProperty in contentProperties)
                {
                    var attribute = contentProperty.Property.GetCustomAttributes(typeof(ContentSafetyBaseImageAttribute)).Cast<ContentSafetyBaseImageAttribute>().FirstOrDefault();
                    if (attribute != null)
                    {
                        yield return new AttributeContentProperty
                        {
                            Attribute = attribute,
                            Content = contentProperty.Content,
                            Property = contentProperty.Property
                        };
                    }
                }
            }
        }

        public static string ProcessImageAnalysis(IContent startPage, EPiServer.Core.ImageData image)
        {
            var result = "";
            var listStrings = new List<string>();
            var detectIfImageAnalysisAllowed = GetPagePropertiesWithAttribute(startPage, typeof(ImageAnalysisAllowedAttribute));
            if (detectIfImageAnalysisAllowed.Any() && detectIfImageAnalysisAllowed.Count == 1)
            {
                var getImageAnalysisAllowedProperty = detectIfImageAnalysisAllowed.FirstOrDefault();
                var getAllowedValue = getImageAnalysisAllowedProperty.Property.GetValue(getImageAnalysisAllowedProperty.Content);
                if (getAllowedValue is bool value)
                {
                    if (value)
                    {
                        var mediaData = image;
                        if (mediaData != null)
                        {
                            var fileBlob = mediaData.BinaryData as FileBlob;
                            var path = fileBlob.FilePath;
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                var analyseImage = AnalyseImage(path);
                                if (analyseImage != null)
                                {
                                    var retrieveSeverityLevelValues = GetPagePropertiesWithAttribute(startPage, typeof(SeverityLevelAttribute));
                                    if (retrieveSeverityLevelValues != null && retrieveSeverityLevelValues.Any())
                                    {
                                        foreach (var severityLevel in retrieveSeverityLevelValues)
                                        {
                                            var getValueCMS = severityLevel.Property.GetValue(severityLevel.Content);
                                            if (getValueCMS == null)
                                            {
                                                var message = string.Format("Please populate integer field {0} with an integer value to be used for the severity level processing as part of Azure AI Content Safety", severityLevel.Property.Name);
                                                return message;
                                            }
                                            else
                                            {
                                                int severityLevelCMS = int.Parse(getValueCMS.ToString());
                                                if (severityLevel.Property.Name.Contains("Sexual"))
                                                {
                                                    if (analyseImage.SexualResult.Severity > severityLevelCMS)
                                                    {
                                                        var message = string.Format("Sexual content. Severity Level Detected is : {0} and the Severity Level allowed is : {1}", analyseImage.SexualResult.Severity, severityLevelCMS);
                                                        listStrings.Add(message);
                                                    }
                                                }
                                                if (severityLevel.Property.Name.Contains("SelfHarm"))
                                                {
                                                    if (analyseImage.SelfHarmResult.Severity > severityLevelCMS)
                                                    {
                                                        var message = string.Format("Self Harm content. Severity Level Detected is : {0} and the Severity Level allowed is : {1}", analyseImage.HateResult.Severity, severityLevelCMS);
                                                        listStrings.Add(message);
                                                    }
                                                }
                                                if (severityLevel.Property.Name.Contains("Violence"))
                                                {
                                                    if (analyseImage.ViolenceResult.Severity > severityLevelCMS)
                                                    {
                                                        var message = string.Format("Violent content. Severity Level Detected is : {0} and the Severity Level allowed is : {1}", analyseImage.ViolenceResult.Severity, severityLevelCMS);
                                                        listStrings.Add(message);
                                                    }

                                                }
                                                if (severityLevel.Property.Name.Contains("Hate"))
                                                {
                                                    if (analyseImage.HateResult.Severity > severityLevelCMS)
                                                    {
                                                        var message = string.Format("Hate content. Severity Level Detected is : {0} and the Severity Level allowed is : {1} , ", analyseImage.SelfHarmResult.Severity, severityLevelCMS);
                                                        listStrings.Add(message);
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
            }
            else
            {
                if (detectIfImageAnalysisAllowed.Count > 1)
                {
                    return "Please only have 1 CMS boolean property with attribute ImageAnalysisAllowed";
                }
            }
            if (listStrings.Any())
            {
                listStrings.Insert(0, "Unable to publish Azure AI Content Safety - Image Analysis has detected:");
                listStrings.Add("Please review image and upload again.");
                result = string.Join(". ", listStrings.ToArray());
            }
            return result;
        }

        public static List<string> ProcessTextAnalysis(IContent startPage, IContent content)
        {
            var listStrings = new List<string>();

            var detectIfTextAnalysisAllowed = GetPagePropertiesWithAttribute(startPage, typeof(TextAnalysisAllowedAttribute));

            if (detectIfTextAnalysisAllowed.Any() && detectIfTextAnalysisAllowed.Count == 1)
            {
                var getFirstDefaultChosenBlocklistOption = detectIfTextAnalysisAllowed.FirstOrDefault();
                var getAllowedValue = getFirstDefaultChosenBlocklistOption.Property.GetValue(getFirstDefaultChosenBlocklistOption.Content);
                if (getAllowedValue is bool value)
                {
                    if (value)
                    {
                        var listTextAnalysed = new List<AnalyzeTextResult>();
                        var getTextAnalysisAttributesBlocklist = GetPropertiesWithAttribute(content, typeof(TextAnalysisAttribute));
                        if (getTextAnalysisAttributesBlocklist.Any() && getTextAnalysisAttributesBlocklist != null)
                        {
                            foreach (var attribute in getTextAnalysisAttributesBlocklist)
                            {
                                var checkObjectValueIsNotNull = attribute.Property.GetValue(attribute.Content, null);
                                if (checkObjectValueIsNotNull != null)
                                {
                                    var getTextAnalysisValue = attribute.Property.GetValue(attribute.Content).ToString();
                                    var analyseText = AnalyseText(getTextAnalysisValue);
                                    listTextAnalysed.Add(analyseText);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        if (listTextAnalysed.Any())
                        {
                            var retrieveSeverityLevelValues = GetPagePropertiesWithAttribute(startPage, typeof(SeverityLevelAttribute));

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
                                if (violenceResults.Any())
                                {
                                    string violenceText;
                                    if (violenceResults.Count == 1)
                                    {
                                        violenceText = string.Format("1 count of Violence related content that is about to be published");
                                        listStrings.Add(violenceText);
                                    }
                                    if (violenceResults.Count > 1)
                                    {
                                        violenceText = string.Format("{0} counts of Violence related content that is about to be published", violenceResults.Count);
                                        listStrings.Add(violenceText);
                                    }
                                }
                                if (hateResults.Any())
                                {
                                    string hateText;
                                    if (hateResults.Count == 1)
                                    {
                                        hateText = string.Format("1 count of Hate related content that is about to be published");
                                        listStrings.Add(hateText);
                                    }
                                    if (hateResults.Count > 1)
                                    {
                                        hateText = string.Format("{0} counts of Hate related content that is about to be published", hateResults.Count);
                                        listStrings.Add(hateText);
                                    }
                                }
                                if (sexualResults.Any())
                                {
                                    string sexualText;
                                    if (sexualResults.Count == 1)
                                    {
                                        sexualText = string.Format("1 count of Sexual related content that is about to be published");
                                        listStrings.Add(sexualText);
                                    }
                                    if (sexualResults.Count > 1)
                                    {
                                        sexualText = string.Format("{0} counts of Sexual related content that is about to be published", sexualResults.Count);
                                        listStrings.Add(sexualText);
                                    }
                                }
                                if (selfHarmResults.Any())
                                {
                                    string selfHarmText;
                                    if (selfHarmResults.Count == 1)
                                    {
                                        selfHarmText = string.Format("1 count of Self Harm related content that is about to be published");
                                        listStrings.Add(selfHarmText);
                                    }
                                    if (selfHarmResults.Count > 1)
                                    {
                                        selfHarmText = string.Format("{0} counts of Self Harm related content that is about to be published", selfHarmResults.Count);
                                        listStrings.Add(selfHarmText);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                if (detectIfTextAnalysisAllowed.Count > 1)
                {
                    var errorMessageList = new List<string>
                    {
                        "Please only have 1 CMS boolean property with attribute TextAnalysisAllowed"
                    };
                    return errorMessageList;
                }
            }
            return listStrings;
        }

        public static List<string> ProcessTextAnalysisBlocklist(IContent startPage, IContent content)
        {
            var listStrings = new List<string>();
            var detectIfTextAnalysisBlocklistAllowed = GetPagePropertiesWithAttribute(startPage, typeof(TextAnalysisBlocklistAllowedAttribute));

            if (detectIfTextAnalysisBlocklistAllowed.Any() && detectIfTextAnalysisBlocklistAllowed != null)
            {
                var getFirstDefaultBlocklistAllowed = detectIfTextAnalysisBlocklistAllowed.FirstOrDefault();
                var getAllowedValue = getFirstDefaultBlocklistAllowed.Property.GetValue(getFirstDefaultBlocklistAllowed.Content);
                if (getAllowedValue is bool value)
                {
                    if (value)
                    {
                        var textBlocklistMatchResult = new List<List<TextBlocklistMatchResult>>();
                        var chosenBlocklistOption = GetPropertiesWithAttribute(content, typeof(TextAnalysisBlocklistDropdown));
                        if (chosenBlocklistOption.Any() && chosenBlocklistOption != null)
                        {
                            var getFirstDefaultChosenBlocklistOption = chosenBlocklistOption.FirstOrDefault();
                            var getBlocklistOptionValue = getFirstDefaultChosenBlocklistOption.Property.GetValue(getFirstDefaultChosenBlocklistOption.Content, null);
                            if (getBlocklistOptionValue != null)
                            {
                                var getBlocklistOption = getFirstDefaultChosenBlocklistOption.Property.GetValue(getFirstDefaultChosenBlocklistOption.Content).ToString();
                                if (!string.IsNullOrWhiteSpace(getBlocklistOption))
                                {
                                    var getTextAnalysisAttributesBlocklist = GetPropertiesWithAttribute(content, typeof(TextAnalysisBlocklist));
                                    if (getTextAnalysisAttributesBlocklist.Any() && getTextAnalysisAttributesBlocklist != null)
                                    {
                                        foreach (var attribute in getTextAnalysisAttributesBlocklist)
                                        {
                                            var checkObjectValueIsNotNull = attribute.Property.GetValue(attribute.Content, null);
                                            if (checkObjectValueIsNotNull != null)
                                            {
                                                var getTextAnalysisValue = attribute.Property.GetValue(attribute.Content, null).ToString();
                                                var analyseTextBlocklist = AnalyseTextWithBlockList(getBlocklistOption, getTextAnalysisValue).ToList();
                                                if (analyseTextBlocklist.Any() && analyseTextBlocklist != null)
                                                {
                                                    textBlocklistMatchResult.Add(analyseTextBlocklist);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    if (textBlocklistMatchResult.Any() && textBlocklistMatchResult != null)
                                    {
                                        string blocklistText;
                                        if (textBlocklistMatchResult.Count == 1)
                                        {
                                            blocklistText = string.Format("1 Blocklist Match in the content published. ");
                                            listStrings.Add(blocklistText);
                                        }
                                        if (textBlocklistMatchResult.Count > 1)
                                        {
                                            blocklistText = string.Format("{0} Blocklist matches in the content published. ", textBlocklistMatchResult.Count);
                                            listStrings.Add(blocklistText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return listStrings;
        }
    }
}
