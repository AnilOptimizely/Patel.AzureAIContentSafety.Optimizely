using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AzureAIContentSafety.Models.ViewModels
{
    public class BlockListViewModel
    {
        public List<SelectListItem> GetTextBlocklistsPages { get; set; }
        public List<SelectListItem> GetBlockItems { get; set; }
        public string DeleteItem { get; set; }
        public string BlockListName { get; set; }
        public string BlockListDescription { get; set; }
        public string TextBlockItemInfo { get; set; }
        public ContentSafetyModel ContentSafetyModel { get; set; }
        public List<TextBlocklist> BlockLists { get; set; }
    }
}
