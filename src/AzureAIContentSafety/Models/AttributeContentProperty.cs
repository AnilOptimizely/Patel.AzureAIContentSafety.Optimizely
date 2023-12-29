using Patel.AzureAIContentSafety.Optimizely.Attributes;
using System.Reflection;

namespace Patel.AzureAIContentSafety.Optimizely.Models
{
    public class AttributeContentProperty
    {
        public ContentSafetyBaseImageAttribute Attribute { get; set; }
        public object Content { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
