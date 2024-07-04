using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class PersonEntity
    {
        public int PsnID { get; set; }
        public int OrgStrucID { get; set; }
        public string PsnNo { get; set; }
        public string CardNo { get; set; }
        public string CardType { get; set; }
        public string CardVer { get; set; } = "";
        public string PsnName { get; set; }
        public int TravelMode { get; set; }

        public string PsnEName { get; set; }
        public string PsnType { get; set; }
        public string IDNum { get; set; }

        public string PsnPicSource { get; set; }
        public string OrgNoList { get; set; }
        public string OrgNameList { get; set; }
        public string Remark { get; set; }
        public string PsnSTime { get; set; }
        public string PsnAuthAllow { get; set; }
        public string PsnETime { get; set; }
        public string BirthDay { get; set; }
        public string OrgName { get; set; }

        public string PsnAccount { get; set; }
        public string PsnPW { get; set; }
        public string VerifiMode { get; set; }
        public string ModifyDate { get; set; }
        public string CreateUserID { get; set; }
        public DateTime? CreateTime { get; set; }
        public string UpdateUserID { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string OrgIDList { get; set; }

        public List<CardEntity> CardList { get; set; }
        public string EmpNo { get; set; }
        public string Text1 { get; set; } = "";
        public string Text2 { get; set; } = "";
        public string Text3 { get; set; } = "";
        public string Text4 { get; set; } = "";
        public string Text5 { get; set; } = "";

        /*update add property */
        public string Personal { get; set; }
        public string Supervisor { get; set; }

        public string PsnClassNo { get; set; }
        public string PsnIsLev { get; set; }
        public string PsnNoneLog { get; set; }
        public string PsnAreaNo { get; set; }

    }
}