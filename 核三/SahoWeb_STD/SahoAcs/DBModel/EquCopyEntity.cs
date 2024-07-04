using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquCopyEntity
    {
        public int CardID { get; set; }
        public int EquID { get; set; }
        public string OpMode { get; set; }
        public string CardRule { get; set; }
        public string CardExtData { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string UserID { get; set; }
        public DateTime Time { get; set; }
        public string TargetCardAuth_CardRule { get; set; }
        public string TargetCardAuth_CardExtData { get; set; }
        public string RuleCompare { get; set; }
        public string ExtDataCompare { get; set; }

    }
}