using Azure.AI.ContentSafety;
using Patel.AzureAIContentSafety.Optimizely.Attributes;

namespace Patel.AzureAIContentSafety.Optimizely.Attributes
{
    public class SeverityLevelAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
        
    }
}
