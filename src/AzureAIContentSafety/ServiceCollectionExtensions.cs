using AzureAIContentSafety.Helpers;
using AzureAIContentSafety.Interfaces;
using AzureAIContentSafety.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureAIContentSafety
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureAIContentSafety(this IServiceCollection services)
        {
            return AddAzureAIContentSafety(services, _ => { });
        }

        public static IServiceCollection AddAzureAIContentSafety(this IServiceCollection services, Action<ContentSafetyOptions> setupAction)
        {
            services.AddOptions<ContentSafetyOptions>().Configure<IConfiguration>((options, configuration) =>
            {
                setupAction(options);
                configuration.GetSection("Patel:AzureAIContentSafety").Bind(options);
            });
            services.SetUpContentSafetyDependencies();
            return services;
        }

        internal static void SetUpContentSafetyDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAzureAIContentSafetyService, AzureAIContentSafetyService>();
            services.AddScoped<OptimizelyCmsHelpers>();
        }
    }
}