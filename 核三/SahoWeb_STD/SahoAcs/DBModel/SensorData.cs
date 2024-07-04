using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class SensorData
    {

        public int DciID { get; set; }
        public int IOMstID { get; set; }
        public int IOMstStatus { get; set; }
        public int IOMID { get; set; }
        public string IOMNo { get; set; }
        public string IOMName { get; set; }
        public int IOMStatus { get; set; }
        public int SenID { get; set; }
        public string SenNo { get; set; }
        public string SenName { get; set; }
        public string LocLongName { get; set; }
        public int SenStatus { get; set; }








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