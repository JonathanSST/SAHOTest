using System;

namespace SahoAcs.DBModel
{
    public class B03WorkDetail
    {
        public Decimal RecordID { get; set; }
        public String WorkDate { get; set; }
        public String PsnNo { get; set; }
        public string PsnName { get; set; }
        public String ClassNo { get; set; }
        public string WorkDesc { get; set; }
        public string WorkDesc2 { get; set; }
        public string AbnormalDesc { get; set; }
        public String WorkTimeS { get; set; }
        public String WorkTimeE { get; set; }
        public String RealTimeS { get; set; }
        public String RealTimeE { get; set; }
        public String WorkTimeO { get; set; }
        public String WorkTimeI { get; set; }
        public String RestTimeO { get; set; }
        public String RestTimeI { get; set; }
        public Int32 Delay { get; set; }
        public Int32 StealTime { get; set; }
        public Int32 OverTime { get; set; }      
        public String StatuDesc { get; set; }
        public string UpdateUserID { get; set; }
        public string UpdateTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Daily { get; set; }
        public string Hours { get; set; }
        public string LeaveStartTime { get; set; }
        public string LeaveEndTime { get; set; }
        public string AttendanceDesc { get; set; }
        public string Remark { get; set; }
        public string CompanyNo { get; set; }
        public string CompanyClass { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentNo { get; set; }
        public string DepartmentClass { get; set; }
        public string DepartmentName { get; set; }
        public string OrgNoList { get; set; }
    }
}