using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquAdjLog
    {
        public int EquID { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string EquClass { get; set; }
        public string CardExtData { get; set; }
        public string CardRule { get; set; }
        public string CardNo { get; set; }
        public string Action { get; set; }
        public string OpMode { get; set; }
    }
}