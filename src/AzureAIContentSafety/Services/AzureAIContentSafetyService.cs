using Azure;
using Azure.AI.ContentSafety;
using ImageData = Azure.AI.ContentSafety.ImageData;
using Azure.Core;
using AzureAIContentSafety.Helpers;
using Microsoft.Extensions.Options;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using Patel.AzureAIContentSafety.Optimizely.Interface;
using Patel.AzureAIContentSafety.Optimizely.Models;

namespace Patel.AzureAIContentSafety.Optimizely.Services
{
    public class AzureAIContentSafetyService : IAzureAIContentSafetyService
    {
        private static IOptions<ContentSafetyOptions> _configuration;
        private static IOptions<ContentSafetyOptions> Configuration => _configuration ??= ServiceLocator.Current.GetInstance<IOptions<ContentSafetyOptions>>();
        private static readonly string ContentSafetySubscriptionKey = Configuration.Value.ContentSafetySubscriptionKey;
        private static readonly string ContentSafetyEndpoint = Configuration.Value.ContentSafetyEndpoint;

        public static ContentSafetyClient GetClient()
        {
            var endpoint = ContentSafetyEndpoint;
            var key = ContentSafetySubscriptionKey;

            ContentSafetyClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
            return client;
        }

        public AnalyzeImageResult AnalyseImage(string imageFilePath)
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

        public AnalyzeTextResult AnalyseText(string textToBeAnalysed)
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

