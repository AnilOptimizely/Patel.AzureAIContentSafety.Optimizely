using AlloyMVC.Extensions;
using AzureAIContentSafety;
using AzureAIContentSafety.Controllers;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Gulla.Episerver.SqlStudio;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;

namespace AlloyMVC;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;

    public Startup(IWebHostEnvironment webHostingEnvironment)
    {
        _webHostingEnvironment = webHostingEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAzureAIContentSafety(options =>
        {
            options.ContentSafetySubscriptionKey = "45efe358f4e647758e6d41451a84dba8";
            options.ContentSafetyEndpoint = "https://aniloptimizely.cognitiveservices.azure.com/";
        });

        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddAlloy()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();

        // Required by Wangkanai.Detection
        services.AddDetection();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddSqlStudio(x => {
            x.AllowMessage = "Your options are very limited!";
            x.AllowPattern = "SELECT TOP \\d{1,3} \\* FROM tblContent";
            x.AutoIntellisenseEnabled = true;
            x.DarkModeEnabled = true;
            x.CustomColumnsEnabled = true;
            x.ShowSavedQueries = true;
            x.DenyMessage = "Careful, please!";
            x.DenyPattern = "\\b(DROP|DELETE|UPDATE|ALTER|ADD|EXEC|TRUNCATE)\\b";
            x.Enabled = true;
            x.GroupNames = "Administrators";
            x.Users = "Anil";
            x.ConnectionString = "Server=UK-CND2020H10;Database=Patel.AzureAIContentSafety;User Id=dxp_InstanceThreeLocalUser;Password=L*CeVciX1am8*kp7ePg@!$pYO";
            x.DisableAuditLog = true;
            x.AuditLogViewAllUsers = "Anil";
            x.AuditLogViewAllGroupNames = "SuperAdmins,LogViewerAdmin";
            x.AuditLogDeleteUsers = "Anil";
            x.AuditLogDeleteGroupNames = "SuperAdmins,LogDeleterAdmin";
            x.AuditLogDaysToKeep = 10;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Required by Wangkanai.Detection
        app.UseDetection();
        app.UseSession();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
            endpoints.MapControllers();
        });
    }
}
