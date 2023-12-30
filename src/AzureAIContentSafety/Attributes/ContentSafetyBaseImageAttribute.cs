using System;
using System.Reflection;

namespace Patel.AzureAIContentSafety.Optimizely.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ContentSafetyBaseImageAttribute : Attribute
    {
        /// <summary>
        /// To be overridden in derived class. Flag if the Update method needs imageAnalyzerResult to be populated.
        /// </summary>
        public virtual bool AnalyzeImageContent => false;

        protected static bool IsBooleanProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(bool);
        }
    }
}
