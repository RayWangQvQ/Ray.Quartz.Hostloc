using System;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Ray.Quartz.Hostloc.Agents;
using Ray.Quartz.Hostloc.Configs;
using Volo.Abp.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Ray.Quartz.Hostloc;

public class HelloWorldService : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly IHostlocApi _hostlocApi;
    private readonly AccountOptions _accountConfig;

    public HelloWorldService(
        IConfiguration configuration,
        IHostlocApi hostlocApi,
        IOptions<AccountOptions> accountOptions
        )
    {
        _configuration = configuration;
        _hostlocApi = hostlocApi;
        _accountConfig = accountOptions.Value;
        Logger = NullLogger<HelloWorldService>.Instance;
    }

    public ILogger<HelloWorldService> Logger { get; set; }


    public async Task SayHelloAsync()
    {
        Logger.LogInformation("Hello World!{newLine}", Environment.NewLine);

        var taskName = _configuration["Run"];
        switch (taskName)
        {
            case "login":
                await LoginAsync();
                break;
        }
    }

    private async Task LoginAsync()
    {
        Logger.LogInformation("开始任务：登录");

        var req = new LoginRequest(_accountConfig.UserName, _accountConfig.Pwd);
        var re = await _hostlocApi.LoginAsync(req);
        Logger.LogInformation("结果：{response}", re);

        Logger.LogInformation("Success{newLine}", Environment.NewLine);
    }
}
