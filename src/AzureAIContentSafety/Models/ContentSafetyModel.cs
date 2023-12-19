
using AzureAIContentSafety.ContentSafety.Models;

namespace AzureAIContentSafety.Models.ViewModels
{
    public class ContentSafetyModel
    {
        public string Message { get; set; }
        public AzureAIContentSafetyBlockListItems BlockItems { get; set; }
    }
}
