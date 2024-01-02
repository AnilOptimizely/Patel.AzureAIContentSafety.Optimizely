namespace Patel.AzureAIContentSafety.Optimizely.Attributes
{
    public class ImageAnalysisAllowedAttribute : ContentSafetyBaseImageAttribute
    {
        public override bool AnalyzeImageContent => true;
    }
}
