using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardVerModel
    {
        public int NewIDNum { get; set; }
        public int RecordID { get; set; }
        public DateTime CardSTime { get; set; }
        public int CardID { get; set; }
        public DateTime CardETime { get; set; }
                
        public string DepID { get; set; }
        public string DepName { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string CardNo { get; set; }
        public string ReaderNo { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string LogStatus { get; set; }
        public string TempCardNo { get; set; }
        public string CardVer { get; set; }
        public string PreEquDir { get; set; }
        public string CtrlNo { get; set; }

        public int TimeMin { get; set; }
        public string CardPicSource { get; set; }
        public int OrgStrucID { get; set; }

    }
}