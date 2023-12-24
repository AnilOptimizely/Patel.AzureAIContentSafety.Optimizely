# [ImageAnalysisAllowed]

 This attribute will use the Image Detection API, as part of the Image Moderation feature within the Azure AI Content Safety service, to scan images for various categories of harmful content when uploading images within Optimizely CMS. 
 
 These categories consist of; sexual content, violence, hate, and self harm. It is moderated using multi-severity levels.  This attribute has been developed in a way where it can be turned on and off at any time in the CMS.

May be added to the following property types:

- **Bool:** True/false indicating if Image Anaysis is able to be used.
  
The attribute can only be added to Boolean properties and used on a Start Page. 

**Example**
``` C#
public class StartPage : SitePageData
{
   [Display(GroupName = SystemTabNames.Content,
   Order = 10,
   Description = "Boolean to determine if Image Detection API for Azure AI Content Safety is allowed",
   Name = "Image Analysis Allowed")]
   [ImageAnalysisAllowed]
   public virtual bool AnalyseImages { get; set; }
}
```
Screenshot of Property being used

