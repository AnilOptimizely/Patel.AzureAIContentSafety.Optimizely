using System.Reflection;
using EPiServer.Core;

namespace Patel.AzureAIContentSafety.Optimizely.Models
{
    public class ImagePropertyAccess
    {
        private readonly object _content;

        public ImagePropertyAccess(ImageData image, object content, PropertyInfo propertyInfo)
        {
            _content = content;
            Image = image;
            Property = propertyInfo;
        }

        public void SetValue(object value)
        {
            Property.SetValue(_content, value);
        }

        public object GetValue()
        {
            return Property.GetValue(_content);
        }

        public ImageData Image { get; }

        public PropertyInfo Property { get; }
    }
}
