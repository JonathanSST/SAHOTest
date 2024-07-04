using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class UserRole
    {
        public int RoleID { get; set; }
        public string RoleNo { get; set; }
        public string RoleName { get; set; }
        public string RoleEName { get; set; }
        public string RoleDesc { get; set; }
        public string RoleState { get; set; }
        public string Remark { get; set; }
        public string UserID { get; set; }
    }//end entity class
}//end namespace