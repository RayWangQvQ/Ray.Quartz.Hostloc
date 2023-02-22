using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ray.Quartz.Hostloc.Agents;
using Ray.Quartz.Hostloc.Configs;
using Refit;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Ray.Quartz.Hostloc.DomainService;

namespace Ray.Quartz.Hostloc;

[DependsOn(
    typeof(AbpAutofacModule)
)]
public class HostlocModule : AbpModule
{
    public override async Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        await base.ConfigureServicesAsync(context);

        var config = context.Services.GetConfiguration();

        context.Services.AddSingleton<CookieManager>();

        #region config

        context.Services.Configure<AccountOptions>(config.GetSection("Account"));
        context.Services.Configure<HttpClientCustomOptions>(config.GetSection("HttpCustomConfig"));
        context.Services.Configure<KickOptions>(config.GetSection("Kick"));

        #endregion

        #region Api

        context.Services.AddScoped<DelayHttpMessageHandler>();
        context.Services.AddScoped<LogHttpMessageHandler>();
        context.Services.AddScoped<ProxyHttpClientHandler>();
        context.Services
            .AddRefitClient<IHostlocApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://hostloc.com");

                var ua = config["UserAgent"];
                if (!string.IsNullOrWhiteSpace(ua))
                    c.DefaultRequestHeaders.UserAgent.ParseAdd(ua);
            })
            .AddHttpMessageHandler<DelayHttpMessageHandler>()
            .AddHttpMessageHandler<LogHttpMessageHandler>()
            .ConfigurePrimaryHttpMessageHandler<ProxyHttpClientHandler>()
            ;

        #endregion

        #region domainservice

        context.Services.AddScoped<PostDomainService, PostDomainService>();

        #endregion
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<HostlocModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }
}
