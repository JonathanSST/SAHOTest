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
    public partial class AttendLogInq : System.Web.UI.Page
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
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "PrintTxt")
            {
                this.SetQueryData();
                this.ExportTxt();
            }
            else
            {
                this.SetInitData();
                this.Calendar_CardTimeSDate1.DateValue = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd 14:00:00");
                this.Calendar_CardTimeEDate1.DateValue = DateTime.Now.ToString("yyyy/MM/dd 14:00:00");
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
            }
            ClientScript.RegisterClientScriptInclude("JsInclude1", "AttendLogInq.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
                         WHERE B01_EquData.EquClass='TRT'  AND B01_EquData.EquNo NOT LIke 'R%'
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
            this.dropControl.Items.Insert(0, new ListItem(" == 全部考勤讀卡機 == ", ""));

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

            this.chk_EmpCardNo.Checked = true;
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
            string IsEmp = Request["IsEmp"] ;
            string OrderByTime = Request["OrderByTime"];

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
            if (OrderByTime.Equals("byLogTime"))
            {
                Sql += " And convert(varchar, B01_CardLog.LogTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";
            }
            else
            {
                Sql += " And convert(varchar, B01_CardLog.ReceiveTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";
            }

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
                string Equ_No = odo.GetStrScalar("Select '''' + EquNo + ''',' from B01_EquData where EquClass = 'TRT' and EquNo NOT LIKE 'R%' FOR XML PATH('')");
                if (!string.IsNullOrEmpty(Equ_No))
                {
                    Equ_No = Equ_No.TrimEnd(',');
                    Sql += " And B01_CardLog.EquNo in (" + Equ_No + ")";
                }
            }

            bool _isEmp = true;
            if (!string.IsNullOrEmpty(IsEmp))
            {
                _isEmp = Convert.ToBoolean(IsEmp);
            }
            if (_isEmp)
            {
                Sql += " And SUBSTRING(B01_CardLog.CardNo, 1, 4) <> '0000' ";
            }
            else
            {
                Sql += " And SUBSTRING(B01_CardLog.CardNo, 1, 4) = '0000' ";
            }

            Sql += " And B01_CardLog.LogStatus >= 48 AND B01_CardLog.LogStatus <=57 ";

            if (OrderByTime.Equals("byLogTime"))
            {
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
                    UserID = UserID
                }).OrderByField("LogTime", true).ToList();
            }
            else
            {
                Sql += " ORDER BY B01_CardLog.ReceiveTime ";
                this.ListLog = this.odo.GetQueryResult<DBModel.AttendLogInq>(Sql.ToString(), new
                {
                    CardTimeSDate = CardTimeSDate,
                    CardTimeEDate = CardTimeEDate,
                    CardNo = CardNo,
                    PsnNo = PsnNo,
                    PsnName = PsnName,
                    Dep = Dep,
                    Control = Control,
                    UserID = UserID
                }).OrderByField("ReceiveTime", true).ToList();
            }

            if (Request["PageEvent"] == "PrintTxt" || Request["PageEvent"] == "PrintExcel")
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
            string IsEmp = Request["IsEmp"];
            string OrderByTime = Request["OrderByTime"];

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
            if (OrderByTime.Equals("byLogTime"))
            {
                Sql += " And convert(varchar, B01_CardLog.LogTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";
            }
            else
            {
                Sql += " And convert(varchar, B01_CardLog.ReceiveTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";
            }
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
                string Equ_No = odo.GetStrScalar("Select '''' + EquNo + ''',' from B01_EquData where EquClass = 'TRT' and EquNo NOT LIKE 'R%' FOR XML PATH('')");
                if (!string.IsNullOrEmpty(Equ_No))
                {
                    Equ_No = Equ_No.TrimEnd(',');
                    Sql += " And B01_CardLog.EquNo in (" + Equ_No + ")";
                }
            }

            bool _isEmp = true;
            if (!string.IsNullOrEmpty(IsEmp))
            {
                _isEmp = Convert.ToBoolean(IsEmp);
            }
            if (_isEmp)
            {
                Sql += " And SUBSTRING(B01_CardLog.CardNo, 1, 4) <> '0000' ";
            }
            else
            {
                Sql += " And SUBSTRING(B01_CardLog.CardNo, 1, 4) = '0000' ";
            }

            Sql += " And B01_CardLog.LogStatus >= 48 AND B01_CardLog.LogStatus <=57 ";



            if (OrderByTime.Equals("byLogTime"))
            {
                Sql += " ORDER BY B01_CardLog.LogTime ";
            }
            else
            {
                Sql += " ORDER BY B01_CardLog.ReceiveTime ";
            }

            this.ListLog = this.odo.GetQueryResult<DBModel.AttendLogInq>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName,
                Dep = Dep,
                Control = Control,
                UserID = UserID
            }).OrderByField("ReceiveTime", true).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("考勤刷卡記錄查詢");
            ws.Cells[1, 1].Value = "接收時間";
            ws.Cells[1, 2].Value = "刷卡時間";
            ws.Cells[1, 3].Value = "卡號";
            ws.Cells[1, 4].Value = "人員編號";
            ws.Cells[1, 5].Value = "姓名";
            ws.Cells[1, 6].Value = "部門";
            ws.Cells[1, 7].Value = "讀卡機編號";
            ws.Cells[1, 8].Value = "讀卡機名稱";
            ws.Cells[1, 9].Value = "刷卡結果";
            int count = 2;
            foreach (var o in this.ListLog)
            {
                ws.Cells[count, 1].Value = o.ReceiveTime.ToString("yyyy/MM/dd HH:mm:ss");
                ws.Cells[count, 2].Value = o.CardTime.ToString("yyyy/MM/dd HH:mm:ss");
                ws.Cells[count, 3].Value = o.CardNo;
                ws.Cells[count, 4].Value = o.PsnNo;
                ws.Cells[count, 5].Value = o.PsnName;
                ws.Cells[count, 6].Value = o.DepName;
                ws.Cells[count, 7].Value = o.EquNo;
                ws.Cells[count, 8].Value = o.EquName;
                ws.Cells[count, 9].Value = o.StateDesc;
                count++;
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=考勤刷卡記錄查詢.xls");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void ExportTxt()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string CardTimeSDate = Request["CardTimeSDate"];
            string CardTimeEDate = Request["CardTimeEDate"];
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            string Dep = Request["Dep"];
            string Control = Request["Control"];
            string IsEmp = Request["IsEmp"];
            string OrderByTime = Request["OrderByTime"];
            DateTime EndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(CardTimeEDate))
            {
                EndDate = Convert.ToDateTime(CardTimeEDate);
            }

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
--LEFT Join B01_Reader On  B01_Controller.CtrlID = B01_Reader.ReaderID
Where 1=1
";
            if (OrderByTime.Equals("byLogTime"))
            {
                Sql += " And convert(varchar, B01_CardLog.LogTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";
            }
            else
            {
                Sql += " And convert(varchar, B01_CardLog.ReceiveTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";
            }
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
                string Equ_No = odo.GetStrScalar("Select '''' + EquNo + ''',' from B01_EquData where EquClass = 'TRT' and EquNo NOT LIKE 'R%' FOR XML PATH('')");
                if (!string.IsNullOrEmpty(Equ_No))
                {
                    Equ_No = Equ_No.TrimEnd(',');
                    Sql += " And B01_CardLog.EquNo in (" + Equ_No + ")";
                }
            }

            bool _isEmp = true;
            if (!string.IsNullOrEmpty(IsEmp))
            {
                _isEmp = Convert.ToBoolean(IsEmp);
            }
            if (_isEmp)
            {
                Sql += " And SUBSTRING(B01_CardLog.CardNo, 1, 4) <> '0000' ";
            }
            else
            {
                Sql += " And SUBSTRING(B01_CardLog.CardNo, 1, 4) = '0000' ";
            }

            Sql += " And B01_CardLog.LogStatus >= 48 AND B01_CardLog.LogStatus <=57 ";



            if (OrderByTime.Equals("byLogTime"))
            {
                Sql += " ORDER BY B01_CardLog.LogTime ";
            }
            else
            {
                Sql += " ORDER BY B01_CardLog.ReceiveTime ";
            }

            this.ListLog = this.odo.GetQueryResult<DBModel.AttendLogInq>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName,
                Dep = Dep,
                Control = Control,
                UserID = UserID
            }).OrderByField("ReceiveTime", true).ToList();

            string sFileName = EndDate.ToString("yyyyMMdd") + ".dat";
            string fileName = string.Format("{0}{1}", System.IO.Path.GetTempPath(), "tempfile_" + System.Guid.NewGuid().ToString("N") + ".dat");

            try
            {
                Response.ContentEncoding = System.Text.Encoding.Default;
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, false, Response.ContentEncoding))
                {
                    foreach (var o in this.ListLog)
                    {
                        string strCardNo = o.CardNo.ToString().Trim();
                        DateTime dtTime = Convert.ToDateTime(o.CardTime);
                        string strLDate = dtTime.ToString("yyyyMMdd");                                  //西元年月日 (8)
                        string strLTime = dtTime.ToString("HH:mm");
                        string strLogStatus = o.LogStatus.ToString().Trim();                            //燈號 
                        int nLogStatus = 49;            //'A'                        
                        int.TryParse(strLogStatus, out nLogStatus);

                        string strStatusCode = "A";     //初始值
                        if (nLogStatus >= 48 && nLogStatus <= 57)
                            strStatusCode = (nLogStatus == 48) ? "J" : ((char)(nLogStatus + 16)).ToString();
                        else
                            strStatusCode = "N";        //可能是未授權卡
                        string strCtrlNo = o.CtrlNo.ToString().Trim();
                        writer.WriteLine(strCardNo + "|" + strLDate + "|" + strLTime + "|" + strStatusCode + "|" + strCtrlNo + "|0| | | | |");
                    }
                        
                }

                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=" + sFileName);
                Response.ContentType = "text/plain";
                //Response.Charset = "big5";
                //Response.CodePage = 1252;
                Response.Charset = "windows-1252";
                Response.WriteFile(fileName);
                Response.End();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}