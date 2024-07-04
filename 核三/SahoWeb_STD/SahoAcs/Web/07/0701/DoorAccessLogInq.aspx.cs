using DapperDataObjectLib;
using OfficeOpenXml;
using PagedList;
using Sa.DB;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;


namespace SahoAcs.Web._07._0701
{
    public partial class DoorAccessLogInq : System.Web.UI.Page
    {
        #region Global block
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string SortName = "LogTime";
        public string SortType = "ASC";
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<DBModel.AttendLogInq> ListLog = new List<DBModel.AttendLogInq>();

        public int PageIndex = 1;
        public string PsnNo = "", PsnID = "", PsnName = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string AuthList = "";
        public IPagedList<DBModel.AttendLogInq> PagedList;
        Pub.MessageObject sRet = new Pub.MessageObject() { result = true, message = "" };
        #endregion End 分頁參數
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "PrintExcel")
            {
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
                this.Calendar_CardTimeSDate.DateValue = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd 00:00:00");
                this.Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
            }
            ClientScript.RegisterClientScriptInclude("JsInclude1", "DoorAccessLogInq.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        private void SetInitData()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DBReader drOrg = null;
            DBReader drCon = null;
            string sql = string.Empty;

            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            //讀卡機資料處理
            sql = @" SELECT  B01_EquData.EquNo, B01_EquData.EquName, B01_EquData.AliveState, 
                         B01_EquData.VarStateTime,B01_Controller.CtrlID,B01_Controller.CtrlNo
                         From B01_DeviceConnInfo 
                         INNER JOIN B01_Master ON B01_DeviceConnInfo.DciID = B01_Master.DciID 
                         INNER JOIN B01_Controller ON B01_Master.MstID = B01_Controller.MstID 
                         INNER JOIN B01_Reader ON B01_Controller.CtrlID = B01_Reader.CtrlID 
                         INNER JOIN B01_EquData ON dbo.B01_Reader.EquNo = B01_EquData.EquNo
                         WHERE  B01_EquData.EquClass IN ('Door Access','Elevator')
                         --And B01_EquData.AliveState = '1'
                         order by EquName            
";
            oAcsDB.GetDataReader(sql, out drCon);
            while (drCon.Read())
            {
                ListItem Item = new ListItem();
                Item.Text = drCon.DataReader["EquName"].ToString() + "." + drCon.DataReader["EquNo"].ToString();
                Item.Value = drCon.DataReader["EquNo"].ToString();
                this.dropControl.Items.Add(Item);
            }
            this.dropControl.Items.Insert(0, new ListItem(" == 全部門禁讀卡機 == ", ""));

            //部門別資料處理
            sql = @"SELECT OrgID, OrgNo, OrgName FROM B01_OrgData
                        WHERE OrgClass = 'Department' ORDER BY OrgNo";
            oAcsDB.GetDataReader(sql, out drOrg);
            while (drOrg.Read())
            {
                ListItem Item = new ListItem();
                Item.Text = drOrg.DataReader["OrgName"].ToString() + "." + drOrg.DataReader["OrgNo"].ToString();
                Item.Value = drOrg.DataReader["OrgID"].ToString();
                this.dropDepartment.Items.Add(Item);
            }
            this.dropDepartment.Items.Insert(0, new ListItem(" == 全部 == ", ""));

        }

        private void SetQueryData()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");

            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string CardTimeSDate = Request["CardTimeSDate"];
            string CardTimeEDate = Request["CardTimeEDate"];
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            string Dep = Request["Dep"];
            string Control = Request["Control"];
            string LogStatus = Request["LogStatus"];
            string DoorNo = Request["DoorNo"];

