using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class OrgEquGroup
    {
        public int? OrgStrucID { get; set; }
        public int EquGrpID { get; set; }
        public string EquGrpName { get; set;}
        public string EquGrpNo { get; set; }
        public string CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
    }
}