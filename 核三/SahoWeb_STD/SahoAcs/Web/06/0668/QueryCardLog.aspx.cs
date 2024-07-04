using DapperDataObjectLib;
using OfficeOpenXml;
using PagedList;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._06._0668
{
    public partial class QueryCardLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> ListLog = new List<CardLogModel>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "DESC";

        public List<string> EquDatas = new List<string>();
        public List<string> OrgDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<CardLogModel> PagedList;

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
            ClientScript.RegisterClientScriptInclude("JsInclude1", "QueryCardLog.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE()");

            if (dtLastCardTime == DateTime.MinValue)
            {
                //DateTime.Now.ToFileTimeUtc();                
                Calendar_CardTimeSDate.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 00:00:00";
                Calendar_CardTimeEDate.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 23:59:59";
                ADVCalendar_CardTimeSDate.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 00:00:00";
                ADVCalendar_CardTimeEDate.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 23:59:59";
            }
            else
            {
                Calendar_CardTimeSDate.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd") + " 00:00:00";
                Calendar_CardTimeEDate.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd") + " 23:59:59";
                ADVCalendar_CardTimeSDate.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd") + " 00:00:00";
                ADVCalendar_CardTimeEDate.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd") + " 23:59:59";
            }
            #endregion

            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeVal", DataRealName = "CardTime", DataWidth = 123, TitleWidth = 120, TitleName = "讀卡時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = "部門名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80, TitleName = "人員編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "人員姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 74, TitleWidth = 70, TitleName = "卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "TempCardNo", DataWidth = 74, TitleWidth = 70, TitleName = "臨時卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardVer", DataWidth = 64, TitleWidth = 60, TitleName = "版次" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "設備編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 104, TitleWidth = 100, TitleName = "設備名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = "讀卡結果" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogTimeVal", DataRealName = "LogTime", DataWidth = 154, TitleWidth = 150, TitleName = "記錄時間" });
            //this.ListCols.Add(new ColNameObj() { ColName = "HeatResult", DataRealName = "HeatResult", DataWidth = 84, TitleWidth = 80, TitleName = "體溫值" });
            foreach (var o in this.ListCols.Where(i => i.DataRealName == null || i.DataRealName == ""))
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
            string sqlorgjoin = @"SELECT DISTINCT O.OrgIDList FROM B01_OrgStruc O
                INNER JOIN B01_Person P ON P.OrgStrucID=O.OrgStrucID
                INNER JOIN B01_MgnOrgStrucs MO ON MO.OrgStrucID=O.OrgStrucID
                INNER JOIN B00_SysUserMgns M ON MO.MgaID = M.MgaID
                WHERE M.UserID=@UserID ";
            this.OrgDatas = this.odo.GetQueryResult<OrgStrucInfo>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i => i.OrgIDList).ToList();
            var McrInfo = this.odo.GetQueryResult<EquGroupData>("SELECT * FROM V_McrInfo").ToList();
            sql = @"SELECT A.* FROM 
                        B01_CardLog A WHERE (A.OrgStruc IN @OrgDatas)";
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            sql += this.GetConditionCmdStr();
            this.ListLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                OrgDatas = this.OrgDatas,
                PsnName = "%" + this.txt_CardNo_PsnName + "%",
                CardTimeS = DateTime.Parse(Request["CardTimeS"]).GetUtcTime(this),
                CardTimeE = DateTime.Parse(Request["CardTimeE"]).GetUtcTime(this),
                LogTimeS = Request["LogTimeS"],
                LogTimeE = Request["LogTimeE"],
                DepName = "%" + Request["DepNoDepName"] + "%",
                EquName = "%" + Request["EquNoEquName"] + "%",
                PsnNo = "%" + Request["ADVPsnNo"] + "%",
                LogStatus = Request["LogStatus"].Split(','),
                DepID = Request["DepNo"].Split(','),
                PsnID = Request["PsnID"]
            }).ToList();
            int localTimeZone = 8;
            this.ListLog = this.ListLog.OrderByField(this.SortName, boolSort).ToList();
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

            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));

            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]).GetZoneTime(this));
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", r["LogTime"]);
                r["LogStatus"] = logstatus.Where(i => Convert.ToInt32(i.Code) == Convert.ToInt32(r["LogStatus"])).FirstOrDefault().StateDesc;
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
                    sql += " AND PsnNo LIKE @PsnNo";
                }
                if (Request["DepNoDepName"] != null && Request["DepNoDepName"] != "")
                {
                    sql += " AND DepName LIKE @DepName";
                }
                if (Request["EquNoEquName"] != null && Request["EquNoEquName"] != "")
                {
                    sql += " AND (B01_CardLog.EquNo LIKE @EquName OR EquName LIKE @EquName )";
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
                    sql += " AND DepID IN @DepID AND (DepName IS NOT NULL AND DepName !='' )";
                }
            }
            //一般查詢的方法
            sql += " AND CardTime BETWEEN @CardTimeS AND @CardTimeE ";
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
            }
            if (Request["LogStatus"] != null && Request["LogStatus"] != "")
            {
                sql += " AND LogStatus IN @LogStatus ";
            }
            if (Request["PsnID"] != "")
            {
                sql += " AND PsnNo IN (SELECT PsnNo FROM B01_Person WHERE PsnID=@PsnID)";
            }
            return sql;
        }

        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");


            for (int i = 0; i < this.ListCols.Count; i++)
            {
                ws.Cells[1, i + 1].Value = this.ListCols[i].TitleName;
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
            Response.AddHeader("content-disposition", "attachment; filename=QueryCardLog.xlsx");
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
                foreach (var o in result)
                {
                    Item = new ListItem();
                    var copyItem = new ListItem();
                    Item.Text = Convert.ToString(o.StateDesc);
                    Item.Value = Convert.ToString(o.Code);//dr["Code"].ToString();
                    copyItem = Item;
                    this.DropDownList_LogStatus.Items.Add(Item);
                }
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


            var result = this.odo.GetQueryResult(sqlouttable, new { UserID = this.hideUserID.Value });
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

            var result = this.odo.GetQueryResult(sql, new { UserID = this.hideUserID.Value });

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
            for (int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.CardLogModel()
                {
                    CardTime = DateTime.Now,
                    LogTime = DateTime.Now
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));
            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LogTime"]));
            }
        }

        void SetOneCardLog()
        {
            string RecordID = Request["RecordID"];
            Response.Clear();
            string PsnPicSource = "";
            string Url = "http://127.0.0.1:8080/";
            var PsnPicSourceUrl = this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaClass='Person' AND ParaNo='Url' ");
            foreach (var o in PsnPicSourceUrl)
            {
                Url = Convert.ToString(o.ParaValue);
            }
            var logs = this.odo.GetQueryResult<CardLogModel>(@"declare @input varchar(max), @sep varchar(2)
                     set @sep = ';'
                     set @input = (select paraValue from B00_SysParameter where ParaNo = 'EvoList')
                    SELECT B01_CardLog.*,CamNo,DATEADD(HOUR,-V.TimeZone,CardTime) AS CardTime FROM B01_CardLog INNER JOIN V_MCRInfo V  ON B01_CardLog.EquNo=V.EquNo
                    LEFT JOIN (select dbo.GetLeftStr(Value, '|') as EquNo, dbo.GetRightStr(Value, '|') as CamNo from dbo.SplitString(@input, @sep, 1)) b on B01_CardLog.EquNo = b.EquNo
                    WHERE RecordID=@RecordID", new { RecordID = RecordID });
            var LogPic = this.odo.GetQueryResult("SELECT * FROM B01_CardLogExt WHERE CardLogID=@RecordID", new { RecordID = RecordID });
            foreach (var o in logs)
            {
                o.CardTime = o.CardTime.GetUtcToZone(this);
            }
            string path = System.IO.Path.GetDirectoryName(this.Request.PhysicalPath);
            path = Request.Url.AbsoluteUri;
            path = path.Substring(0, path.ToLower().LastIndexOf(@"/web"));
            path = string.Concat(path, "/CardImg/", logs.First().RecordID, ".", WebAppService.GetSysParaData("CardPic"));
            logs.First().CardPicPath = path;
            foreach (var o in LogPic)
            {
                logs.First().CardPicSource = Convert.ToString(o.CardPicSource);
            }
            var PsnDatas = this.odo.GetQueryResult<PersonEntity>("SELECT * FROM B01_Person WHERE PsnNo=@PsnNo", new { PsnNo = logs.First().PsnNo });
            foreach (var o in PsnDatas)
            {
                if (o.PsnPicSource != null && o.PsnPicSource != "")
                {
                    PsnPicSource = o.PsnPicSource;
                }
                else
                {
                    PsnPicSource = "/Img/Default.png";
                }
            }
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
               new { message = this.odo.DbExceptionMessage, card_log = logs.First(), error_msg = "", PsnPicSource = Url + PsnPicSource }));
            Response.End();
        }


    }
}