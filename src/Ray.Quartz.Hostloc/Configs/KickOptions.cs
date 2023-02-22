using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.Quartz.Hostloc.Configs
{
    public class KickOptions
    {
        public bool Enable { get; set; }

        public long Tid { get; set; }

        public List<int> Floors { get; set; }

        public int IntervalSec { get; set; }

        public string Reply { get; set; }
    }
}
