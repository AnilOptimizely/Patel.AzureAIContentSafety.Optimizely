using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Web;

namespace AzureAIContentSafety.Business.Plugins
{
    public class BlocklistQuickNavigator : IQuickNavigatorItemProvider
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
                dictionary.Add("blocklist", new QuickNavigatorMenuItem("Azure Content Safety - BlockList", "/EPiServer/CMS/blocklist/", null, "true", null));
            }

            return dictionary;
        }
    }
}
