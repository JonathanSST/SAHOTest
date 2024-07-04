using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class ScheduleTable
    {
        public string ScheduleID { get; set; }
        public string employeeID { get; set; }
        public DateTime? CreateTime { get; set; }
        public string employeeName { get; set; }
        public DateTime VacationDate { get; set; }
        public string VacationInfo { get; set; }
        public string OrgName { get; set; }
        public string OrgStrucID { get; set; }
        public string SyncMark { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string ChangeLog { get; set; }


    }
}