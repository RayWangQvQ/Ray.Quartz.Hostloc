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
        Task<string> LoginAsync([Body(BodySerializationMethod.UrlEncoded)] LoginRequest request);
    }

    public class LoginRequest
    {
        public LoginRequest(string userName,string pwd)
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
}
