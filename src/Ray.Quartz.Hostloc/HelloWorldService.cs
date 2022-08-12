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
using Ray.Quartz.Hostloc.Helpers;
using System.Collections.Generic;
using System.Linq;

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
            case "visitSpace":
                await VisitSpace();
                break;
            default:
                Logger.LogWarning("任务不存在：{task}", taskName);
                break;
        }
    }

    private async Task LoginAsync()//todo:获取登录后cookie
    {
        Logger.LogInformation("开始任务：登录");
        Logger.LogInformation(_accountConfig.UserName);

        var req = new LoginRequest(_accountConfig.UserName, _accountConfig.Pwd);//todo:密码加密
        var re = await _hostlocApi.LoginAsync(req);
        Logger.LogInformation("结果：{response}", re.IsSuccessStatusCode);

        Logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(re.Headers));

        Logger.LogInformation("Success{newLine}", Environment.NewLine);
    }

    private async Task VisitSpace()
    {
        Logger.LogInformation("开始任务：访问空间");

        var cookie = "";

        //获取目标空间 <a href="space-uid-39205.html" c="1">ikxin</a></cite>
        var postPage = await _hostlocApi.GetPostListPageAsync();

        var halfPostPage = postPage.Substring(postPage.Length / 2);
        var uidList = RegexHelper.SubstringMultiple(halfPostPage, "<a href=\"space-uid-", ".html\" c=\".+\">.+</a></cite>");
        uidList = uidList.Select(x =>
        {
            if (int.TryParse(x, out int code)) return x;
            else return "0";
        })
        .Distinct()
        .ToList();
        if (uidList.Count > 10) uidList = uidList.Take(10).ToList();
        Logger.LogInformation("共获取到{count}个目标空间{newLine}", uidList.Count, Environment.NewLine);

        for (int i = 0; i < uidList.Count; i++)
        {
            var num=i+1;
            var item = uidList[i];
            Logger.LogInformation("访问第{num}个：{code}", num, item);
            try
            {
                var re = await _hostlocApi.GetSpacePageAsync(cookie, item);
                Logger.LogInformation("进入空间成功！");

                //<script src="home.php?mod=spacecp&ac=pm&op=checknewpm&rand=1660281401" type="text/javascript"></script>
                var url = RegexHelper.SubstringSingle(re, "<script src=\"home.php\\?", "\" type=\"text/javascript\"></script>");
                Logger.LogInformation("目标url：{url}", url);

                var result = await _hostlocApi.VisitSpaceAsync(cookie, "?" + url);
                Logger.LogInformation("上报成功{newLine}", Environment.NewLine);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "访问失败");
                throw;
            }
            finally
            {
                await Task.Delay(3 * 1000);
            }
        }

        Logger.LogInformation("Success{newLine}", Environment.NewLine);
    }
}
