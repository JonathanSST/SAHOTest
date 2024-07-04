using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardLogModel
    {
        public int NewIDNum { get; set; }
        public int RecordID { get; set; }
        public DateTime CardTime { get; set; }

        public DateTime PreCardTime { get; set; }

        public string CardDate { get; set; }
        public int CardQty { get; set; }

        public DateTime LogTime { get; set; }
        public string DepID { get; set; }
        public string DepName { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string PsnEName { get; set; }
        public string IDNum { get; set; }
        public string CardNo { get; set; }
        public string CardVer { get; set; }
        public string ReaderNo { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string LogStatus { get; set; }
        public string TempCardNo { get; set; }
        public string EquDir { get; set; }
        public string PreEquDir { get; set; }
        public string CtrlNo { get; set; }
        public string CamNo { get; set; }
        public int TimeMin { get; set; }
        public string CardPicSource { get; set; }
        public string CardPicPath { get; set; }
        public int OrgStrucID { get; set; }
        public string MgaName { get; set; }
        public string HeatResult { get; set; }
        public DateTime UtcTime { get; set; }
        public string oComID { get; set; }
        public string oComNo { get; set; }
        public string oComName { get; set; }
        public string oUnitID { get; set; }
        public string oUnitNo { get;set; }
        public string oUnitName { get; set; }
        public string oDepID { get; set; }
        public string oDepNo { get; set;}
        public string oDepName { get; set;}
        public string oTitleID { get; set; }
        public string oTitleNo { get; set;}
        public string oTitleName { get; set;}
    }//end entity class
}//end namespace