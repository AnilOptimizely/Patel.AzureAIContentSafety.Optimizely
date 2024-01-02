using Patel.AzureAIContentSafety.Optimizely.Attributes;

namespace Patel.AzureAIContentSafety.Optimizely.ContentSafety.Attributes
{
    public class TextAnalysisAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
    }
}
