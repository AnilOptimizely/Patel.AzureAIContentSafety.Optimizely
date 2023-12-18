using AzureAIContentSafety.ContentSafety.Attributes;
using AzureAIContentSafety.Models.AzureAIContentSafety;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace AlloyMVC.Models.Pages;

/// <summary>
/// Used for the pages mainly consisting of manually created content such as text, images, and blocks
/// </summary>
[SiteContentType(GUID = "9CCC8A41-5C8C-4BE0-8E73-520FF3DE8267")]
[SiteImageUrl(Globals.StaticGraphicsFolderPath + "page-type-thumbnail-standard.png")]
public class StandardPage : SitePageData
{
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 310)]
    [CultureSpecific]
    public virtual XhtmlString MainBody { get; set; }

    [Display(
        GroupName = SystemTabNames.Content,
        Name = "Azure Content Safety Text",
        Order = 120)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    [TextAnalysis]
    public virtual string AnalyseText { get; set; }

    [Display(
        GroupName = SystemTabNames.Content,
        Name = "Azure Content Safety Text Two",
        Order = 125)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    [TextAnalysis]
    public virtual string AnalyseTextTwo {  get; set; }

    [SelectOne(SelectionFactoryType = typeof(BlockListSelectionFactory))]
    [Display(Name = "Select an Azure Content Safety Block List",
            GroupName = SystemTabNames.Content,
            Description = "Select an Azure Content Safety Block List to associate with the page.",
            Order = 11)]
    [TextAnalysisBlocklistDropdown]
    public virtual string BlockList { get; set; }

    [Display(
        GroupName = SystemTabNames.Content,
        Order = 320)]
    public virtual ContentArea MainContentArea { get; set; }
}
