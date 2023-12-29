# Blocklist Management

The default Azure AI Content Safety service offers classifiers which are sufficient for most content moderation needs. However, you might need to screen for items that are specific to your use case. This is where the blocklists feature can be used to cater to this use case. Below are various use cases of where Blocklists can be integrated within Optimizely CMS 12.

Blocklists can be also be created by using the Azure Content Safety Studio which can be accessed  https://contentsafety.cognitive.azure.com/text ( A Content Safety resource is required for this). They can also be managed through a custom Add-On I have developed for this which is in this package. A screenshot of this is below

Link to documentation is [here](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/how-to/use-blocklist?tabs=linux%2Crest).

## List All Blocklists

Screenshot of Add-On where Blocklists are listed.
![Screenshot of Add-On where Blocklists are listed.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/BlocklistAddOn.webp)

Code snippet used to Get All Blocklists
![Code snippet used to Get All Blocklists.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/GetAllBlocklists.webp)

## Create New Blocklist

Screenshot of Add-On where New Blocklist can be created

![Screenshot of Add-On where New Blocklist can be created.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/BlocklistAddOnCreateNewBlocklist.webp)

Code snippet used to create new Blocklist

![Code snippet used to create New Blocklist.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/CodeSnippetCreateNewBlocklist.webp)

Result of Blocklist being created

![Result of Blocklist being created.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/AddOnBlocklistCreated.png)

## Delete Blocklist 

Screenshot of Add-On where Blocklist can be deleted

![Screenshot of Add-On where Blocklist can be deleted.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/AddOnBlocklistDeleted.webp)

Code snippet used to delete Blocklist

![Code snippet used to delete Blocklist.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/CodeSnippetDeleteNewBlocklist.webp)

Result of Blocklist being deleted

![Result of Blocklist being deleted.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/ResultBlocklistDeleted.webp)

## Get Blocklist Items

Screenshot of Add-On where Blocklist items can be retrieved

![Screenshot of Add-On where Blocklist items can be retrieved.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/AddOnGetBlocklistItems.webp)

Code snippet used to get Blocklist Items

![Code snippet used to get Blocklist Items.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/CodeSnippetGetBlocklistItems.webp)

Result of retrieving Blocklist items

![Result of retrieving Blocklist items.](https://github.com/AnilOptimizely/Patel-AzureAIContentSafety/blob/main/docs/Features/Blocklist%20Management/ResultGetBlocklistItems.webp)
