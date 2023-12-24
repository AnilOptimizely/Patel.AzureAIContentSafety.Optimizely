# [TextAnalysisAllowed]

This attribute will utilise the Text Detection API, a component of the Text Moderation feature within the Azure AI Content Safety service, to analyse text for various types of inappropriate content during the publishing of content within Optimizely CMS. 

These categories include sexual content, violence, hate speech, and self-harm. The moderation process incorporates multiple severity levels. This attribute has been designed to be easily toggled on or off within the CMS at any given time. 

It may be applied to the following property types:

- **Bool:** True/false indicating if Text Anaysis is able to be used.
  
The attribute can only be appended to bool properties and is intended for use exclusively on a Start Page. 

**Example**
``` C#
public class StartPage : SitePageData
{
  [Display(GroupName = SystemTabNames.Content,
      Order = 10,
      Description = "Boolean to determine if Text Detection API for Azure AI Content Safety is allowed",
      Name = "Text Analysis Allowed")]
  [TextAnalysisAllowed]
  public virtual bool AnalyseText { get; set; }
}
```
Screenshot of Property being used
