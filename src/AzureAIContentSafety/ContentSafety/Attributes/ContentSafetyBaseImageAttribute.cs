using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Models;
using AzureAIContentSafety.Services;
using System.Collections;
using System.Reflection;

namespace AzureAIContentSafety.ContentSafety.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ContentSafetyBaseImageAttribute : Attribute
    {
        /// <summary>
        /// To be overridden in derived class. Flag if the Update method needs imageAnalyzerResult to be populated.
        /// </summary>
        public virtual bool AnalyzeImageContent => false;
        
        public abstract void UpdateImage(ImagePropertyAccess imagePropertyAccess, AnalyzeImageResult analyseContentSafetyImageResult, 
            AzureAIContentSafetyService azureAIContentSafetyService);

        protected static bool IsBooleanProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(bool);
        }
    }
}
