using System;

namespace SahoAcs.DBModel
{
    public class B03PsnHoliday:BaseTableEntity
    {
        public Int32 RecordID { get; set; }
        public String PsnNo { get; set; }
        public String LicNo { get; set; }
        public String HoliNo { get; set; }
        public string LicType { get; set; }
        public String OrgNo { get; set; }
        public String Daily { get; set; }
        public String Hours { get; set; }
        public String Minutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PsnName { get; set; }
        public string CompanyNo { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentNo { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPersonCount { get; set; }
        public string BreakCount { get; set; }
        public string OrgName { get; set; }
        public string Desc { get; set; }
    }
}