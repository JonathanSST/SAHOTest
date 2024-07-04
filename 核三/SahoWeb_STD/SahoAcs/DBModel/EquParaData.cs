using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquParaData
    {
        public int EquID { get; set; }
        public string EquNo { get; set; }
        public int CtrlID { get; set; }
        public string ParaType { get; set; }
        public string EquName { get; set;}
        public string EquParaID { get; set; }
        public string ParaName { get; set; }
        public string ParaDesc { get; set; }
        public string DefaultValue { get; set; }
        public string InputType { get; set; }
        public string ParaValue { get; set; }
        public string ValueOptions { get; set; }
        public string EditFormURL { get; set; }
        public string M_ParaValue { get; set; }
        public string FloorName { get; set; }
        public string UserID { get; set; }

    }
}