using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.Quartz.Hostloc
{
    public class CookieManager
    {
        /// <summary>
        /// abc=123 expires=Sun, 11-Sep-2022 14:55:58 GMT; Max-Age=2592000; path=/; secure; HttpOnly;
        /// </summary>
        public List<string> CookieList { get; set; } = new List<string>();

        public string CookieStr => CookieList?
            .Select(x => x.Substring(0, x.IndexOf(";", StringComparison.Ordinal) + 1))
            .JoinAsString("");
    }
}
