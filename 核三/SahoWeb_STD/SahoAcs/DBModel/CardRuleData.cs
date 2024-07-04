using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardRuleData
    {
        public int RuleID { get; set; }
        public string RuleNo { get; set; }
        public string RuleName { get; set; }
        public string EquModel { get; set; }
        public int EquID { get; set; }
        public int RecordID { get; set; }
        public string ParaValue { get; set; }
    }
}