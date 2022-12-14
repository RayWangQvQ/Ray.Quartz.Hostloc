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
        [Post("/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1")]
        Task<ApiResponse<string>> LoginAsync([Body(BodySerializationMethod.UrlEncoded)] LoginRequest request);

        [Get("/home.php?mod=spacecp&ac=credit&op=log&suboperation=creditrulelog")]
        Task<ApiResponse<string>> GetCreditHistoryPageAsync([Header("Cookie")] string cookie);

        [Get("/forum-45-1.html")]
        Task<string> GetPostListPageAsync();

        [Get("/space-uid-{uid}.html")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<string> GetSpacePageAsync([Header("Cookie")] string cookie, string uid);

        [Get("/home.php{**param}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<string> VisitSpaceAsync([Header("Cookie")] string cookie, string param);

        [Get("/forum.php{**url}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<ApiResponse<string>> GetVotePostListPageAsync(string url= "?mod=forumdisplay&fid=45&orderby=lastpost&filter=dateline&dateline=86400&specialtype=poll");

        [Get("/forum.php{**url}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<ApiResponse<string>> GetForumPageAsync([Header("Cookie")] string cookie, string url);

        [Post("/forum.php?mod=misc&action=votepoll&fid=45&pollsubmit=yes&quickforward=yes&inajax=1")]
        Task<ApiResponse<string>> VoteAsync([Header("Cookie")] string cookie, string tid, [Body(BodySerializationMethod.UrlEncoded)] VoteRequest request);

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
}
