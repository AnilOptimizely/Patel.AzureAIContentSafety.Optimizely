using Azure.AI.ContentSafety;
using Patel.AzureAIContentSafety.Optimizely.Models;
using Patel.AzureAIContentSafety.Optimizely.Services;
using Patel.AzureAIContentSafety.Optimizely.Attributes;

namespace Patel.AzureAIContentSafety.Optimizely.ContentSafety.Attributes
{
    public class TextAnalysisBlocklistAllowedAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
    }
}
