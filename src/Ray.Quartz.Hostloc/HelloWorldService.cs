using System;
using System.Threading.Tasks;
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
using System.Text.Encodings.Web;
using System.Text.Json;
using Ray.Quartz.Hostloc.Models;
using Ray.Quartz.Hostloc.Statistics;

namespace Ray.Quartz.Hostloc;

public class HelloWorldService : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly IHostlocApi _hostlocApi;
    private readonly CookieManager _cookieManager;
    private readonly AccountOptions _accountConfig;

    public HelloWorldService(
        IConfiguration configuration,
        IHostlocApi hostlocApi,
        IOptions<AccountOptions> accountOptions,
        CookieManager cookieManager
        )
    {
        _configuration = configuration;
        _hostlocApi = hostlocApi;
        _cookieManager = cookieManager;
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
            case "encryptPwd":
                await EncryptPwdAsync();
                break;
            case "login":
                await LoginAsync();
                break;
            case "getCreditHistory":
                await GetCreditHistoryAsync();
                break;
            case "visitSpace":
                await VisitSpaceAsync();
                break;
            case "vote":
                await VoteAsync();
                break;
            default:
                Logger.LogWarning("任务不存在：{task}", taskName);
                break;
        }
    }

    /// <summary>
    /// 加密密码
    /// </summary>
    /// <returns></returns>
    public Task EncryptPwdAsync()
    {
        //Logger.LogInformation("原密码：{pwd}",_accountConfig.Pwd);
        var pwdEncrypted = _accountConfig.Pwd.DESToEncrypt(_configuration["EncryptionKey"]);
        Logger.LogInformation("加密后的密码：{pwd}", pwdEncrypted);
        Logger.LogInformation("生成后，请删除配置中的原始，使用加密后密码作为配置");

        return Task.CompletedTask;
    }

    /// <summary>
    /// 登录获取Cookie
    /// </summary>
    /// <returns></returns>
    public async Task LoginAsync()
    {
        Logger.LogInformation("开始任务：登录");
        Logger.LogInformation(_accountConfig.UserName);

        _accountConfig.Pwd = _accountConfig.PwdEncrypted.DESToDecrypt(_configuration["EncryptionKey"]);

        var req = new LoginRequest(_accountConfig.UserName, _accountConfig.Pwd);
        var re = await _hostlocApi.LoginAsync(req);
        Logger.LogInformation("结果：{response}", re.IsSuccessStatusCode);

        _cookieManager.CookieList = re.Headers.GetValues("Set-Cookie").ToList();
        Logger.LogInformation("CookieStr已获取");

        Logger.LogInformation("Success{newLine}", Environment.NewLine);
    }

    /// <summary>
    /// 访问他人主页
    /// </summary>
    /// <returns></returns>
    public async Task VisitSpaceAsync()
    {
        var taskName = "访问别人空间";
        Logger.LogInformation("开始任务：{taskName}", taskName);

        var statistics = new VisitSpacesStatistics();

        //登录取cookie
        await InitAsync();

        //获取当前任务状态
        var historyList = await GetCreditHistoryAsync();
        statistics.PreCreditHistory = historyList.FirstOrDefault(x => x.ActionName == taskName);

        //获取帖子列表 <a href="space-uid-39205.html" c="1">ikxin</a></cite>
        var postPage = await _hostlocApi.GetPostListPageAsync();

        //获取目标空间uid
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

        //访问空间
        for (int i = 0; i < uidList.Count; i++)
        {
            var num = i + 1;
            var item = uidList[i];
            Logger.LogInformation("访问第{num}个：{code}", num, item);
            try
            {
                var re = await _hostlocApi.GetSpacePageAsync(_cookieManager.CookieStr, item);
                Logger.LogInformation("进入空间成功！");

                //<script src="home.php?mod=spacecp&ac=pm&op=checknewpm&rand=1660281401" type="text/javascript"></script>
                var url = RegexHelper.SubstringSingle(re, "<script src=\"home.php\\?", "\" type=\"text/javascript\"></script>");
                Logger.LogInformation("目标url：{url}", url);

                var result = await _hostlocApi.VisitSpaceAsync(_cookieManager.CookieStr, "?" + url);
                statistics.VisitUids.Add(item);
                Logger.LogInformation("上报成功{newLine}", Environment.NewLine);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "访问失败，跳过");
            }
            finally
            {
                await Task.Delay(3 * 1000);
            }
        }

        Logger.LogInformation("Success{newLine}", Environment.NewLine);

        var newHistoryList = await GetCreditHistoryAsync();
        statistics.AfterCreditHistory = newHistoryList.FirstOrDefault(x => x.ActionName == taskName);

        statistics.LogInfo(Logger);
    }

    /// <summary>
    /// 参与投票
    /// </summary>
    /// <returns></returns>
    public async Task VoteAsync()
    {
        var taskName = "参与投票";
        Logger.LogInformation("=========开始任务：{taskName}=========", taskName);

        await InitAsync();

        //获取投票帖子
        Logger.LogInformation("搜索今日投票帖");
        var pageResponse = await _hostlocApi.GetVotePostListPageAsync();
        var page = pageResponse.Content;
        var table = RegexHelper.QuerySingle(page, "<table summary=\"forum_45\".+?id=\"threadlisttableid\".*?>.+?</table>");

        var tbodyList = RegexHelper.QueryMultiple(table, "<tbody id=\"normalthread_[0-9]+?\">.+?</tbody>");
        if (tbodyList.Count == 0)
        {
            Logger.LogInformation("一天内没有人发起投票，结束");
            return;
        }

        Logger.LogInformation("共找到{count}个投票贴", tbodyList.Count);
        Logger.LogInformation(Environment.NewLine);

        var votePostList = new List<VotePost>();
        for (int i = 0; i < tbodyList.Count; i++)
        {
            try
            {
                var num = i;
                Logger.LogInformation("------查看第{num}个投票贴----", num + 1);

                var item = tbodyList[i];
                var votePost = new VotePost(item);
                votePostList.Add(votePost);

                //获取投票页面
                Logger.LogInformation("帖子：{vatePage}", votePost.Title);
                var votePageUrl = votePost.Url.Substring("forum.php".Length);
                var tid = RegexHelper.QuerySingle(votePageUrl, "tid=[0-9]+");
                tid = tid.Replace("tid=", "");
                var votePage = await _hostlocApi.GetForumPageAsync(_cookieManager.CookieStr, votePageUrl);
                var votePageContent = votePage.Content;
                if (votePageContent.IsNullOrWhiteSpace())
                {
                    Logger.LogInformation("未获取到页面");
                }
                else if (votePageContent.Contains("投票已经结束"))
                {
                    Logger.LogInformation("投票已结束，跳过");
                }
                else if (votePageContent.Contains("您已经投过票"))
                {
                    Logger.LogInformation("曾经已投过票，跳过");
                }
                else
                {
                    Logger.LogInformation("开始投票");

                    //获取选项
                    var voteTable =
                        RegexHelper.QuerySingle(votePage.Content, "<table summary=\"poll panel\".+?</table>");
                    /*
                    var trList = RegexHelper.QueryMultiple(voteTable, "<tr.+?</tr>");
                    var selectTr = trList[^2];
                    var selectValue = RegexHelper.QuerySingle(selectTr, "value=\"[0-9]+\"")
                        .Replace("value", "")
                        .Replace("\"", "");
                    */
                    var selectValue = RegexHelper.QueryMultiple(voteTable, "value=\"[0-9]+\"")
                        .Select(x => x.Replace("value=", "")
                            .Replace("\"", ""))
                        .Last();
                    Logger.LogInformation("选项id：{id}", selectValue);

                    //上报
                    var voteResult = await _hostlocApi.VoteAsync(_cookieManager.CookieStr, tid,
                        new VoteRequest() { pollanswers = selectValue });

                    Logger.LogInformation("投票结果：{result}", voteResult.IsSuccessStatusCode);

                    //检测投票结果
                    Logger.LogInformation("刷新页面确认结果：");
                    votePage = await _hostlocApi.GetForumPageAsync(_cookieManager.CookieStr, votePageUrl);
                    if (votePage.Content?.Contains("您已经投过票") ?? false)
                    {
                        Logger.LogInformation("确认通过");
                    }
                    else
                    {
                        Logger.LogInformation("投票失败");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "投票异常");
            }
            finally
            {
                Logger.LogInformation(Environment.NewLine);
                await Task.Delay(3 * 1000);
            }
        }

        //Logger.LogInformation(JsonSerializer.Serialize(votePostList, options: new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));
    }

    #region private

    private async Task<List<CreditHistory>> GetCreditHistoryAsync()
    {
        await InitAsync();

        var pageResponse = await _hostlocApi.GetCreditHistoryPageAsync(_cookieManager.CookieStr);

        var table = RegexHelper.QuerySingle(pageResponse.Content, "<table summary=\"积分获得历史\".+</table>");

        var trs = RegexHelper.QueryMultiple(table, "<tr.+?</tr>");//?指定非贪婪模式

        var list = new List<CreditHistory>();
        for (var index = 1; index < trs.Count; index++)
        {
            var tr = trs[index];
            var history = new CreditHistory(tr);
            //history.LogInfo(Logger);
            list.Add(history);
        }

        Logger.LogInformation(JsonSerializer.Serialize(list));
        return list;
    }

    private async Task InitAsync()
    {
        if (string.IsNullOrWhiteSpace(_cookieManager.CookieStr))
        {
            await LoginAsync();
        }
    }

    #endregion

}
