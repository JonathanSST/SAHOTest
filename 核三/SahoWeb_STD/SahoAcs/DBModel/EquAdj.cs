using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquAdj
    {
        public int? OrgStrucID { get; set; }
        public string OrgStrucName { get; set; }
        public int EquID { get; set; }
        public string EquName { get; set; }
        public string EquNo { get; set; }
        public string OpMode { get; set; }
        public int? CardID { get; set; }
        public string CardNo { get; set; }
        public string EquClass { get; set; }
        public string EquModel { get; set; }
        public string VerifiMode { get; set; }
        public string CardExtData { get; set; }
        public string CardRule { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string ItemInfo3 { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
    }
}