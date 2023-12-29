using System.Reflection;

namespace AzureAIContentSafety.ContentSafety.Models
{
    public class ContentProperty
    {
        public object Content { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
