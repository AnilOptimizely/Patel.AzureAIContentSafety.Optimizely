# Blocklist Management

The default Azure AI Content Safety service offers classifiers which are sufficient for most content moderation needs. However, you might need to screen for items that are specific to your use case. This is where the blocklists feature can be used to cater to this use case. Below are various use cases of where Blocklists can be integrated within Optimizely CMS 12.

Blocklists can be also be created by using the Azure Content Safety Studio which can be accessed  https://contentsafety.cognitive.azure.com/text ( A Content Safety resource is required for this). They can also be managed through a custom Add-On I have developed for this which is in this package. A screenshot of this is below

Link to documentation is [here](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/how-to/use-blocklist?tabs=linux%2Crest).

## List All Blocklists

**Screenshot of Add-On where Blocklists are listed**
![Screenshot of Add-On where Blocklists are listed.](/docs/Features/Images/BlocklistAddOn.jpg)

## Create New Blocklist

**Screenshot of Add-On where New Blocklist can be created**
![Screenshot of Add-On where New Blocklist can be created.](/docs/Features/Images/BlocklistAddOnCreateNewBlocklist.jpg)

**Result of Blocklist being created**
![Result of Blocklist being created.](/docs/Features/Images/AddOnBlocklistCreated.jpg)

## Delete Blocklist 

**Screenshot of Add-On where Blocklist can be deleted**
![Screenshot of Add-On where Blocklist can be deleted.](/docs/Features/Images/AddOnBlocklistDeleted.jpg)

**Result of Blocklist being deleted**
![Result of Blocklist being deleted.](/docs/Features/Images/ResultBlocklistDeleted.jpg)

## Get Blocklist Items

**Screenshot of Add-On where Blocklist items can be retrieved**
![Screenshot of Add-On where Blocklist items can be retrieved.](/docs/Features/Images/AddOnGetBlocklistItems.jpg)

**Result of retrieving Blocklist items**
![Result of retrieving Blocklist items.](/docs/Features/Images/ResultGetBlocklistItems.jpg)
