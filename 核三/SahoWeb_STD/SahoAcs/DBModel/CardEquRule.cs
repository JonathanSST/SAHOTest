using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardEquRule
    {
        public int EquID { get; set; }
        public int RuleID { get; set; }
        public string CardNo { get; set; }
        public string UserID { get; set; }
        public int CardID { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }
    }
}