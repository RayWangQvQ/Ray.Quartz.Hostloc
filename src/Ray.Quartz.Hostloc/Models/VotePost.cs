using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ray.Quartz.Hostloc.Helpers;

namespace Ray.Quartz.Hostloc.Models
{
    public class VotePost
    {
        public VotePost(string tBody)
        {
            var th = RegexHelper.QuerySingle(tBody, "<th.+?</th>");

            //var thClass= RegexHelper.SubstringSingle(th, "<th.+class=\"", "\".*>");
            //IsOpen = thClass == "common";

            /*
             *     <a
        href="forum.php?mod=viewthread&thisisparam"
        onclick="atarget(this)" class="s xst">这是标题</a>
             */

            var alinks = RegexHelper.QueryMultiple(th, "<a.+?</a>");
            var targetLink = alinks.FirstOrDefault(x => x.Contains("href=\"forum.php?mod=viewthread")
                                                        && x.Contains("onclick=\"atarget(this)\""));
            Title = RegexHelper.SubstringSingle(targetLink, ">", "</a>");
            Url = RegexHelper.SubstringSingle(targetLink, "<a.+href=\"", "\"");
            Url = HttpUtility.UrlDecode(Url)?.Replace("amp;", "");

            //var alink = RegexHelper.QuerySingle(th, "<a href=\"forum.php?mod=viewthread.+?onclick=\"atarget\\(this\\)\".+?</a>");
            //Title = RegexHelper.SubstringSingle(alink, ">", "</a>");
            //Url = RegexHelper.SubstringSingle(alink, "<a href=\"", "\"");
            //Url = HttpUtility.UrlDecode(Url)?.Replace("amp;", "");


        }
        //public bool IsOpen { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

    }
}
