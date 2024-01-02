# [ImageAnalysisAllowed]

This attribute will utilise the Image Detection API, a component of the Image Moderation feature within the Azure AI Content Safety service, to analyse images for various types of inappropriate content during the image upload process within Optimizely CMS. 

These categories include sexual content, violence, hate speech, and self-harm. The moderation process incorporates multiple severity levels. This attribute has been designed to be easily toggled on or off within the CMS at any given time. 

It may be applied to the following property types:

- **Bool:** True/false indicating if Image Anaysis is able to be used.
  
The attribute can only be appended to bool properties and is intended for use exclusively on a Start Page. 

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

**Screenshot of Boolean property being used with the ImageAnalysisAllowed attribute**
![](/docs/Features/Images/ImageAnalysisAllowedAttribute.JPG)

