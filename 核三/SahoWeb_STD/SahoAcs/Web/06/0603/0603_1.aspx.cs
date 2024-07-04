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
    public partial class _0603_1 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogTrt> ListLog = new List<CardLogTrt>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string PsnID = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        
        public IPagedList<CardLogTrt> PagedList;

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
                //this.EmptyCondition();
                this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            }
            ClientScript.RegisterClientScriptInclude("0603", "0603_1.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE() AND EquClass='TRT' ");

            if (dtLastCardTime == DateTime.MinValue)
            {
                Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");                
            }
            else
            {
                Calendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd");
                Calendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd");
            }
            #endregion

            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttResult").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeVal", DataRealName = "CardTime", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttCardTime").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "First", DataWidth = 74, TitleWidth = 70, TitleName = GetLocalResourceObject("ttFirst").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "Last", DataWidth = 74, TitleWidth = 70, TitleName = GetLocalResourceObject("ttLast").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 94, TitleWidth = 90, TitleName = GetLocalResourceObject("ttCardNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = GetLocalResourceObject("ttDeptName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80, TitleName = GetLocalResourceObject("ttPsnNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 154, TitleWidth = 150, TitleName = GetLocalResourceObject("ttPsnName").ToString() });
                                                
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            CreateDeptDropItem();
        }

        private void CreateDeptDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            #endregion

            #region Process String
            sql = @"SELECT OrgID, (OrgName + '(' + OrgNo + ')') AS 'OrgName' FROM
                (SELECT B00_SysUserMgns.UserID, OrgStrucAllData.OrgID, OrgStrucAllData.OrgNo,
                 OrgStrucAllData.OrgName FROM B00_SysUserMgns
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_Person ON B01_MgnOrgStrucs.OrgStrucID = B01_Person.OrgStrucID
                LEFT OUTER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON B01_Person.OrgStrucID = OrgStrucAllData.OrgStrucID
                ) AS Mgns
                WHERE Mgns.UserID = @UserID GROUP BY OrgID, (OrgName + '(' + OrgNo + ')')";
            #endregion

            var ddlResult = this.odo.GetQueryResult(sql,new {UserID = hideUserID.Value });

            this.ddlDept.DataSource = ddlResult;
            this.ddlDept.DataTextField = "OrgName";
            this.ddlDept.DataValueField = "OrgID";
            this.ddlDept.Items.Insert(0, Item);
            foreach(var o in ddlResult)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = Convert.ToString(o.OrgName);
                Item.Value = Convert.ToString(o.OrgID);
                this.ddlDept.Items.Add(Item);
            }
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Item = new System.Web.UI.WebControls.ListItem();
            //    Item.Text = dr["OrgName"].ToString();
            //    Item.Value = dr["OrgID"].ToString();
            //    this.ddlDept.Items.Add(Item);
            //}
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
            string sql  = @"SELECT 
                CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) AS 'CardTime',                				
                B01_CardLog.CardNo, 				
                B01_CardLog.PsnNo, 
				B01_CardLog.PsnName,DepID,DepName				
				,MIN(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'First'
                ,MAX(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'Last'
				FROM B01_CardLog ";
            //this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i=>i.EquNo).ToList();            
            string sqlwhere = " WHERE  CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) BETWEEN @CardTimeS AND @CardTimeE  AND (EquClass='TRT' OR IsAndTrt='1') ";           
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            if (Request["PsnNo"] != "")
            {
                sqlwhere += " AND PsnNo LIKE @PsnNo";
            }
            if (Request["CardNo"] != "")
            {
                sqlwhere += " AND CardNo LIKE @CardNo";
            }
            if (Request["DepID"] != "")
            {
                sqlwhere += " AND DepID = @DepID";
            }
            if (Request["PsnID"] != "")
            {
                sqlwhere += " AND PsnNo IN (SELECT PsnNo FROM B01_Person WHERE PsnID=@PsnID)";
            }
            sql += sqlwhere;
            sql += @" GROUP BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,111),CardNo,PsnNo,PsnName,DepID,DepName ";
            sql = "SELECT R1.*,LogStatus FROM (" + sql + ") AS R1 ";
            sql += @"INNER JOIN B01_CardLog C ON R1.CardNo=C.CardNo 
                        AND R1.Last=SUBSTRING(CONVERT(VARCHAR(50),C.CardTime,121),12,8)
                        AND R1.CardTime=CONVERT(VARCHAR(10),C.CardTime,111)";
            this.ListLog = this.odo.GetQueryResult<CardLogTrt>(sql, new
            {
                EquList = EquDatas,
                CardNo = "%" + Request["CardNo"] + "%",
                CardTimeS = Request["CardTimeS"],
                CardTimeE = Request["CardTimeE"],                               
                PsnNo ="%"+Request["PsnNo"]+"%",                
                DepID = Request["DepID"],
                PsnID=Request["PsnID"]
            }).OrderByField(this.SortName,boolSort).ToList();            

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
           
            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(r["CardTime"]));
                //r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LogTime"]));
                r["LogStatus"] = logstatus.Where(i => Convert.ToInt32(i.Code) == Convert.ToInt32(r["LogStatus"])).FirstOrDefault().StateDesc;
            }
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
            Response.AddHeader("content-disposition", "attachment; filename=0601.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.CardLogTrt()
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

    }//end page class
}//end namespace