﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using System.Data;
using PagedList;
using OfficeOpenXml;

namespace SahoAcs
{
    public partial class ESDCardLogByGarmin : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<ESDCardLogModel> ListLog = new List<ESDCardLogModel>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        
        public IPagedList<ESDCardLogModel> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
                this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            ClientScript.RegisterClientScriptInclude("ESDCardLog", "ESDCardLogByGarmin.js?"+DateTime.Now.ToString("yyyyMMddHHmm"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE() 
            AND EquNo IN (SELECT EquNo FROM B01_EquData)
            AND PsnNo IN (SELECT PsnNo FROM B01_Person)");

            if (dtLastCardTime == DateTime.MinValue)
            {
                Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                ADVCalendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                ADVCalendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
            }
            else
            {
                Calendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                Calendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
                ADVCalendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                ADVCalendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
            }
            #endregion

            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = base.GetLocalResourceObject("ttPsnName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "OrgName",  DataWidth = 124, TitleWidth = 120, TitleName = GetLocalResourceObject("ttOrgName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80, TitleName = GetLocalResourceObject("ttPsnNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CTDate", DataWidth = 74, TitleWidth = 70, TitleName = GetLocalResourceObject("ttCTDate").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CTTime", DataWidth = 64, TitleWidth = 60, TitleName = GetLocalResourceObject("ttCTTime").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "ESD1", DataWidth = 94, TitleWidth = 90, TitleName = GetLocalResourceObject("ttESD1").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "ESD2", DataWidth = 94, TitleWidth = 90, TitleName = GetLocalResourceObject("ttESD2").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "ESD3", DataWidth = 94, TitleWidth = 90, TitleName = GetLocalResourceObject("ttESD3").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "ESD4", DataWidth = 94, TitleWidth = 90, TitleName = GetLocalResourceObject("ttESD4").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = GetLocalResourceObject("ttEquNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 84, TitleWidth = 80, TitleName = GetLocalResourceObject("ttEquName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "LogTimeVal", DataRealName = "LogTime", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttLogTime").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttResult").ToString() });
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            
            #endregion
            this.CreateDropDownList_LogStatusItem();
            this.CreateDropDownList_DepItem();
            this.CreateDropDownList_EquItem();
        }


        /// <summary>設定主資料表的分頁</summary>
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


        private void SetQueryData()
        {
            if (Request["SortName"] != null)
            {
                this.SortName = Request["SortName"];
                this.SortType = Request["SortType"];
            }
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");
            foreach (var s in Request.Form.GetValues("ColName"))
            {                
                collist.Add(this.ListCols.Where(i => i.ColName == s).FirstOrDefault());
            }
            this.ListCols = collist;            
            string sql = "";
            string sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i=>i.EquNo).ToList();
            sql = @" SELECT C.PsnNo,OrgStrucID,CardTime,ESDResult,EquNo,EquName,LogTime,LogStatus,DepName AS OrgName,P.PsnName AS PsnName 
                              FROM B01_CardLog C
                              INNER JOIN B01_Person P
                              ON C.PsnNo = P.PsnNo
                              WHERE (EquNo IN @EquList) AND (SELECT COUNT(*) FROM SplitString(ESDResult,'|',0))>=8";
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            sql += this.GetConditionCmdStr();

                this.ListLog = this.odo.GetQueryResult<ESDCardLogModel>(sql, new
                {
                    EquList = EquDatas,
                    PsnName = "%" + this.txt_CardNo_PsnName + "%",
                    CardTimeS = Request["CardTimeS"],
                    CardTimeE = Request["CardTimeE"],
                    LogTimeS = Request["LogTimeS"],
                    LogTimeE = Request["LogTimeE"],
                    DepName = "%" + Request["DepNoDepName"] + "%",
                    EquName = "%" + Request["EquNoEquName"] + "%",
                    PsnNo = "%" + Request["ADVPsnNo"] + "%",
                    LogStatus = Request["LogStatus"].Split(','),
                    DepID = Request["DepNo"].Split(','),
                    PsnID = Request["PsnID"]
                }).OrderByField(this.SortName, boolSort).ToList();
                

            

            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                //轉datatable
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }

            //this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
            //this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));

            foreach (DataRow r in this.DataResult.Rows)
            {
                //var OrId = r["OrgStrucID"];
                //string ESDsConn = $"SELECT OrgName FROM OrgStrucData('unit',{OrId})";
                //var ESDDate2 = this.odo.GetQueryResult(ESDsConn);

                //foreach (var o in ESDDate2)
                //{ r["OrgName"] = o.OrgName; }


                string[] sESD = r["ESDResult"].ToString().Split('|');
                int iESDlen = sESD.Length;
                if (sESD.Length >= 4)
                {
                    r["ESD1"] = sESD[iESDlen - 4];
                    r["ESD2"] = sESD[iESDlen - 3];
                    r["ESD3"] = sESD[iESDlen - 2];
                    r["ESD4"] = sESD[iESDlen - 1];
                }
                r["CTDate"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(r["CardTime"]));
                r["CTTime"] = string.Format("{0:HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(r["LogTime"]));
                r["LogStatus"] = (r["LogStatus"].ToString()=="0")?"PASS":"FAIL";
            }
        }


        private string GetConditionCmdStr()
        {
            string sql = "";
            this.txt_CardNo_PsnName = Request["CardNoPsnNo"];
            //進階查詢功能設定
            if (Request["QueryMode"] == "2")
            {
                this.txt_CardNo_PsnName = Request["ADVPsnNameCardNo"];
                if (Request["ADVPsnNo"] != null && Request["ADVPsnNo"] != "")
                {
                    sql += " AND P.PsnNo LIKE @PsnNo";
                }
                if (Request["DepNoDepName"] != null && Request["DepNoDepName"] != "")
                {
                    sql += " AND DepName LIKE @DepName";
                }
                if (Request["EquNoEquName"] != null && Request["EquNoEquName"] != "")
                {
                    sql += " AND ( EquNo LIKE @EquName OR EquName LIKE @EquName )";
                }
                if (Request["LogTimeS"] != null && Request["LogTimeS"] != "")
                {
                    sql += " AND LogTime >= @LogTimeS";
                }
                if (Request["LogTimeE"] != null && Request["LogTimeE"] != "")
                {
                    sql += " AND LogTime <= @LogTimeE";
                }
                if (Request["EquNo"] != null && Request["EquNo"] != "")
                {
                    this.EquDatas = Request["EquNo"].Split(',').ToList();
                }
                if (Request["DepNo"] != null && Request["DepNo"] != "")
                {
                    sql += " AND DepID IN @DepID";
                }
            }
            //一般查詢的方法
            sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " AND (P.PsnNo LIKE @PsnName OR P.PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
            }
            if (Request["LogStatus"] != null && Request["LogStatus"] != "")
            {
                sql += " AND LogStatus IN @LogStatus ";
            }
            if (Request["PsnID"] != "")
            {
                sql += " AND P.PsnNo IN (SELECT PsnNo FROM B01_Person WHERE PsnID=@PsnID)";
            }
            return sql;
        }



        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");


            for (int i = 0; i < this.ListCols.Count; i++)
            {
                ws.Cells[1, i + 1].Value = this.ListCols[i].TitleName.Replace("<br/>","");
            }

            //Content
            for (int i = 0; i < this.DataResult.Rows.Count; i++)
            {
                for (int col = 0; col < this.ListCols.Count(); col++)
                {
                    ws.Cells[i + 2, col + 1].Value = this.DataResult.Rows[i][this.ListCols[col].ColName].ToString();
                }
            }
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=0601.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }


