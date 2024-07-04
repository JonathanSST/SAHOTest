using System;
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
    public partial class _0603_3 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> ListLog = new List<CardLogModel>();

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
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Resend")
            {
                //this.SetOneCardLog();
                if (Request["ChkOne"] != null)
                {
                    string[] RecordList = Request.Form.GetValues("ChkOne");
                    this.odo.Execute("UPDATE B01_CardLog SET SyncMark3=0 WHERE RecordID IN @RecordList", new {RecordList=RecordList});
                }
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                   new { message = "OK", ErrorMessage = "" }));
                Response.End();
            }
            else
            {
                this.SetInitData();
                //this.EmptyCondition();
                this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            }
            ClientScript.RegisterClientScriptInclude("0603", "0603_3.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE()");

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
            string sql = "";
            string sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i => i.EquNo).ToList();
            //在中科一所的環境，須可以用身份證號查詢自己的讀卡記錄
            sql = @" SELECT * FROM B01_CardLog A INNER JOIN B01_Person B ON A.PsnNo=B.PsnNo WHERE (EquClass='TRT' OR IsAndTrt=1) ";
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            sql += this.GetConditionCmdStr();
            this.ListLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                EquList = EquDatas,
                PsnName = "%" + this.txt_CardNo_PsnName + "%",
                IDNum=this.txt_CardNo_PsnName,
                CardTimeS = Convert.ToDateTime(Request["CardTimeS"]),
                CardTimeE = Convert.ToDateTime(Request["CardTimeE"])
                //LogTimeS = Request["LogTimeS"],
                //LogTimeE = Request["LogTimeE"],
                //DepName = "%" + Request["DepNoDepName"] + "%",
                //EquName = "%" + Request["EquNoEquName"] + "%",
                //PsnNo = "%" + Request["ADVPsnNo"] + "%",
                //LogStatus = Request["LogStatus"].Split(','),
                //DepID = Request["DepNo"].Split(','),
                //PsnID = Request["PsnID"]
            }).OrderByField(this.SortName, boolSort).ToList();
            this.ListLog = this.ListLog.Where(i => this.EquDatas.Contains(i.EquNo)).ToList();
            this.ListLog.ForEach(ob =>
            {
                ob.LogStatus = logstatus.Where(i => Convert.ToInt32(i.Code) == Convert.ToInt32(ob.LogStatus)).FirstOrDefault().StateDesc;
            });

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
                    sql += " AND DepID IN @DepID AND (DepName IS NOT NULL OR DepName !='')";
                }
            }
            //一般查詢的方法
            sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " AND (A.PsnNo LIKE @PsnName OR A.PsnName LIKE @PsnName OR CardNo LIKE @PsnName OR B.IDNum=@IDNum) ";
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
            Response.AddHeader("content-disposition", "attachment; filename=0602.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }


        #region CreateDropDownList_LogStatusItem

        private void CreateDropDownList_LogStatusItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            List<string> liSqlPara = new List<string>();
            //this.DropDownList_LogStatus.Items.Clear();
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
                    //this.DropDownList_LogStatus.Items.Add(Item);
                }
            }
            else
            {
                //this.DropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
            //this.ADVDropDownList_LogStatus.Items.AddRange(this.DropDownList_LogStatus.Items.OfType<ListItem>().ToArray());

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
            wheresql += " (SysUserMgns.UserID = @UserID ) AND EquData.EquClass='TRT' ";
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
                    //PsnName = "TEST",
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
            var logs = this.odo.GetQueryResult<CardLogModel>("SELECT * FROM B01_CardLog WHERE RecordID=@RecordID", new { RecordID = RecordID });
            var LogPic = this.odo.GetQueryResult("SELECT * FROM B01_CardLogExt WHERE CardLogID=@RecordID", new { RecordID = RecordID });
            foreach (var o in LogPic)
            {
                logs.First().CardPicSource = Convert.ToString(o.CardPicSource);
            }
            var PsnDatas = this.odo.GetQueryResult<PersonEntity>("SELECT * FROM B01_Person WHERE PsnNo=@PsnNo", new { PsnNo = logs.First().PsnNo });
            foreach (var o in PsnDatas)
            {
                if (o.PsnPicSource != "")
                {
                    PsnPicSource = o.PsnPicSource;
                }
                else
                {
                    PsnPicSource = "/Img/Default.png";
                }               
            }
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
               new { message = "Error", card_log = logs.First(), error_msg = "", PsnPicSource = Url + PsnPicSource }));
            Response.End();
        }



        }//end page class
}//end namespace