using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.Interface;
using EPiServer.Core;
using EPiServer.Logging;
using System;
using System.Collections.Generic;

namespace AzureAIContentSafety.Helpers
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
                    MarkAnalysisAsCompletedImage(imageData);
                    return analyseImage;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error analyzing image '{imageData.Name}' with content id '{imageData.ContentLink.ID}'", e);
            }
            return null;
        }

        public static AnalyzeTextResult AnalyseText(string textToAnalyse, IContent content)
        {
            try
            {
                var analyseImage = OptimizelyCmsHelpers.AnalyseText(textToAnalyse);
                if (analyseImage != null)
                {
                    MarkAnalysisAsCompletedText(content);
                    return analyseImage;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error analyzing image '{content.Name}' with content id '{content.ContentLink.ID}'", e);
            }
            return null;
        }

        public static IReadOnlyList<TextBlocklistMatchResult> AnalyseTextWithBlockList(string blockListName, string inputString, IContent content)
        {
            try
            {
                var analyseTextBlocklist = OptimizelyCmsHelpers.AnalyseTextWithBlockList(blockListName, inputString);
                if (analyseTextBlocklist != null)
                {
                    MarkAnalysisAsCompletedText(content);
                    return analyseTextBlocklist;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error analyzing image '{content.Name}' with content id '{content.ContentLink.ID}'", e);
            }
            return null;
        }

        private static void MarkAnalysisAsCompletedImage(EPiServer.Core.ImageData image)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (image is IContentAnalyse analyzableImage)
            {
                analyzableImage.ContentSafetyAnalysisCompleted = true;
            }
        }

        private static void MarkAnalysisAsCompletedText(IContent content)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (content is IContentAnalyse analyzableImage)
            {
                analyzableImage.ContentSafetyAnalysisCompleted = true;
            }
        }
    }
}
