using EPiServer.Core;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Web;
using AzureAIContentSafety.Helpers;
using Patel.AzureAIContentSafety.Optimizely.Services;
using Microsoft.Extensions.DependencyInjection;
using Azure.AI.ContentSafety;
using EPiServer.Framework.Blobs;
using Patel.AzureAIContentSafety.Optimizely.Models;
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

            if (e.Content is EPiServer.Core.ImageData image)
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
                var processText = OptimizelyCmsHelpers.ProcessTextAnalysis(getStartPage, content);
                if (!string.IsNullOrWhiteSpace(processText))
                {
                    e.CancelReason = processText;
                    e.CancelAction = true;
                    e.Content = content;
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            
        }
    }
}
