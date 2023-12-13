using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Mvc.Rendering;
using AzureAIContentSafety.Interfaces;
using EPiServer.ServiceLocation;
using EPiServer;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Models;
using EPiServer.Core;
using System.Reflection;
using AzureAIContentSafety.Services;
using System.IO;
using Azure;
using ImageData = Azure.AI.ContentSafety.ImageData;

namespace AzureAIContentSafety.Helpers
{
    public class OptimizelyCmsHelpers
    {
        protected readonly Injected<IAzureAIContentSafetyService> _azureContentSafetyService;
        private readonly IContentLoader _contentLoader;

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

        public static ContentSafetyClient GetClient()
        {
            ContentSafetyClient client = new(new Uri(""), new AzureKeyCredential(""));
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
