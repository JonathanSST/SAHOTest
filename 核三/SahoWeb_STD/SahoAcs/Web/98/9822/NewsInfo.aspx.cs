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
    public partial class NewsInfo : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<NewsInfoEntity> ListLog = new List<NewsInfoEntity>();
        public City city = new City();
        public List<City> Cities = new List<City>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();        

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string PsnID = "";

        public string SortName = "NewsTitle";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        
        public IPagedList<NewsInfoEntity> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.SetInitData();
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {                
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                //this.SetInitData();
                //this.SetQueryData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                //this.SetInitData();
                this.SetDefaultData();
                //this.EmptyCondition();
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            }
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel();";
            js = "<script type='text/javascript'>" + js + "</script>";       
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("NewsInfo", "NewsInfo.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            //ClientScript.RegisterClientScriptInclude("autolocation", "/Scripts/autolocation.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion


            //this.Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            this.Calendar_CardTimeSDate.DateValue = DateTime.Now.AddMonths(-3).ToString("yyyy/MM/dd");
            this.Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
           
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


        private void SetDefaultData()
        {
            this.Calendar_CardTimeSDate.DateValue = DateTime.Now.AddMonths(-3).ToString("yyyy/MM/dd");
            this.Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            this.ListLog = this.odo.GetQueryResult<NewsInfoEntity>(@"SELECT * FROM B01_NewsInfo WHERE CONVERT(VARCHAR(10),NewsDate,111) BETWEEN @DateS AND @DateE ORDER BY CreateTime DESC"
                    , new {DateS=this.Calendar_CardTimeSDate.DateValue, DateE=this.Calendar_CardTimeEDate.DateValue}).ToList();
            this.PagedList = this.ListLog.OrderByField(SortName, true).ToPagedList(PageIndex, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
        }


        private void SetQueryData()
        {            
            this.ListLog = this.odo.GetQueryResult<NewsInfoEntity>(@"SELECT * FROM B01_NewsInfo WHERE CONVERT(VARCHAR(10),NewsDate,111) BETWEEN @DateS AND @DateE ORDER BY CreateTime DESC"
                    , new { DateS = Request["DateS"], DateE = Request["DateE"] }).ToList();
            this.PagedList = this.ListLog.OrderByField(SortName, true).ToPagedList(PageIndex, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
        }
        
        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");
            ws.Cells[1, 1].Value = "公告日期";
            ws.Cells[1, 2].Value = "主旨";
            ws.Cells[1, 3].Value = "內容";
            //ws.Cells[1, 4].Value = "公告日期";
            int count = 2;
            foreach(var o in this.ListLog)
            {
                ws.Cells[count, 1].Value = string.Format("{0:yyyy/MM/dd}", o.NewsDate);
                ws.Cells[count, 2].Value = o.NewsTitle;
                ws.Cells[count, 3].Value = o.NewsContent;
                count++;
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=CourseManage.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void EmptyCondition()
        {
            
        }

    }//end page class
}//end namespace