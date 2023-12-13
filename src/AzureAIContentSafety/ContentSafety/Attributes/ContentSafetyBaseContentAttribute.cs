using Azure.AI.ContentSafety;
using AzureAIContentSafety.ContentSafety.Models;
using AzureAIContentSafety.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIContentSafety.ContentSafety.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ContentSafetyBaseContentAttribute : Attribute
    {
        public virtual bool AnalyzeCMSContent => false;

        public abstract void UpdateContent(AnalyzeTextResult analyseContentSafetyTextResult,
            AzureAIContentSafetyService azureAIContentSafetyService, ContentPropertyAccess contentPropertyAccess);

        protected static bool IsBooleanProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(bool);
        }

        protected static bool IsIntProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(int);
        }

        protected static bool IsDoubleProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(double);
        }

        protected static bool IsStringProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(string);
        }

        protected static bool IsStringListProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(IList<string>) ||
                   propertyInfo.PropertyType == typeof(IEnumerable<string>) ||
                   propertyInfo.PropertyType == typeof(ICollection);
        }
    }
}
