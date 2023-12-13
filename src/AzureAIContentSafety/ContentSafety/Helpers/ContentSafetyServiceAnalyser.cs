using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Interface;
using AzureAIContentSafety.Helpers;
using EPiServer.Core;
using EPiServer.Logging;

namespace AzureAIContentSafety.ContentSafety.Helpers
{
    public static class ContentSafetyServiceAnalyser
    {
        private static readonly ILogger Log = LogManager.GetLogger();

        public static AnalyzeImageResult AnalyzeImage(string imageFilePath, EPiServer.Core.ImageData imageData)
        {
            try
            {
                var analyseImage = OptimizelyCmsHelpers.AnalyseImage(imageFilePath);
                if (analyseImage != null)
                {
                    MarkAnalysisAsCompleted(imageData);
                    return analyseImage;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error analyzing image '{imageData.Name}' with content id '{imageData.ContentLink.ID}'", e);
            }
            return null;
        }

        //public static AnalyzeTextResult AnalyseText()
        //{
        //    try
        //    {
        //        var analyseImage = OptimizelyCmsHelpers.AnalyseImage(imageFilePath);
        //        if (analyseImage != null)
        //        {
        //            MarkAnalysisAsCompleted(imageData);
        //            return analyseImage;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error($"Error analyzing image '{imageData.Name}' with content id '{imageData.ContentLink.ID}'", e);
        //    }
        //    return null;
        //}

        private static void MarkAnalysisAsCompleted(EPiServer.Core.ImageData image)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (image is IContentAnalyse analyzableImage)
            {
                analyzableImage.ContentSafetyAnalysisCompleted = true;
            }
        }
    }
}
