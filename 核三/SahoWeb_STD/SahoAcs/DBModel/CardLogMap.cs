using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardLogMap
    {
        public int RecordID { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public string PsnEName { get; set; }
        public string EquName { get; set; }
        public string CardNo { get; set; }
        public string EquDir { get; set; }
        public DateTime? PsnETime { get; set; }
        public string LogStatus { get; set; }
        public DateTime CardTime { get; set; }
        public string Undertaker { get; set; }
        public string StateDesc { get; set; }

        public int PicID { get; set; }
        public int PointX { get; set; }
        public int PointY { get; set; }
        public string EquNo { get; set; }
        public int EquID { get; set; }
        public string MgaName { get; set;}
        public string LogSrc { get; set; }
        public int DayCount { get; set; }

    }//end class
}//end namespace