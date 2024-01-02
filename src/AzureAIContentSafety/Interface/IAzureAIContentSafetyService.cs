using Azure.AI.ContentSafety;
using Patel.AzureAIContentSafety.Optimizely.Models;
using System.Collections.Generic;

namespace Patel.AzureAIContentSafety.Optimizely.Interface
{
    public interface IAzureAIContentSafetyService
    {
        AnalyzeImageResult AnalyseImage(string imageFilePath);
        AnalyzeTextResult AnalyseText(string textToAnalyze);
        List<TextBlocklist> GetBlocklists();
        string CreateNewBlockList(string blockListName, string blockListDescription);
        List<string> AddNewBlockItems(string blockListName, List<TextBlockItemInfo> textBlocks);
        List<string> AnalyseTextBlockList(string blockListName, string inputString);
        string GetBlockList(string blockListName);
        IReadOnlyList<TextBlocklistMatchResult> AnalyseTextWithBlockList(string blockListName, string inputString);
        AzureAIContentSafetyBlockListItems GetBlockItems(string blockListName);
        AzureAIContentSafetyBlockListItems GetBlockItemsDropdown(string blockListName);
        string DeleteBlockList(string blockListName);
        string GetBlockListItem(string blockListName, string itemId);
    }
}
