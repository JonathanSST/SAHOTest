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
    public partial class QueryOpenDoorZZ : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<SysLogEntity> ListLog = new List<SysLogEntity>();
        public List<StayCardLog> ListLogOut = new List<StayCardLog>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "LogTime";
        public string SortType = "DESC";
        string sql = "";  //宿舍進出紀錄的查詢
        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        
        public IPagedList<SysLogEntity> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                //this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "QueryOneLog")
            {
                //this.SetOneCardLog();
            }
            else
            {
                this.SetInitData();
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            if (!IsPostBack)
            {
               
            }
            ClientScript.RegisterClientScriptInclude("ZZ01", "QueryOpenDoorZZ.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(LogTime) FROM B00_SysLog WHERE LogTime <= GETDATE()");
            this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
            this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "Company", DataWidth = 124, TitleWidth = 120, TitleName = "公司" });
            this.ListCols.Add(new ColNameObj() { ColName = "UserID", DataWidth = 104, TitleWidth = 100, TitleName = "使用者帳號" });            
            this.ListCols.Add(new ColNameObj() { ColName = "UserName", DataWidth = 124, TitleWidth = 120,TitleName="使用者名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 124, TitleWidth = 120, TitleName = "設備編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogTimeVal",DataRealName="LogTime", DataWidth = 124, TitleWidth = 120,TitleName="紀錄時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogType", DataWidth = 124, TitleWidth = 120, TitleName = "紀錄類型" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogInfo", DataWidth = 124, TitleWidth = 120, TitleName = "紀錄資訊" });            
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            
            #endregion
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
            
            foreach (var s in Request.Form.GetValues("ColName"))
            {
                collist.Add(this.ListCols.Where(i => i.ColName == s).FirstOrDefault());
            }
            this.ListCols = collist;
          
            //增加以日進行搜尋，並再列出進出別的條件  2020/11/13
            sql = @"SELECT  * FROM B00_SysLog WHERE LogType='指令傳送' AND LogTime BETWEEN @TimeS AND @TimeE ORDER BY LogTime DESC";
                         
            //取得在宿與非在宿的紀錄
            this.ListLog = this.odo.GetQueryResult<SysLogEntity>(sql,new { TimeS = Request["CardDateS"], TimeE = Request["CardDateE"] }).ToList();
            
            bool boolSort = this.SortType.Equals("ASC");
            this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog.OrderByField(SortName, boolSort));
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));
            foreach (DataRow r in this.DataResult.Rows)
            {                
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", r["LogTime"]);                
            }
        }

        /// <summary>加入查詢條件</summary>
        /// <returns></returns>
        private string GetConditionCmdStr()
        {
            string sql = "";                       
            //一般查詢的方法
            sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
            }
            if (Request["MgaName"] != null && Request["MgaName"] != "")
            {
                sql += " AND MgaName = @MgaName ";
            }
            if (Request["EquNo"] != null && Request["EquNo"] != "")
            {
                sql += " AND EquNo = @EquNo ";
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
            //ws.Cells[this.DataResult.Rows.Count+2, 1, this.DataResult.Rows.Count+2, this.ListCols.Count].Merge = true;
            //ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = string.Format("未出人數：{0}，未進人數：{1}",this.ListLog.Where(i=>i.EquDir=="進").Count(),this.ListLog.Where(i=>i.EquDir=="出").Count());
            ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = "總筆數";
            ws.Cells[this.DataResult.Rows.Count + 2, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=Stay01.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {
            for (int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.SysLogEntity()
                {
                    //PsnName = "TEST",
                    LogTime = DateTime.Now
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }
        


    }//end page class
}//end namespace