        public List<string> AnalyseTextBlockList(string blockListName, string inputString)
        {
            var list = new List<string>();
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
                    foreach (var matchResult in response.Value.BlocklistsMatchResults)
                    {
                        var itemStringMatchResultOne = string.Format("Azure AI Content Safety - Blocklist match result: Blockitem was hit in text: BlockItemText: {0} - BlockListName: {1} ",
                            matchResult.BlocklistName,
                            matchResult.BlockItemText);
                        list.Add(itemStringMatchResultOne);
                    }
                }
            }
            catch (RequestFailedException ex)
            {
                var itemStringMatchResultOne = string.Format("Analyze text failed.\nStatus code: {0}, Error code: {1}, Error message: {2}", ex.Status, ex.ErrorCode, ex.Message);
                list.Add(itemStringMatchResultOne);
            }
            return list;
        }

        public IReadOnlyList<TextBlocklistMatchResult> AnalyseTextWithBlockList(string blockListName, string inputString)
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

        public List<TextBlocklist> GetBlocklists()
        {
            var blocklists = GetClient().GetTextBlocklists().ToList();
            if (blocklists != null && blocklists.Count != 0)
            {
                Console.WriteLine("Azure AI Content Safety -  Blocklists Retrieval operation complete");
                foreach (var blocklist in blocklists)
                {
                    Console.WriteLine($"  BlockList Name: {blocklist.BlocklistName}");
                    Console.WriteLine($"  Blocklist Description: {blocklist.Description}");
                }
            }
            return blocklists;
        }

        public string CreateNewBlockList(string blockListName, string blockListDescription)
        {
            if (!string.IsNullOrWhiteSpace(blockListName) && !string.IsNullOrWhiteSpace(blockListDescription))
            {
                var data = new
                {
                    description = blockListDescription,
                };

                var createResponse = GetClient().CreateOrUpdateTextBlocklist(blockListName, RequestContent.Create(data));

                if (createResponse.Status == 201)
                {
                    Console.WriteLine("Azure AI Content Safety -  Create New Blocklist operation complete");
                    Console.WriteLine($"  Blocklist Name: {blockListName}");
                    Console.WriteLine($"  Blocklist Description: {blockListName}");
                    return string.Format("\nBlocklist: {0} created. Description of the Blocklist is: {1}", blockListName, blockListDescription);
                }
                else if (createResponse.Status == 200)
                {
                    Console.WriteLine("Azure AI Content Safety -  Updating of Blocklist operation complete");
                    Console.WriteLine($"  Blocklist Name: {blockListName}");
                    Console.WriteLine($"  Blocklist Description: {blockListName}");
                    return string.Format("\nBlocklist: {0} updated.", blockListName);
                }
            }
            return "";
        }

        public List<string> AddNewBlockItems(string blockListName, List<TextBlockItemInfo> textBlocks)
        {
            var listItems = new List<string>();
            if (!string.IsNullOrWhiteSpace(blockListName))
            {
                var addedBlockItems = GetClient().AddBlockItems(blockListName, new AddBlockItemsOptions(textBlocks));

                if (addedBlockItems != null && addedBlockItems.Value != null)
                {
                    Console.WriteLine("Azure AI Content Safety -  Add New Block Items operation complete");
                    foreach (var addedBlockItem in addedBlockItems.Value.Value)
                    {
                        Console.WriteLine($"  Blocklist Name: {addedBlockItem.Text}");
                        Console.WriteLine($"  Blocklist Description: {addedBlockItem.Description}");
                        Console.WriteLine($"  BlockItemId: {addedBlockItem.BlockItemId}");
                        var itemString = string.Format("Block Items Added : Block List Name: {0}, Block List Item Name: {1}, BlockItemId: {2}", blockListName, addedBlockItem.Text, addedBlockItem.BlockItemId);
                        listItems.Add(itemString);
                    }
                }

            }
            return listItems;
        }

        public string GetBlockList(string blockListName)
        {
            // Example: get blocklist

            var getBlocklist = GetClient().GetTextBlocklist(blockListName);
            if (getBlocklist != null && getBlocklist.Value != null)
            {
                return string.Format("Get blocklist: BlocklistName: {0}, Description: {1}", getBlocklist.Value.BlocklistName, getBlocklist.Value.Description);

            }
            return "Unable to retrieve BlockList";
        }

        public AzureAIContentSafetyBlockListItems GetBlockItems(string blockListName)
        {
            var model = new AzureAIContentSafetyBlockListItems
            {
                BlockItems = []
            };
            var allBlockitems = GetClient().GetTextBlocklistItems(blockListName);
            if (allBlockitems.Any())
            {
                Console.WriteLine("Azure AI Content Safety -  GetTextBlocklistItems operation complete");
                Console.WriteLine("TextBlocklist Items Count is {0}", allBlockitems.Count());
                foreach (var blocklistItem in allBlockitems)
                {
                    Console.WriteLine($"  BlockItemId: {blocklistItem.BlockItemId}");
                    Console.WriteLine($"  Blocklist Text: {blocklistItem.Text}");
                    model.BlockItems.Add(string.Format("BlockItemId: {0}, Blocklist Text: {1}", blocklistItem.BlockItemId, blocklistItem.Text));
                }
                model.BlockListName = blockListName;
            }
            return model;
        }
        public AzureAIContentSafetyBlockListItems GetBlockItemsDropdown(string blockListName)
        {
            var model = new AzureAIContentSafetyBlockListItems
            {
                BlockItems = []
            };
            var allBlockitems = GetClient().GetTextBlocklistItems(blockListName);
            if (allBlockitems.Any())
            {
                Console.WriteLine("Azure AI Content Safety -  GetTextBlocklistItems operation complete");
                Console.WriteLine("TextBlocklist Items Count is {0}", allBlockitems.Count());
                foreach (var blocklistItem in allBlockitems)
                {
                    Console.WriteLine($"  BlockItemId: {blocklistItem.BlockItemId}");
                    Console.WriteLine($"  Blocklist Text: {blocklistItem.Text}");
                    var dropdownItemsList = new List<string>
                    {
                        blocklistItem.BlockItemId,
                        blocklistItem.Text
                    };
                    model.BlockListName = blocklistItem.Text;
                    var result = string.Join("+", dropdownItemsList);
                    Console.WriteLine($"  result: {result}");
                    model.BlockItems.Add(result);
                }
            }
            return model;
        }

        public string DeleteBlockList(string blockListName)
        {
            if (!string.IsNullOrWhiteSpace(blockListName))
            {
                // Example: get blocklist
                var deleteResult = GetClient().DeleteTextBlocklist(blockListName);
                if (deleteResult != null && deleteResult.Status == 204)
                {
                    Console.WriteLine("Azure AI Content Safety -  Remove Block List Items operation complete");
                    Console.WriteLine($"  Deleted blocklist: {blockListName}");
                    return string.Format("Deleted blocklist: {0},", blockListName);

                }

            }
            return "Unable to delete BlockList";
        }

        public string GetBlockListItem(string blockListName, string itemId)
        {
            var getBlockItem = GetClient().GetTextBlocklistItem(blockListName, itemId);
            return string.Format("BlockItemId: {0}, Text: {1}, Description: {2}", getBlockItem.Value.BlockItemId, getBlockItem.Value.Text, getBlockItem.Value.Description);
        }
    }
}
