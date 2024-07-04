using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class B03WorkAbnormal
    {
        public Decimal RecordID { get; set; }
        public String PsnNo { get; set; }
        /// <summary>
        /// 個人姓名
        /// </summary>
        public String PsnName { get; set; }
        public DateTime LogDate { get; set; }
        public String WorkDate { get; set; }
        public String ClassNo { get; set; }
        public String LogTime { get; set; }
        public String WorkTimeS { get; set; }
        public String WorkTimeE { get; set; }
        public String RealTimeS { get; set; }
        public String RealTimeE { get; set; }
        public String WorkTimeO { get; set; }
        public String WorkTimeI { get; set; }
        public String RestTimeO { get; set; }
        public String RestTimeI { get; set; }
        public int Delay { get; set; }
        public int StealTime { get; set; }
        public int OverTime { get; set; }
        public String StatuDesc { get; set; }
        public String AbnormalDesc { get; set; }
        public string AbnormalDesc2 { get; set; }
        public string WorkDesc { get; set; }    
        public string WorkDesc2 { get; set; }
        public int IsSend { get; set; }
        public String CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        public String UpdateUserID { get; set; }
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 個人Email
        /// </summary>
        public string Personal { get; set; }
        /// <summary>
        /// 主管Email
        /// </summary>
        public string Supervisor { get; set; }
        /// <summary>
        /// 主管姓名
        /// </summary>
        public string SName { get; set; }
        /// <summary>
        /// 主管Email
        /// </summary>
        public string SEmail { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Daily { get; set; }
        public string Hours { get; set; }
        public int IsPass { get; set; }
        public string OrgStrucID { get; set; }
        public string OrgStrucName { get; set; }
    }
}