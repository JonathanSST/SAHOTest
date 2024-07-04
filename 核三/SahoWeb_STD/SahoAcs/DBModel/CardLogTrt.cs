using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardLogTrt
    {
        public DateTime CardTime { get; set; }
        public DateTime LogTime { get; set; }
        public string DepID { get; set; }
        public string DepName { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string CardNo { get; set; }
        public string CardVer { get; set; }
        public string ReaderNo { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string LogStatus { get; set; }
        public string TempCardNo { get; set; }
        public string EquDir { get; set; }

        public string First { get; set; }
        public string Last { get; set; }
        public string CardTimeS { get; set; }
        public string CardTimeE { get; set; }
        public string CarNum { get; set; }
        public string PlateNo { get; set; }
        public string ETC { get; set; }
        public string oDepID { get; set; }

        public string oDepName { get; set; }
    }
}