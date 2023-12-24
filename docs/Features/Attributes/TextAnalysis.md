# [TextAnalysis]

The following attribute enables users to incorporate a string property within the CMS, which will undergo moderation via the Text Moderation API as an integral feature of the Azure AI Content Safety Service. 
Upon publication of the content in the CMS, each property to which this attribute is applied will be processed through the Azure AI Content Safety - Text Moderation API.

The attribute may be applied to the following property types:

- **String:** String value indicating the content being used for the Text Analysis feature.

The attribute can exclusively be added to string properties and is applicable to any content type derived from IContent (the base content type in Optimizely CMS).

**Example**
``` C#
public class StandardPage : SitePageData
{
  [Display(
      GroupName = SystemTabNames.Content,
      Name = "Text Analysis Text",
      Description = "Text used for the Azure AI Content Safety - Text Moderation",
      Order = 10)]
  [CultureSpecific]
  [UIHint(UIHint.Textarea)]
  [TextAnalysis]
  public virtual string TextAnalysisText { get; set; }
}
```
Screenshot of Attribute being used
