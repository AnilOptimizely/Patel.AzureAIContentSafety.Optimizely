using System.Reflection;

namespace Patel.AzureAIContentSafety.Optimizely.Models
{
    public class ContentProperty
    {
        public object Content { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
