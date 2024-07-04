using DapperDataObjectLib;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._07._0706
{
    public partial class WorkDetail : System.Web.UI.Page
    {
        public List<B03PsnHoliday> DataList = new List<B03PsnHoliday>();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            string StartTime = Request["StartTime"] + " 00:00:00";
            string EndTime = Request["EndTime"] + " 23:59:59";
            string CompanyNo = Request["CompanyNo"];
            string DepartmentNo = Request["DepartmentNo"];
            string CompanyName = Request["CompanyName"];
            string DepartmentName = Request["DepartmentName"];

            string Sql = @"
SELECT 
OrgStrucAllData_0.OrgNo As CompanyNo,
OrgStrucAllData_0.OrgClass As CompanyClass,
OrgStrucAllData_0.OrgName As CompanyName,
OrgStrucAllData_1.OrgNo As DepartmentNo,
OrgStrucAllData_1.OrgClass As DepartmentClass,
OrgStrucAllData_1.OrgName As DepartmentName,
A.*,P.PsnName,VName AS HoliNo 
FROM B03_PsnHoliday A 
INNER JOIN B01_Person P ON A.PsnNo=P.PsnNo 
INNER JOIN B00_VacationData V ON A.HoliNo=V.VNo 
Inner Join OrgStrucAllData('Company') AS OrgStrucAllData_0 ON P.OrgStrucID = OrgStrucAllData_0.OrgStrucID
Inner Join OrgStrucAllData('Department') AS OrgStrucAllData_1 ON P.OrgStrucID = OrgStrucAllData_1.OrgStrucID
WHERE ((StartTime BETWEEN @StartTime AND @EndTime) OR (EndTime BETWEEN @StartTime AND @EndTime))
";
            if (!string.IsNullOrEmpty(CompanyNo))
            {
                Sql += " And OrgStrucAllData_0.OrgNo=@CompanyNo";
            }
            if (!string.IsNullOrEmpty(DepartmentNo))
            {
                Sql += " And OrgStrucAllData_1.OrgNo=@DepartmentNo";
            }
            Sql += " Order by A.StartTime";

            this.DataList = this.odo.GetQueryResult<B03PsnHoliday>(Sql, new {
                StartTime = Request["StartTime"].ToString(),
                EndTime = Request["EndTime"].ToString(),
                CompanyNo = CompanyNo,
                DepartmentNo = DepartmentNo
            }).ToList();

            if (DataList.Count != 0)
            {
                this.lblDepartment.Text = "[" + DataList[0].DepartmentNo + "]" + DataList[0].DepartmentName.ToString().Trim() + "  休假清單";
            }
        }
    }
}