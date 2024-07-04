using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquGroupData
    {
        public int EquGrpID { get; set; }
        public int EquID { get; set; }
        public string CardRule { get; set; }
        public string CardExtData { get; set; }
        public string CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUserID { get; set; }
        public DateTime UpdateTime{get; set;}
        public string EquModel { get; set; }
        public string EquGrpName { get; set; }
        public string EquNo { get; set; }
        public string EquGrpNo { get; set; }
        public string OwnerID { get; set; }
        public string EquName { get; set; }
        public string EquClass { get; set; }
        public int TimeZone { get; set; }
    }
}