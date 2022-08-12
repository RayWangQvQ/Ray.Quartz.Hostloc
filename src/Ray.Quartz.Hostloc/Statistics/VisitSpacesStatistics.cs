using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ray.Quartz.Hostloc.Models;

namespace Ray.Quartz.Hostloc.Statistics
{
    public class VisitSpacesStatistics
    {
        public CreditHistory PreCreditHistory { get; set; }

        public CreditHistory AfterCreditHistory { get; set; }

        public List<string> VisitUids { get; set; }=new List<string>();

        public void LogInfo(ILogger logger)
        {
            logger.LogInformation("=====================统计======================");

            logger.LogInformation("任务前状态：");
            PreCreditHistory?.LogInfo(logger);

            logger.LogInformation("任务后状态：");
            AfterCreditHistory?.LogInfo(logger);

            logger.LogInformation("共成功访问了{num}个空间：",VisitUids.Count);
            logger.LogInformation(VisitUids.JoinAsString(","));
        }
    }
}
