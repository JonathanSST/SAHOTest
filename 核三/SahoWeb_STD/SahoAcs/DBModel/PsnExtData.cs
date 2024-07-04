using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class PsnExtData
    {
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public DateTime LastTime { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string CtrlAreaName { get; set; }
        public string CtrlAreaNo { get; set; }
        //public string CardTimeVal { get; set; }
    }
}