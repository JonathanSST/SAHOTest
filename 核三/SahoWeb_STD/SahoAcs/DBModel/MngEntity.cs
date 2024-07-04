using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class MngEntity
    {
        public int MgaID { get; set; }
        public string MgaNo { get; set; }
        public string MgaName { get; set; }
        public string MgaEName { get; set; }
        public string Email { get; set; }
        public string MgaDesc { get; set; }
        public string Remark { get; set; }
        public string OwnerList { get; set; }
        public string CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
    }
}