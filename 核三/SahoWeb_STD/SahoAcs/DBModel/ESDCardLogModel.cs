using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class ESDCardLogModel :CardLogModel
    {
        public string ESD1 { get; set; }
        public string ESD2 { get; set; }
        public string ESD3 { get; set; }
        public string ESD4 { get; set; }
        public string OrgName { get; set; }
        //public string OrgStrucID { get; set; }
        public string ESDResult { get; set; }
        public string CTDate { get; set; }
        public string CTTime { get; set; }
    }
}