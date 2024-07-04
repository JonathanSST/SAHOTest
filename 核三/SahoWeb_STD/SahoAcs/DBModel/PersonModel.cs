using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class PersonModel
    {
        public int psn_id { get; set; }
        public string psn_no { get; set; }
        public string psn_name{get;set;}
        public string id_num{get;set;}
        public string org_struct_id{get;set;}

        public string psn_e_name { get; set; }

        public string psn_s_time { get; set; }
        public string psn_e_time { get; set; }

        public string birthday { get; set; }

        public string org_c { get; set; }
        public string org_d { get; set; }
        public string org_u { get; set; }
        public string org_t { get; set; }
        public string psn_account { get; set; }
        public string psn_pw { get; set; }
        public string remark { get; set; }
    }

    public class EquData
    {
        public int EquID { get; set; }
        public string EquModel { get; set; }
        public string EquName { get; set; }
        public string EquNo { get; set; }
        public string ItemInfo3 { get; set; }
        public string VerifiMode { get; set; }
    }
}