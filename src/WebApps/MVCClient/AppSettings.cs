using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient
{
    public class AppSettings
    {
        //public Connectionstrings ConnectionStrings { get; set; }
        public string Api1Url { get; set; }
        public string Api2Url { get; set; }
        public string ApiGWUrl { get; set; }
        public string IdentityUrl { get; set; }
        public string CallBackUrl { get; set; }
        public string AllowedHosts { get; set; } // not sure
        //public Logging Logging { get; set; }
    }
}