            string Sql = string.Empty;
            Sql = @"Select
B01_CardLog.RecordID,B01_CardLog.ReceiveTime,B01_CardLog.CardTime,B01_CardLog.LogTime,B01_CardLog.PsnNo,B01_CardLog.PsnName,
B01_CardLog.CardNo,B01_CardLog.CardVer,B01_Controller.CtrlID,B01_Controller.CtrlNo,B01_Controller.CtrlName,
--OrgStrucAllData_1.OrgID,OrgStrucAllData_1.OrgStrucNo,OrgStrucAllData_1.OrgNameList,OrgStrucAllData_1.OrgNoList,
B01_CardLog.LogStatus,B00_CardLogState.StateDesc,
B01_CardLog.DepName
--OrgStrucAllData_1.OrgName As DepName
,B01_CardLog.EquNo,B01_CardLog.EquName
From B01_CardLog
--LEFT Join B01_Person On B01_CardLog.PsnNo = B01_Person.PsnNo
--LEFT Join OrgStrucAllData('Department') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
--INNER Join B01_Card ON B01_CardLog.CardNo = B01_Card.CardNo
INNER Join B01_Controller On B01_Controller.CtrlNo = B01_CardLog.CtrlNo
INNER Join B00_CardLogState On B00_CardLogState.Code = B01_CardLog.LogStatus
--LEFT Join B01_Reader On  B01_Controller.CtrlID = B01_Reader.CtrlID
Where 1=1
";
            Sql += " And convert(varchar, B01_CardLog.LogTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";

            if (!string.IsNullOrEmpty(CardNo))
            {
                Sql += " And B01_CardLog.CardNo = '" + CardNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnNo))
            {
                Sql += " And B01_CardLog.PsnNo = '" + PsnNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnName))
            {
                Sql += " And B01_CardLog.PsnName like N'" + PsnName.ToString().Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(Dep))
            {
                Sql += " And B01_CardLog.DepID = '" + Dep.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(Control))
            {
                Sql += " And B01_CardLog.EquNo = '" + Control.ToString().Trim() + "' ";
            }
            else
            {
                string Equ_No = odo.GetStrScalar("Select '''' + EquNo + ''',' from B01_EquData where EquClass IN ('Elevator','Door Access') FOR XML PATH('')");
                if (!string.IsNullOrEmpty(Equ_No))
                {
                    Equ_No = Equ_No.TrimEnd(',');
                    Sql += " And B01_CardLog.EquNo in (" + Equ_No + ")";
                }
            }

            if (!string.IsNullOrEmpty(LogStatus))
            {
                Sql += " And B01_CardLog.LogStatus ='" + LogStatus.ToString().Trim() + "' ";
            }
            if (!string.IsNullOrEmpty(DoorNo))
            {
                Sql += " And B01_CardLog.ReaderNo ='" + DoorNo.ToString().Trim() + "' ";
            }
            Sql += " ORDER BY B01_CardLog.LogTime ";
            this.ListLog = this.odo.GetQueryResult<DBModel.AttendLogInq>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName,
                Dep = Dep,
                Control = Control,
                LogStatus = LogStatus,
                UserID = UserID
            }).OrderByField("LogTime", true).ToList();

            if (Request["PageEvent"] == "PrintExcel")
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

        private void SetDoPage()
        {
            //啟始及結束頁
            StartPage = (PageIndex < ShowPage) ? 1 : PageIndex;
            if (StartPage > 1)
            {
                StartPage = (PageIndex + ShowPage / 2 >= this.PagedList.PageCount) ? this.PagedList.PageCount - ShowPage + 1 : PageIndex - ShowPage / 2;
            }
            EndPage = (StartPage - 1 > this.PagedList.PageCount - ShowPage) ? this.PagedList.PageCount + 1 : StartPage + ShowPage;
            //上下頁
            PrePage = PageIndex - 1 <= 1 ? 1 : PageIndex - 1;
            NextPage = PageIndex + 1 >= this.PagedList.PageCount ? this.PagedList.PageCount : PageIndex + 1;
        }

        private void ExportExcel()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string CardTimeSDate = Request["CardTimeSDate"];
            string CardTimeEDate = Request["CardTimeEDate"];
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            string Dep = Request["Dep"];
            string Control = Request["Control"];
            string LogStatus = Request["LogStatus"];
            string DoorNo = Request["DoorNo"];

            string Sql = string.Empty;
            Sql = @"Select
