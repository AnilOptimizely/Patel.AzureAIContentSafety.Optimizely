using Azure.AI.ContentSafety;
using Patel.AzureAIContentSafety.Optimizely.Models;
using Patel.AzureAIContentSafety.Optimizely.Services;
using Patel.AzureAIContentSafety.Optimizely.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patel.AzureAIContentSafety.Optimizely.ContentSafety.Attributes
{
    public class TextAnalysisAttribute : ContentSafetyBaseContentAttribute
    {
        public override bool AnalyzeCMSContent => true;
    }
}
