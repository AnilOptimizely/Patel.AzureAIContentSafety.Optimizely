
using Patel.AzureAIContentSafety.Optimizely.Attributes;

namespace Patel.AzureAIContentSafety.Optimizely.ContentSafety.Attributes
{
    public class TextAnalysisBlocklistAllowedAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
    }
}
