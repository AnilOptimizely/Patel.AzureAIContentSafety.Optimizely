using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Web;

namespace AzureAIContentSafety.AddOn
{
    public sealed class BlocklistQuickNavigator : IQuickNavigatorItemProvider
    {
        public int SortOrder
        {
            get { return 130; }
        }

        public IDictionary<string, QuickNavigatorMenuItem> GetMenuItems(ContentReference currentContent)
        {
            var dictionary = new Dictionary<string, QuickNavigatorMenuItem>();

            if (PrincipalInfo.CurrentPrincipal.IsInRole("CmsAdmins"))
            {
                dictionary.Add("azurecontentsafetyblocklist", new QuickNavigatorMenuItem("Azure Content Safety - BlockList", "episerver/cms/azurecontentsafetyblocklist", null, "true", null));
            }

            return dictionary;
        }
    }
}
