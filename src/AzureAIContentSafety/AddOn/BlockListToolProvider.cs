using EPiServer.Authorization;
using EPiServer.Shell.Navigation;

namespace AzureAIContentSafety.AddOn
{
    [MenuProvider]
    public sealed class BlockListToolProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var menuItems = new List<MenuItem>
            {
                new UrlMenuItem("BlockList",
                MenuPaths.Global + "/cms/azurecontentsafetyblocklist",
                "/AzureContentSafetyBlockList")
                {
                    SortIndex = SortIndex.Last + 25,
                    AuthorizationPolicy = CmsPolicyNames.CmsAdmin,
                }
            };
            return menuItems;
        }
    }
}
