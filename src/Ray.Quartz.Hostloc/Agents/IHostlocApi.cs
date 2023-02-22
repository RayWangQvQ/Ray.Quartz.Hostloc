using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.Quartz.Hostloc.Agents
{
    public interface IHostlocApi
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1")]
        Task<ApiResponse<string>> LoginAsync([Body(BodySerializationMethod.UrlEncoded)] LoginRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        [Get("/home.php?mod=spacecp&ac=credit&op=log&suboperation=creditrulelog")]
        Task<ApiResponse<string>> GetCreditHistoryPageAsync([Header("Cookie")] string cookie);

        /// <summary>
        /// 获取帖子列表
        /// </summary>
        /// <returns></returns>
        [Get("/forum-45-1.html")]
        Task<string> GetPostListPageAsync();

        /// <summary>
        /// 获取个人空间
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Get("/space-uid-{uid}.html")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<string> GetSpacePageAsync([Header("Cookie")] string cookie, string uid);

        /// <summary>
        /// 访问空间
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [Get("/home.php{**param}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<string> VisitSpaceAsync([Header("Cookie")] string cookie, string param);

        /// <summary>
        /// 获取投票贴列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Get("/forum.php{**url}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<ApiResponse<string>> GetVotePostListPageAsync(string url= "?mod=forumdisplay&fid=45&orderby=lastpost&filter=dateline&dateline=86400&specialtype=poll");

        [Get("/forum.php{**url}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<ApiResponse<string>> CommonForumAsync([Header("Cookie")] string cookie, string url);

        [Post("/forum.php?mod=misc&action=votepoll&fid=45&pollsubmit=yes&quickforward=yes&inajax=1")]
        Task<ApiResponse<string>> VoteAsync([Header("Cookie")] string cookie, string tid, [Body(BodySerializationMethod.UrlEncoded)] VoteRequest request);

        /// <summary>
        /// 获取帖子页
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [Post("/thread-{tid}-{pageIndex}-1.html")]
        Task<ApiResponse<string>> GetPostPageAsync(long tid, int pageIndex=1);

        /// <summary>
        /// 回复
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        [Post("/forum.php?mod=post&action=reply&fid={fid}&tid={tid}&extra=page=1&replysubmit=yes&infloat=yes&handlekey=fastpost&inajax=1")]
        Task<ApiResponse<string>> Reply(long tid,long fid, [Body(BodySerializationMethod.UrlEncoded)] ReplyRequest request);
    }

    public class LoginRequest
    {
        public LoginRequest(string userName, string pwd)
        {
            username = userName;
            password = pwd;
        }

        public string fastloginfield { get; set; } = "username";

        public string username { get; set; }

        public string password { get; set; }

        public string formhash { get; set; }

        public string quickforward { get; set; } = "yes";

        public string handlekey { get; set; } = "ls";
    }

    public class VoteRequest
    {
        [AliasAs("pollanswers[]")]
        public string pollanswers { get; set; }
    }

    public class ReplyRequest
    {
        public ReplyRequest(string formhash,string msg)
        {
            this.formhash = formhash;
            this.message=msg;
        }

        public string file { get; set; } = "";

        public string message { get; set; }

        public long posttime { get; set; } = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);

        public string formhash { get; set; }

        public string usesig { get; set; } = "1";

        public string subject { get; set; }
    }
}
