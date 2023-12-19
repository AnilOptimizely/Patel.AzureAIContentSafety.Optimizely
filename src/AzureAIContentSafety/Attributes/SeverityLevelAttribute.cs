using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Models;
using AzureAIContentSafety.Models;
using AzureAIContentSafety.Services;
namespace AzureAIContentSafety.Attributes
{
    public class SeverityLevelAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
        public override void UpdateContent(AnalyzeTextResult analyseContentSafetyTextResult, AzureAIContentSafetyService azureAIContentSafetyService, ContentPropertyAccess contentPropertyAccess)
        {

        }
    }
}
