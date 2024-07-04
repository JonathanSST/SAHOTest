using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class OsManageEntity
    {
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string CardNo { get; set; }
        public string CardDate { get; set; }
        public DateTime CardTime { get; set; }
        public DateTime CardTimeReal { get; set; }
        public string CompNo { get; set; }
        public string TitleNo { get; set; }
        public string CompName { get; set; }
        public string TitleName { get; set; }
        public string EquNo { get; set; }
        public string EquNo2 { get; set; }
        public string EquName { get; set; }
        public string EquName2 { get; set; }
        public string First { get; set; }
        public string FirstReal { get; set; }
        public string Last { get; set; }
        public string LastReal { get; set; }
        public string LogStatus { get; set; }
        public string NewTitle { get; set; }
        public int ErrCount { get; set; }
        public int OkCount { get; set; }
        public int AllCount { get; set; }
        public int WorkDay { get; set; }
        public int Holiday { get; set; }
    }
}//end namespace