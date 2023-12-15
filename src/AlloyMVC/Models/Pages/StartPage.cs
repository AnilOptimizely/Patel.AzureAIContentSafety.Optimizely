using System.ComponentModel.DataAnnotations;
using AlloyMVC.Models.Blocks;
using AzureAIContentSafety.ContentSafety.Attributes;
using EPiServer.SpecializedProperties;

namespace AlloyMVC.Models.Pages;

/// <summary>
/// Used for the site's start page and also acts as a container for site settings
/// </summary>
[ContentType(
    GUID = "19671657-B684-4D95-A61F-8DD4FE60D559",
    GroupName = Globals.GroupNames.Specialized)]
[SiteImageUrl]
[AvailableContentTypes(
    Availability.Specific,
    Include = new[]
    {
        typeof(ContainerPage),
        typeof(ProductPage),
        typeof(StandardPage),
        typeof(ISearchPage),
        typeof(LandingPage),
        typeof(ContentFolder) }, // Pages we can create under the start page...
    ExcludeOn = new[]
    {
        typeof(ContainerPage),
        typeof(ProductPage),
        typeof(StandardPage),
        typeof(ISearchPage),
        typeof(LandingPage)
    })] // ...and underneath those we can't create additional start pages
public class StartPage : SitePageData
{
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 320)]
    [CultureSpecific]
    public virtual ContentArea MainContentArea { get; set; }

    [Display(GroupName = SystemTabNames.Content,
    Order = 15,
    Name = "Hate Result Severity Level")]
    [SeverityLevel]
    public virtual int HateResultSeverityLevel { get; set; }

    [Display(GroupName = SystemTabNames.Content,
        Order = 20,
        Name = "Sexual Result Severity Level")]
    [SeverityLevel]
    public virtual int SexualResultSeverityLevel { get; set; }

    [Display(GroupName = SystemTabNames.Content,
        Order = 25,
        Name = "Violence Result Severity Level")]
    [SeverityLevel]
    public virtual int ViolenceResultSeverityLevel { get; set; }

    [Display(GroupName = SystemTabNames.Content,
        Order = 30,
        Name = "Self Harm Result Severity Level")]
    [SeverityLevel]
    public virtual int SelfHarmResultSeverityLevel { get; set; }

    [Display(GroupName = SystemTabNames.Content,
        Order = 40,
        Description = "Boolean to determine if Text Detection API for Azure AI Content Safety is allowed",
        Name = "Text Analysis Allowed")]
    [TextAnalysisAllowed]
    public virtual bool AnalyseText { get; set; }

    [Display(GroupName = SystemTabNames.Content,
       Order = 45,
       Description = "Boolean to determine if Image Detection API for Azure AI Content Safety is allowed",
       Name = "Image Analysis Allowed")]
    [ImageAnalysisAllowed]
    public virtual bool AnalyseImages { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings, Order = 300)]
    public virtual LinkItemCollection ProductPageLinks { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings, Order = 350)]
    public virtual LinkItemCollection CompanyInformationPageLinks { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings, Order = 400)]
    public virtual LinkItemCollection NewsPageLinks { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings, Order = 450)]
    public virtual LinkItemCollection CustomerZonePageLinks { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings)]
    public virtual PageReference GlobalNewsPageLink { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings)]
    public virtual PageReference ContactsPageLink { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings)]
    public virtual PageReference SearchPageLink { get; set; }

    [Display(GroupName = Globals.GroupNames.SiteSettings)]
    public virtual SiteLogotypeBlock SiteLogotype { get; set; }
}
