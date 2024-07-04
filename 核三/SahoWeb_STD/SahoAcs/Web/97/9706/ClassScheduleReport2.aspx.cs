using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;
using SahoAcs.DBClass;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using OfficeOpenXml;


namespace SahoAcs.Web._97._9706
{
    public partial class ClassScheduleReport2 : SahoAcs.DBClass.BasePage
    {
        public List<B03Empclassschedule> ClassList = new List<B03Empclassschedule>();
        public List<B03Empclassschedule> ClassLast2 = new List<B03Empclassschedule>();
        public List<B00Holidayex> HolidayList = new List<B00Holidayex>();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<string> ClassNoList = new List<string>();
        public List<string> ClassAllList = new List<string>();            
        public string NowMonth = DateTime.Now.ToString("yyyy/MM");
        public DateTime Date1, Date2;
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.SetInit();
            if (Request["PageEvent"] == null)
            {
                
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryData")
            {
             
                this.SetClassList();
            }
            if(Request["PageEvent"]!=null && Request["PageEvent"] == "Print")
            {
                this.SetClassList();
                this.SetExport();
            }
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("JsFun", "ClassScheduleReport.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }


        void SetInit()
        {
           //查詢組織相關資料
            this.OrgDataInit = this.od.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>("SELECT *,OrgNameList AS OrgName FROM OrgStrucAllData('Unit') WHERE OrgNo<>''").ToList();
            this.ClassNoList.Add("1");
            this.ClassNoList.Add("2");
            this.ClassNoList.Add("3");
            this.ClassNoList.Add("0");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("0");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("0");
            Date1 = DateTime.Parse(NowMonth + "/01");
            Date2 = Date1.AddMonths(1);
            Date2 = Date2.AddDays(-1);
        }

        private void SetClassList()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string PsnNo = "", ClassMonth = "";
            string sqlMainCmd = @" SELECT DISTINCT TOP 300 A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID AND (A.PsnNo LIKE @PsnNo OR A.PsnName LIKE @PsnNo) ";
            if (Request.Form["PsnNo"] != null)
            {
                PsnNo = Request.Form["PsnNo"] + "%";
            }
            if (!this.GetFormEndValue("DeptList").Equals(""))
            {
                sqlMainCmd += " AND A.OrgStrucId=@OrgID";
            }
            this.PersonList = this.od.GetQueryResult<PersonEntity>(sqlMainCmd+" ORDER BY A.PsnNo", new { UserID, PsnNo, OrgID=this.GetFormEndValue("DeptList") }).OrderBy(i => i.PsnName).ToList();                     
            if (Request.Form["ClassMonth"] != null)
            {
                ClassMonth = Request.Form["ClassMonth"];
            }
          
            this.ClassList = this.od.GetQueryResult<B03Empclassschedule>("SELECT * FROM B03_EmpClassSchedule WHERE ClassMonth=@ClassMonth",new { PsnNo, ClassMonth }).ToList();
            this.HolidayList = this.od.GetQueryResult<B00Holidayex>("SELECT * FROM B00_HolidayEx WHERE HEDate LIKE @Month", new { Month = ClassMonth.Replace("/", "-")+"%" }).ToList();
            DateTime DateS = DateTime.Now, DateE = DateTime.Now, DateChk = DateTime.Now;
            Date1 = DateTime.Parse(Request.Form["ClassMonth"] + "/01");
            Date2 = Date1.AddMonths(1);
            Date2 = Date2.AddDays(-1);
            
        }



        /// <summary>
        /// 匯出班表
        /// </summary>
        void SetExport()
        {
            //建立Excel
            ExcelPackage ep = new ExcelPackage();
            //建立第一個Sheet，後方為定義Sheet的名稱
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("輪值排班表");
            sheet.Cells[1, 1].Value = "日期";
            sheet.Cells[2, 1].Value = "姓名/星期";
            var DateCs = Date1;
            int col = 2;
            int row = 1;
            string WeekName = "日一二三四五六";
            while (DateCs <= Date2)
            {                
                sheet.Cells[1, col].Value = DateCs.ToString("dd");
                DateCs = DateCs.AddDays(1);
                col++;
            }
            col = 2;
            DateCs = Date1;
            while (DateCs <= Date2)
            {                
                sheet.Cells[2, col].Value = WeekName.Substring((int)DateCs.DayOfWeek, 1);
                DateCs = DateCs.AddDays(1);
                col++;
            }
            row = 3;
            foreach(var p in this.PersonList)
            {
                sheet.Cells[row, 1].Value = p.PsnName;
                col = 2;
                DateCs = Date1;
                while (DateCs <= Date2)
                {                    
                    var showday = "";                    
                    if (p.Text5.Equals("ABCR") || p.Text5.Equals("0123"))
                    {
                        if (this.ClassList.Where(i =>p.PsnNo.Equals(i.PsnNo)&& i.ClassDate.ToString("yyyy/MM/dd").Equals(DateCs.ToString("yyyy/MM/dd"))).Count() > 0)
                        {
                            showday = this.ClassList.Where(i =>i.PsnNo.Equals(p.PsnNo)&& i.ClassDate.ToString("yyyy/MM/dd").Equals(DateCs.ToString("yyyy/MM/dd"))).First().ClassNo;
                            //showday += "\n"+this.ClassList.Where(i => i.ClassDate.ToString("yyyy/MM/dd").Equals(DateCs.ToString("yyyy/MM/dd"))).First().AdjClassNo;
                            if (showday.Equals("0"))
                            {
                                showday = "休";
                            }
                        }
                    }
                    else
                    {
                        showday = p.Text5;
                        if (this.HolidayList.Where(i =>i.HEDate.Equals(DateCs.ToString("yyyy-MM-dd"))).Count() > 0)
                        {
                            showday = "例";
                        }
                    }
                    sheet.Cells[row, col].Style.WrapText = true;
                    sheet.Cells[row, col].Value = showday;                 
                    col++;
                    DateCs = DateCs.AddDays(1);
                }
                row++;
            }

            col = 2;
            sheet.Cells.AutoFitColumns(); //自動欄寬
            while (Date1 <= Date2)
            {
                sheet.Column(col).Width = 4;
                col++;
                Date1 = Date1.AddDays(1);
            }
            row++;            
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=ClassScheduleReport.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();
        }

    }//end class
}//end namespace