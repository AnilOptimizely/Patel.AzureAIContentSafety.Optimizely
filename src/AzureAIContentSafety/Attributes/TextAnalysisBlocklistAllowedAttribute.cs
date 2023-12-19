using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Models;
using AzureAIContentSafety.Services;

namespace AzureAIContentSafety.ContentSafety.Attributes
{
    public class TextAnalysisBlocklistAllowedAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
        public override void UpdateContent(AnalyzeTextResult analyseContentSafetyTextResult, AzureAIContentSafetyService azureAIContentSafetyService, ContentPropertyAccess contentPropertyAccess)
        {
            
        }
    }
}
