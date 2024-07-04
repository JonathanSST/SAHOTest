using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class OrgDataEntity
    {
        public int OrgID { get; set; }        
        public string OrgClass { get; set; }
        public string OrgNo { get; set; }
        public string OrgName { get; set; }
        public int OrgStrucID { get; set; }
    }
}