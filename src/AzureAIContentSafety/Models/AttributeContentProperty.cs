using AzureAIContentSafety.Attributes;
using AzureAIContentSafety.ContentSafety.Attributes;
using System.Reflection;

namespace AzureAIContentSafety.ContentSafety.Models
{
    public class AttributeContentProperty
    {
        public ContentSafetyBaseImageAttribute Attribute { get; set; }
        public object Content { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
