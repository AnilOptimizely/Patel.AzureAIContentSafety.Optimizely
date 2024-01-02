using Patel.AzureAIContentSafety.Optimizely.Attributes;

namespace Patel.AzureAIContentSafety.Optimizely.ContentSafety.Attributes
{
    public class TextAnalysisBlocklistDropdown : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
    }
}
