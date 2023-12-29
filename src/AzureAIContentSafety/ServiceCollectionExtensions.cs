using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace Patel.AzureAIContentSafety.Optimizely
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureAIContentSafety(this IServiceCollection services)
        {
            return services.AddAzureAIContentSafety(_ => { });
        }

        public static IServiceCollection AddAzureAIContentSafety(this IServiceCollection services, Action<ContentSafetyOptions> setupAction)
        {
            services.AddOptions<ContentSafetyOptions>().Configure<IConfiguration>((options, configuration) =>
            {
                setupAction(options);
                configuration.GetSection("Patel:AzureAIContentSafety").Bind(options);
            });
            return services;
        }
    }
}