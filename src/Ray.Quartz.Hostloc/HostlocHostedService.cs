using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace Ray.Quartz.Hostloc;

public class HostlocHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public HostlocHostedService(
        IConfiguration configuration, 
        IHostEnvironment hostEnvironment, 
        IHostApplicationLifetime hostApplicationLifetime
        )
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication =  await AbpApplicationFactory.CreateAsync<HostlocModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

        var helloWorldService = _abpApplication.ServiceProvider.GetRequiredService<HelloWorldService>();

        await Task.Delay(10 * 1000, cancellationToken);
        Console.WriteLine("READY");
        await helloWorldService.SayHelloAsync(cancellationToken);
        Console.WriteLine("DONE");
        await Task.Delay(10 * 1000, cancellationToken);
        //_hostApplicationLifetime.StopApplication();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
