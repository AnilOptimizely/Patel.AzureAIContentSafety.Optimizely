# [TextAnalysisBlocklistAllowed]

The following attribute will utilise the Text Detection API, a component of the Text Moderation feature within the Azure AI Content Safety service. Its purpose is to analyse whether text being published within Optimizely CMS matches custom blocklists created by users. This enhancement aims to broaden the coverage of harmful content, making it more specific to individual user needs. 

Blocklists can be created by using a custom Add-On which is within this package. The creation of new blocklists can also be done in the the Azure Content Safety Studio (https://contentsafety.cognitive.azure.com/text). Please note that a Content Safety resource is a prerequisite for this process. 

This attribute has been designed to be easily toggled on or off within the CMS at any given time. 

It may be applied to the following property types:

- **Bool:** True/false indicating if Text Analysis for Blocklists is able to be used.
  
The attribute can only be appended to bool properties and is intended for use exclusively on a Start Page. 

**Example**
``` C#
public class StartPage : SitePageData
{
  [Display(GroupName = SystemTabNames.Content,
       Order = 10,
       Description = "Boolean to determine if Text Detection API for Azure AI Content Safety is allowed to be used, using Custom Blocklists",
       Name = "Blocklist Allowed")]
  [TextAnalysisBlocklistAllowed]
  public virtual bool BlocklistAllowed { get; set; }
}
```
Screenshot of Attribute being used
