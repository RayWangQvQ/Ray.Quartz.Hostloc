using Ray.Quartz.Hostloc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ray.Quartz.Hostloc.Domain
{
    public class Post
    {
        public Post(long tid, string fullPage)
        {
            Tid=tid;
            FullPage=fullPage;

            SetFid();
            SetFormHash();
            SetTotalPage();
            SetLastFloorIndex();
        }

        public long Tid { get; private set; }

        public string FullPage { get; set; }

        public long Fid { get; private set; }

        public string FormHash { get; private set; }

        public int TotalPage { get; set; }

        public int LastFloorIndex { get; set; }

        private void SetFid()
        {
            /*
             *      <input type="hidden" name="mod" id="scbar_mod" value="search" />
					<input type="hidden" name="formhash" value="f24773e5" />
					<input type="hidden" name="srchtype" value="title" />
					<input type="hidden" name="srhfid" value="45" />
					<input type="hidden" name="srhlocality" value="forum::viewthread" />
             */

            if(FullPage.IsNullOrWhiteSpace())return;

            var fid = RegexHelper.SubstringSingle(FullPage, "<input type=\"hidden\" name=\"srhfid\" value=\"", "\" />");

            if(fid.IsNullOrWhiteSpace()) return;

            _ = int.TryParse(fid, out int re);

            Fid = re;
        }

        private void SetFormHash()
        {
            if (FullPage.IsNullOrWhiteSpace()) return;

            var formHash = RegexHelper.SubstringSingle(FullPage, "<input type=\"hidden\" name=\"formhash\" value=\"", "\" />");

            FormHash = formHash;
        }

        private void SetTotalPage()
        {
            /*
             * 					<div class="pg"><strong>1</strong><a href="thread-1140231-2-1.html">2</a><a
							href="thread-1140231-3-1.html">3</a><a href="thread-1140231-4-1.html">4</a><a
							href="thread-1140231-5-1.html">5</a><a href="thread-1140231-6-1.html">6</a><a
							href="thread-1140231-7-1.html">7</a><a href="thread-1140231-8-1.html">8</a><a
							href="thread-1140231-9-1.html">9</a><label><input type="text" name="custompage" class="px"
								size="2" title="输入页码，按回车快速跳转" value="1"
								onkeydown="if(event.keyCode==13) {window.location='forum.php?mod=viewthread&tid=1140231&amp;extra=page%3D1&amp;page='+this.value;; doane(event);}" /><span
								title="共 9 页"> / 9 页</span></label><a href="thread-1140231-2-1.html" class="nxt">下一页</a>
					</div>
             */

            if (FullPage.IsNullOrWhiteSpace()) return;

            var totalPage = RegexHelper.SubstringSingle(FullPage, "title=\"共 ", " 页\"> /");

            int.TryParse(totalPage, out int re);
            TotalPage = re;
        }

        private void SetLastFloorIndex()
        {
            /*
						<td class="pls ptn pbn">
							<div class="hm ptn">
								<span class="xg1">查看:</span> <span class="xi1">923</span><span
									class="pipe">|</span><span class="xg1">回复:</span> <span class="xi1">85</span>
							</div>
						</td>
             */

            if (FullPage.IsNullOrWhiteSpace()) return;

            var replyCount = RegexHelper.SubstringSingle(FullPage, "<span class=\"xg1\">回复:</span> <span class=\"xi1\">", "</span>");

            int.TryParse(replyCount, out int count);
            LastFloorIndex = ++count;
        }
    }
}
