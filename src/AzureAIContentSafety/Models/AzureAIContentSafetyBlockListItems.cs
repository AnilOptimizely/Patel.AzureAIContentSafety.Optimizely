using System.Collections.Generic;

namespace AzureAIContentSafety.ContentSafety.Models
{
    public class AzureAIContentSafetyBlockListItems
    {
        public string BlockListName { get; set; }
        public List<string> BlockItems { get; set; }
    }
}