B01_CardLog.RecordID,B01_CardLog.ReceiveTime,B01_CardLog.CardTime,B01_CardLog.LogTime,B01_CardLog.PsnNo,B01_CardLog.PsnName,
B01_CardLog.CardNo,B01_CardLog.CardVer,B01_Controller.CtrlID,B01_Controller.CtrlNo,B01_Controller.CtrlName,
--OrgStrucAllData_1.OrgID,OrgStrucAllData_1.OrgStrucNo,OrgStrucAllData_1.OrgNameList,OrgStrucAllData_1.OrgNoList,
B01_CardLog.LogStatus,B00_CardLogState.StateDesc,
B01_CardLog.DepName
--OrgStrucAllData_1.OrgName As DepName
,B01_CardLog.EquNo,B01_CardLog.EquName
From B01_CardLog
--LEFT Join B01_Person On B01_CardLog.PsnNo = B01_Person.PsnNo
--LEFT Join OrgStrucAllData('Department') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
--INNER Join B01_Card ON B01_CardLog.CardNo = B01_Card.CardNo
INNER Join B01_Controller On B01_Controller.CtrlNo = B01_CardLog.CtrlNo
INNER Join B00_CardLogState On B00_CardLogState.Code = B01_CardLog.LogStatus
--LEFT Join B01_Reader On  B01_Controller.CtrlID = B01_Reader.CtrlID
Where 1=1
";
            Sql += " And convert(varchar, B01_CardLog.LogTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";

            if (!string.IsNullOrEmpty(CardNo))
            {
                Sql += " And B01_CardLog.CardNo = '" + CardNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnNo))
            {
                Sql += " And B01_CardLog.PsnNo = '" + PsnNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnName))
            {
                Sql += " And B01_CardLog.PsnName like N'" + PsnName.ToString().Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(Dep))
            {
                Sql += " And B01_CardLog.DepID = '" + Dep.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(Control))
            {
                Sql += " And B01_CardLog.EquNo = '" + Control.ToString().Trim() + "' ";
            }
            else
            {
                string Equ_No = odo.GetStrScalar("Select '''' + EquNo + ''',' from B01_EquData where EquClass = 'Door Access' FOR XML PATH('')");
                if (!string.IsNullOrEmpty(Equ_No))
                {
                    Equ_No = Equ_No.TrimEnd(',');
                    Sql += " And B01_CardLog.EquNo in (" + Equ_No + ")";
                }
            }

            if (!string.IsNullOrEmpty(LogStatus))
            {
                Sql += " And B01_CardLog.LogStatus ='" + LogStatus.ToString().Trim() + "' ";
            }
            if (!string.IsNullOrEmpty(DoorNo))
            {
                Sql += " And B01_CardLog.ReaderNo ='" + DoorNo.ToString().Trim() + "' ";
            }

            Sql += " ORDER BY B01_CardLog.LogTime ";
            this.ListLog = this.odo.GetQueryResult<DBModel.AttendLogInq>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName,
                Dep = Dep,
                Control = Control,
                LogStatus = LogStatus,
                UserID = UserID
            }).OrderByField("LogTime", true).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("門禁刷卡記錄查詢");
            
            ws.Cells[1, 1].Value = "刷卡時間";
            ws.Cells[1, 2].Value = "卡號";
            ws.Cells[1, 3].Value = "人員編號";
            ws.Cells[1, 4].Value = "姓名";
            ws.Cells[1, 5].Value = "部門";
            ws.Cells[1, 6].Value = "讀卡機編號";
            ws.Cells[1, 7].Value = "讀卡機名稱";
            ws.Cells[1, 8].Value = "刷卡結果";
            int count = 2;
            foreach (var o in this.ListLog)
            { 
                ws.Cells[count, 1].Value = o.LogTime.ToString("yyyy/MM/dd HH:mm:ss");
                ws.Cells[count, 2].Value = o.CardNo;
                ws.Cells[count, 3].Value = o.PsnNo;
                ws.Cells[count, 4].Value = o.PsnName;
                ws.Cells[count, 5].Value = o.DepName;
                ws.Cells[count, 6].Value = o.EquNo;
                ws.Cells[count, 7].Value = o.EquName;
                ws.Cells[count, 8].Value = o.StateDesc;
                count++;
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=門禁刷卡記錄查詢.xls");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }
}