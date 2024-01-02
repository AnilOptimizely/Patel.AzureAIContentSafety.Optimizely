using EPiServer.ServiceLocation;

namespace Patel.AzureAIContentSafety.Optimizely
{
    [ServiceConfiguration]
    public class ContentSafetyOptions
    {
        public string ContentSafetySubscriptionKey { get; set; }
        public string ContentSafetyEndpoint { get; set; }
    }
}
