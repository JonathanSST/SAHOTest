using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardEntity
    {
        public int PsnID { get; set; }
        public string PsnNo { get; set; }
        public int CardID { get; set; }
        public string CardNo { get; set; }
        public string PsnName { get; set; }

        public string CardVer { get; set; }
        public string CardPW { get; set; }
        public string CardType { get; set; }
        public string EquGrpList { get; set; }
        public string CardTypeName { get; set; }
        public int CardAuthAllow { get; set; }
        public string OrgStruncID { get; set; }
        public string OrgNameList { get; set; }
        public string OrgNoList { get; set; }
        public string OrgName { get; set; }
        public string CardSerialNo { get; set; }
        public string CardNum { get; set; }
        public string CardDesc { get; set; }
        public DateTime CardSTime { get; set; }
        public DateTime? CardETime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public string VerifiMode { get; set; }
        public DateTime? UpdateTime { get; set; }
    }//end entity class
}//end namespace