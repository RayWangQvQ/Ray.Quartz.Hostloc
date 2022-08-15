using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ray.Quartz.Hostloc.Helpers;

namespace Ray.Quartz.Hostloc.Models
{
    public class CreditHistory
    {
        public CreditHistory(string tr)
        {
            var tds = RegexHelper.SubstringMultiple(tr, "<td>", "</td>");

            ActionName = RegexHelper.SubstringSingle(tds[0], "<a href=.+>", "</a>");
            if (int.TryParse(tds[1], out int totalCount)) TotalCount = totalCount;
            if (int.TryParse(tds[2], out int periodCount)) PeriodCount = periodCount;
            if (int.TryParse(tds[3], out int prestige)) Prestige = prestige;
            if (int.TryParse(tds[4], out int money)) Money = money;
            if (DateTime.TryParse(tds[5], out DateTime updateTime)) UpdateTime = updateTime;
        }

        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 总次数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 周期次数
        /// </summary>
        public int PeriodCount { get; set; }

        /// <summary>
        /// 威望
        /// </summary>
        public int Prestige { get; set; }

        /// <summary>
        /// 金钱
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// 最后奖励时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public void LogInfo(ILogger logger)
        {
            logger.LogInformation("-------积分明细-------");
            logger.LogInformation("动作名称：{ActionName}", ActionName);
            logger.LogInformation("总次数：{TotalCount}", TotalCount);
            logger.LogInformation("周期次数：{PeriodCount}", PeriodCount);
            logger.LogInformation("威望：{Prestige}", Prestige);
            logger.LogInformation("金钱：{Money}", Money);
            logger.LogInformation("最后奖励时间：{UpdateTime}{newLine}", UpdateTime, Environment.NewLine);
            logger.LogInformation("-------积分明细-------");
        }
    }
}
