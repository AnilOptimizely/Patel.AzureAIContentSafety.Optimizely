using Azure.AI.ContentSafety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patel.AzureAIContentSafety.Optimizely.Attributes
{
    public class ImageAnalysisAllowedAttribute : ContentSafetyBaseImageAttribute
    {
        public override bool AnalyzeImageContent => true;
    }
}
