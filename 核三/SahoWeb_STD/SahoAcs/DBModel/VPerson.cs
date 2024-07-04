using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class VPerson
    {
        public int PsnID { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string PsnEName { get; set; }
        public string PsnType { get; set; }
        public string PsnTypeName { get; set; }
        public string IDNum { get; set; }
        public string BirthDay { get; set; }
        public int OrgStrucID { get; set; }
        public string OrgNoList { get; set; }
        public string OrgNameList { get; set; }
        public string OrgID1 { get; set; }
        public string OrgNo1 { get; set; }
        public string OrgName1 { get; set; }
        public string OrgID2 { get; set; }
        public string OrgNo2 { get; set; }
        public string OrgName2 { get; set; }
        public string OrgID3 { get; set; }
        public string OrgNo3 { get; set; }
        public string OrgName3 { get; set; }
        public string OrgID4 { get; set; }
        public string OrgNo4 { get; set; }
        public string OrgName4 { get; set; }
        public string OrgID5 { get; set; }
        public string OrgNo5 { get; set; }
        public string OrgName5 { get; set; }
        public string PsnAccount { get; set; }
        public string PsnPW { get; set; }
        public string PsnAuthAllow { get; set; }
        public DateTime PsnSTime { get; set; }
        public DateTime PsnETime { get; set; }
        public string PsnPicSource { get; set; }
        public string Remark { get; set; }
        public string Rev01 { get; set; }
        public string Rev02 { get; set; }
        public string Text1 { get; set; } = "";
        public string Text2 { get; set; } = "";
        public string Text3 { get; set; } = "";
        public string Text4 { get; set; } = "";
        public string Text5 { get; set; } = "";
        public DateTime TrainingEDate { get; set; }

    }
}