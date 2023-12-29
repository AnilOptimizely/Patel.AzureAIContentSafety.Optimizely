namespace Patel.AzureAIContentSafety.Optimizely.Models
{
    public class ContentSafetyModel
    {
        public string Message { get; set; }
        public AzureAIContentSafetyBlockListItems BlockItems { get; set; }
    }
}
