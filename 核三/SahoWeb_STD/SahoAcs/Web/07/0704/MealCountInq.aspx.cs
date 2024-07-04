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

namespace SahoAcs.Web._07._0704
{
    public partial class MealCountInq : System.Web.UI.Page
    {
        #region Global block
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string SortName = "ProcTime";
        public string SortType = "ASC";
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<B03_MealOrder> ListLog = new List<B03_MealOrder>();

        public int PageIndex = 1;
        public string PsnNo = "", PsnID = "", PsnName = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string AuthList = "";
        public IPagedList<B03_MealOrder> PagedList;
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
                this.Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                this.Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
            }
            ClientScript.RegisterClientScriptInclude("JsInclude1", "MealCountInq.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        private void SetQueryData()
        {
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");

            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string CardTimeSDate = Request["CardTimeSDate"];
            string CardTimeEDate = Request["CardTimeEDate"];
            string Sql = string.Empty;

            Sql = @" Select
            MO.PsnNo, MO.PsnName, COUNT(MO.CardNo) AS MealCount
            From B03_MealOrder MO
            --INNER JOIN B01_Card C ON MO.CardNo = C.CardNo
            --INNER JOIN B01_Person P ON P.PsnID = C.PsnID
            Where 1=1
            ";
            Sql += " And convert(varchar, MO.MealDate, 120) Between '" + CardTimeSDate + "' And '" + CardTimeEDate + "'";
            Sql += " And MO.OrderSrc in ('1','5') And MO.[Status] IN (1,3) ";
            Sql += " GROUP BY MO.PsnNo, MO.PsnName,MO.CardNo ";

            this.ListLog = this.odo.GetQueryResult<B03_MealOrder>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                UserID = UserID
            }).OrderByField("MealDate", true).ToList();

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
            string Sql = string.Empty;

            Sql = @" Select
            MO.PsnNo, MO.PsnName, COUNT(MO.CardNo) AS MealCount
            From B03_MealOrder MO
            --INNER JOIN B01_Card C ON MO.CardNo = C.CardNo
            --INNER JOIN B01_Person P ON P.PsnID = C.PsnID
            Where 1=1
            ";
            Sql += " And convert(varchar, MO.MealDate, 120) Between '" + CardTimeSDate + "' And '" + CardTimeEDate + "'";
            Sql += " And MO.OrderSrc in ('1','5') And MO.[Status] IN (1,3) ";
            Sql += " GROUP BY MO.PsnNo, MO.PsnName,MO.CardNo ";

            this.ListLog = this.odo.GetQueryResult<B03_MealOrder>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                UserID = UserID
            }).OrderByField("MealDate", true).ToList();

            if (this.ListLog.Count != 0)
            {
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("廠商統計報表");
                ws.Cells[1, 1].Value = "員工編號";
                ws.Cells[1, 2].Value = "姓名";
                ws.Cells[1, 3].Value = "用餐次數";
                

                int count = 2;
                foreach (var o in this.ListLog)
                {
                    ws.Cells[count, 1].Value = o.PsnNo;
                    ws.Cells[count, 2].Value = o.PsnName;
                    ws.Cells[count, 3].Value = o.MealCount;
                    count++;
                }

                ws.Cells.AutoFitColumns(); //自動欄寬
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=廠商統計報表.xls");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }

        }
    }
}