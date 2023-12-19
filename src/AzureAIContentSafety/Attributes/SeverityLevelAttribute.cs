using Azure.AI.ContentSafety;
using AzureAIContentSafety.Models;
using AzureAIContentSafety.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
