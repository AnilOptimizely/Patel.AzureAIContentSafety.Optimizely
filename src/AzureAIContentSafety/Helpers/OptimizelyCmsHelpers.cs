using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Mvc.Rendering;
using EPiServer.ServiceLocation;
using EPiServer;
using AzureAIContentSafety.ContentSafety.Models;
using EPiServer.Core;
using System.Reflection;
using Azure;
using ImageData = Azure.AI.ContentSafety.ImageData;
using Microsoft.Extensions.Options;
using EPiServer.Logging;
using AzureAIContentSafety.Attributes;
using AzureAIContentSafety.Interface;

namespace AzureAIContentSafety.Helpers
{
    public class OptimizelyCmsHelpers
    {
        protected readonly Injected<IAzureAIContentSafetyService> _azureContentSafetyService;
        private readonly IContentLoader _contentLoader;
        private  static IOptions<ContentSafetyOptions> _configuration;
        private static IOptions<ContentSafetyOptions> Configuration => _configuration ??= ServiceLocator.Current.GetInstance<IOptions<ContentSafetyOptions>>();

        private  static readonly string ContentSafetySubscriptionKey = Configuration.Value.ContentSafetySubscriptionKey;
        private  static readonly string ContentSafetyEndpoint = Configuration.Value.ContentSafetyEndpoint;
        private  readonly ILogger Log = LogManager.GetLogger();

        public OptimizelyCmsHelpers(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

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
            ImageData image = new ImageData() { Content = BinaryData.FromBytes(File.ReadAllBytes(imageFilePath)) };
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
    }
}
