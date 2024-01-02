# [TextAnalysisBlocklistDropdown]

The following attribute enables users to choose a blocklist, which will be used to moderate content that is being published in the CMS as part of the Text Detection API.

The attribute may be applied to the following property types:

- **String:** String value indicating the blocklist to be used for the Text Moderation API.

The attribute can exclusively be added to a string property and is applicable to any content type derived from IContent (the base content type in Optimizely CMS). It is advised that this attribute should only be used once per content type. 
This attribute should be used alongside the [TextAnalysisBlocklist] attribute, as the value from this attribute is used for the Text Moderation API along with the blocklist value which is chosen from this attribute.


**Example**
``` C#
public class StandardPage : SitePageData
{
  [SelectOne(SelectionFactoryType = typeof(BlockListSelectionFactory))]
      [Display(Name = "Select an Azure Content Safety Block List",
        GroupName = SystemTabNames.Content,
        Description = "Select an Azure Content Safety Block List to associate with the page.",
        Order = 11)]
  [TextAnalysisBlocklistDropdown]
  public virtual string BlockList { get; set; }
}
```

**Screenshot of Dropdown being used with the TextAnalysisBlocklistDropdown Attribute being used**
![TextAnalysisBlocklistDropdown](/docs/Features/Images/BlockListDropdown.jpg)
