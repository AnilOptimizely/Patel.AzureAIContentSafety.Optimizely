using EPiServer.Core;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Web;
using AzureAIContentSafety.Helpers;
using Patel.AzureAIContentSafety.Optimizely.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;
using Patel.AzureAIContentSafety.Optimizely.Interface;

namespace Patel.AzureAIContentSafety.Optimizely.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public class Initialization : IConfigurableModule
    {
        private IContentEvents _contentEvents = null;
        private IContentLoader _contentLoader = null;

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IAzureAIContentSafetyService, AzureAIContentSafetyService>();
            context.Services.AddScoped<OptimizelyCmsHelpers>();
            context.Services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        public void Initialize(InitializationEngine context)
        {
            ServiceProviderHelper serviceLocationHelper = context.Locate;
            _contentEvents = context.Locate.Advanced.GetInstance<IContentEvents>();
            _contentLoader = serviceLocationHelper.ContentLoader();
            _contentEvents.PublishingContent += Events_PublishingContent;
        }

        void Events_PublishingContent(object sender, ContentEventArgs e)
        {
            var getStartPage = _contentLoader.Get<IContent>(ContentReference.StartPage);

            if (e.Content is ImageData image)
            {
                var processImage = OptimizelyCmsHelpers.ProcessImageAnalysis(getStartPage, image);
                if (!string.IsNullOrWhiteSpace(processImage))
                {
                    e.CancelReason = processImage;
                    e.CancelAction = true;
                    e.Content = image;
                }
            }

            if (e.Content is IContent content)
            {
                var listAzureContentSafetyErrorMessages = new List<string>();
                var processText = OptimizelyCmsHelpers.ProcessTextAnalysis(getStartPage, content);
                if (processText.Any())
                {
                    var checkErrorMessage = processText.Where(a => a.StartsWith("Please only have 1 CMS") && processText.Count == 1).ToString();
                    if (!string.IsNullOrWhiteSpace(checkErrorMessage))
                    {
                        e.CancelReason = checkErrorMessage;
                        e.CancelAction = true;
                        e.Content = content;
                    }
                    listAzureContentSafetyErrorMessages.AddRange(processText);
                }
                var processTextBlocklist = OptimizelyCmsHelpers.ProcessTextAnalysisBlocklist(getStartPage, content);
                if (processTextBlocklist.Any())
                {
                    listAzureContentSafetyErrorMessages.AddRange(processTextBlocklist);
                }

                if (listAzureContentSafetyErrorMessages.Any())
                {
                    listAzureContentSafetyErrorMessages.Insert(0, "Unable to publish - Azure AI Content Safety - Text Analysis has detected");
                    listAzureContentSafetyErrorMessages.Add(" Please review content and publish again.");
                    var result = string.Join(". ", listAzureContentSafetyErrorMessages.ToArray());
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        e.CancelReason = result;
                        e.CancelAction = true;
                        e.Content = content;
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
