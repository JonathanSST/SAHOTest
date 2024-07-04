using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquGroupLog
    {
        public int EquGrpID { get; set; }
        public string EquGrpName { get; set; }
        public string EquGrpNo { get; set; }
        public string CardNo { get; set; }
        public string Action { get; set; }
    }
}