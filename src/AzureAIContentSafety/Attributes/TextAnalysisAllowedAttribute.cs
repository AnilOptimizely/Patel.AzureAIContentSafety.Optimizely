namespace Patel.AzureAIContentSafety.Optimizely.Attributes
{
    public class TextAnalysisAllowedAttribute : ContentSafetyBaseContentAttribute
    {

        public override bool AnalyzeCMSContent => true;
    }
}
