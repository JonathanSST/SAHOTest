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
    public partial class QueryGuestData1 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> ListLog = new List<CardLogModel>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<OrgDataEntity> OrgDataList = new List<OrgDataEntity>();

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
            ClientScript.RegisterClientScriptInclude("0621", "QueryGuestData1.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
                this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
            }
            else
            {
                this.CalendarS.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                this.CalendarE.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
            }
            #endregion

            this.OrgDataList = this.odo.GetQueryResult<OrgDataEntity>("SELECT * FROM B01_OrgData WHERE OrgClass='Company' ").ToList();
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
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");         
            string sql = "";            
            sql = @"SELECT A.*,B.PsnEName FROM B01_CardLog A INNER JOIN B01_Person B ON A.PsnNo=B.PsnNo WHERE CardTime BETWEEN @CardTimeS AND @CardTimeE 
                        AND LogStatus=0 AND (A.PsnNo LIKE @PsnName OR A.PsnName LIKE @PsnName) ORDER BY CardTime DESC";            
            this.ListLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                CardTimeS = Request["CardTimeS"],
                CardTimeE = Request["CardTimeE"],
                PsnName = "%"+ Request["PsnName"]+"%"
            }).ToList();
            
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                //轉datatable
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                //this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }             
        }
        

        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");
            
            ws.Cells[1, 1].Value = "業戶識別碼";
            ws.Cells[1, 2].Value = "業戶名稱";
            ws.Cells[1, 3].Value = "業戶編號";
            ws.Cells[1, 4].Value = "卡號";
            ws.Cells[1, 5].Value = "讀卡時間";
            ws.Cells[1, 6].Value = "設備編號";
            ws.Cells[1, 7].Value = "設備名稱";
            //Content
            for (int i = 0; i < this.ListLog.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = this.ListLog[i].PsnEName;
                ws.Cells[i + 2, 2].Value = this.ListLog[i].PsnName;
                ws.Cells[i + 2, 3].Value = this.ListLog[i].PsnNo;
                ws.Cells[i + 2, 4].Value = this.ListLog[i].CardNo;
                ws.Cells[i + 2, 5].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.ListLog[i].CardTime);
                ws.Cells[i + 2, 6].Value = this.ListLog[i].EquNo;
                ws.Cells[i + 2, 7].Value = this.ListLog[i].EquName;
            }
            ws.Cells[this.ListLog.Count + 2, 1].Value = "總筆數：";
            ws.Cells[this.ListLog.Count + 2, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryGuestData.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }
        


    }//end page class
}//end namespace