        #region CreateDropDownList_LogStatusItem

        private void CreateDropDownList_LogStatusItem()
        {
            System.Web.UI.WebControls.ListItem Item;                        
            List<string> liSqlPara = new List<string>();
            this.DropDownList_LogStatus.Items.Clear();
            this.ADVDropDownList_LogStatus.Items.Clear();

            var result = this.odo.GetQueryResult("SELECT Code, StateDesc FROM B00_CardLogState AS CardLogState");
            if (result.Count() > 0)
            {
                this.DropDownList_LogStatus.Items.Add(new ListItem() { Text = "PASS", Value = "0" });
                this.DropDownList_LogStatus.Items.Add(new ListItem() { Text = "FAIL", Value = "160" });
                //foreach (var o in result)
                //{
                //    Item = new ListItem();
                //    var copyItem = new ListItem();
                //    Item.Text = Convert.ToString(o.StateDesc);
                //    Item.Value = Convert.ToString(o.Code);//dr["Code"].ToString();
                //    copyItem = Item;
                //    this.DropDownList_LogStatus.Items.Add(Item);
                //}
            }
            else
            {
                this.DropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
            this.ADVDropDownList_LogStatus.Items.AddRange(this.DropDownList_LogStatus.Items.OfType<ListItem>().ToArray());
            
        }
        #endregion

        #region CreateDropDownList_DepItem
        private void CreateDropDownList_DepItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            string sql = "", wheresql = "";
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_Dep.Items.Clear();

            #region Process String
            string sqlouttable = "";
            sql = @" SELECT DISTINCT
                     OrgStrucAllData.OrgID AS DepID, OrgStrucAllData.OrgName AS DepName,UserID
                     FROM  OrgStrucAllData('@Type') AS OrgStrucAllData
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID  ";
            sqlouttable += "SELECT * FROM (";
            sqlouttable += sql.Replace("@Type", "Department");
            sqlouttable += " UNION ";
            sqlouttable += sql.Replace("@Type", "Unit");
            sqlouttable += " UNION ";
            sqlouttable += sql.Replace("@Type", "Title");
            sqlouttable += ") AS ResultTable ";
            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (UserID = @UserID ) ";
            wheresql += " AND DepID<>'' ";
            #endregion

            #endregion

            sqlouttable += " WHERE " + wheresql + " ORDER BY DepName ";

            
            var result = this.odo.GetQueryResult(sqlouttable, new {UserID=this.hideUserID.Value});
            if (result.Count() > 0)
            {
                foreach (var o in result)
                {
                    if (Convert.ToString(o.DepName) != "")
                    {
                        Item = new System.Web.UI.WebControls.ListItem();
                        Item.Text = Convert.ToString(o.DepName);
                        Item.Value = Convert.ToString(o.DepID);
                        ADVDropDownList_Dep.Items.Add(Item);
                    }

                }
            }
            else
            {
                ADVDropDownList_Dep.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion


        #region CreateDropDownList_EquItem
        private void CreateDropDownList_EquItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            string sql = "", wheresql = "";            
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_Equ.Items.Clear();

            #region Process String
            sql = @" SELECT DISTINCT
                     EquData.EquNo, EquData.EquName
                     FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUserMgns.UserID = @UserID ) ";            
            #endregion

            #endregion

            sql += " WHERE " + wheresql + " ORDER BY EquData.EquNo ";
            
            var result = this.odo.GetQueryResult(sql, new {UserID=this.hideUserID.Value});

            if (result.Count() > 0)
            {
                foreach (var o in result)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = Convert.ToString(o.EquName); // dr["EquName"].ToString();
                    Item.Value = Convert.ToString(o.EquNo); //dr["EquNo"].ToString();
                    ADVDropDownList_Equ.Items.Add(Item);
                }
            }
            else
            {
                ADVDropDownList_Equ.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion


        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.ESDCardLogModel()
                {
                    //PsnName = "TEST",
                    CardTime = DateTime.Now,
                    LogTime = DateTime.Now
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));

            foreach (DataRow r in this.DataResult.Rows)
            {
                var OrId = r["OrgStrucID"];
                string ESDsConn = $"SELECT OrgName FROM OrgStrucData('unit',{OrId})";
                var ESDDate2 = this.odo.GetQueryResult(ESDsConn);

                foreach (var o in ESDDate2)
                {
                    r["OrgName"] = o.OrgName;
                }
                string[] sESD = r["ESDResult"].ToString().Split('|');
                int iESDlen = sESD.Length;
                r["ESD1"] = sESD[iESDlen - 4];
                r["ESD2"] = sESD[iESDlen - 3];
                r["ESD3"] = sESD[iESDlen - 2];
                r["ESD4"] = sESD[iESDlen - 1];
                r["CTDate"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(r["CardTime"]));
                r["CTTime"] = string.Format("{0:HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(r["LogTime"]));
                r["LogStatus"] = (r["LogStatus"].ToString() == "0") ? "PASS" : "FAIL";
            }
        }

    }//end page class
}//end namespace