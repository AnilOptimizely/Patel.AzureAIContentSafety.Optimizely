using System.Collections.Generic;

namespace Patel.AzureAIContentSafety.Optimizely.Models
{
    public class AzureAIContentSafetyBlockListItems
    {
        public string BlockListName { get; set; }
        public List<string> BlockItems { get; set; }
    }
}
