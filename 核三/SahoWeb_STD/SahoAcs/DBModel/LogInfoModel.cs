using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class LogInfoModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime RefreshTime { get; set; }
    }
}