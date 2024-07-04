using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class SysLogEntity
    {
        public int RecordID { get; set; }
        public DateTime LogTime { get; set; }
        public string LogType { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string LogFrom { get; set; }
        public string LogIP { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string LogInfo { get; set; }
        public string LogDesc { get; set; }
    }
}