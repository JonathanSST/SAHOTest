using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class StayCardLog
    {
        
        public string CardTime { get; set; }
        public string CardDate { get; set; }        
        public string DepID { get; set; }
        public string DepName { get; set; }
        public string Company { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string CardNo { get; set; }
        public string CardVer { get; set; }
        public string ReaderNo { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string LogStatus { get; set; }
        public int SyncMark3 { get; set; }
        public int SyncMark2 { get; set; }
        public int SyncMark { get; set; }
        public string TempCardNo { get; set; }
        public string CardTimeIn { get; set; }
        public string CardTimeOut { get; set; }
        public string CardTimeS { get; set; }
        public string CardTimeE { get; set; }
        public string RealTimeS { get; set; }
        public string RealTimeE { get; set; }
        public string EquDir { get; set; }
        public int TotalOut { get; set; }
        public int TotalIn { get; set; }
        public string OrgNo { get; set; }
        public string OrgName { get; set; }
    }
}