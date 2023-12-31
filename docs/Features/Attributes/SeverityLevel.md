# [SeverityLevel]

As part of the Image and Text Analysis features which are part of the Azure AI Content Safety platform. 

This attribute allows a user to configure severity levels for each harmful category. Each request from the Azure AI Content Safety service, comes back with a severity level of how harmful the content or image is. This value is then compared to the int value configured in the CMS to determine whether the content/image is able to be published successfully in the CMS.

This int value needs to be filled in with a number value between 1-6, The lower the value, the more harmful content will be allowed to be published in the CMS. For more information about the severity levels, please visit this link : https://learn.microsoft.com/en-us/azure/ai-services/content-safety/concepts/harm-categories?tabs=definitions

The attribute may be applied to the following property types:

- **Int:** Int value indicating the severity level allowed for a category.

The attribute can only be appended to int properties and is intended for use exclusively on a Start Page. 
When creating these properties, the name of the property also needs to contain the category name (Sexual, SelfHarm,Violence, Hate) so that the CMS system can find the value, associated with the int property when carrying out the moderation.
Sexual, SelfHarm,Violence, Hate

The Azure AI Content Safety Service currently has 4 Harm categories which are; Hate/Fairness, Sexual, Violence and Self-Harm. It is recommended as part of setup, that an int property with the SeverityLevel attribute, is created for each Harm Category. 

**Example**
``` C#
public class StartPage : SitePageData
{
    [Display(GroupName = SystemTabNames.Content,
       Order = 15,
       Description = "Integer value which is used to determine the allowed level of Hate related content within the CMS",
       Name = "Hate Result Severity Level")]
   [SeverityLevel]
   public virtual int HateResultSeverityLevel { get; set; }

   [Display(GroupName = SystemTabNames.Content,
      Order = 20,
      Description = "Integer value which is used to determine the allowed level of Sexual related content within the CMS",
      Name = "Sexual Result Severity Level")]
  [SeverityLevel]
  public virtual int SexualResultSeverityLevel { get; set; }

  [Display(GroupName = SystemTabNames.Content,
      Order = 25,
      Description = "Integer value which is used to determine the allowed level of Violence related content within the CMS",
      Name = "Violence Result Severity Level")]
  [SeverityLevel]
  public virtual int ViolenceResultSeverityLevel { get; set; }

   [Display(GroupName = SystemTabNames.Content,
       Order = 30,
       Description = "Integer value which is used to determine the allowed level of Self Harm related content within the CMS",
       Name = "Self Harm Result Severity Level")]
   [SeverityLevel]
   public virtual int SelfHarmResultSeverityLevel { get; set; }
}
```
**Screenshot of Attributes being used in the CMS**
![SeverityLevelsCMS](/docs/Features/Images/SeverityLevels.JPG)
