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
    public partial class CardLogFillManage : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CourseEntity> ListLog = new List<CourseEntity>();
        public City city = new City();
        public List<City> Cities = new List<City>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();        

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string PsnID = "";

        public string SortName = "CourseName";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        
        public IPagedList<CourseEntity> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetInitData();
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                //this.SetInitData();
                this.SetQueryData();
                //this.ExportExcel();
            }
            else
            {
                //this.SetInitData();
                //this.EmptyCondition();
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            }
            ClientScript.RegisterClientScriptInclude("CardLogFillManage", "CardLogFillManage.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            //ClientScript.RegisterClientScriptInclude("autolocation", "/Scripts/autolocation.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion


            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 124, TitleWidth = 120, TitleName = "人員姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 124, TitleWidth = 120, TitleName = "工號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 124, TitleWidth = 120, TitleName = "卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseName", DataWidth = 124, TitleWidth = 120, TitleName = "課程名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataRealName = "CardTime", DataWidth = 134, TitleWidth = 130, TitleName = "刷卡時間" });            
            this.ListCols.Add(new ColNameObj() { ColName = "CourseType", DataWidth = 104, TitleWidth = 100, TitleName = "課程類別" });            
            this.ListCols.Add(new ColNameObj() { ColName = "Season", DataWidth = 104, TitleWidth = 100, TitleName = "期別" });
            this.ListCols.Add(new ColNameObj() { ColName = "City", DataWidth = 104, TitleWidth = 100, TitleName = "上課縣市" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseType", DataWidth = 104, TitleWidth = 100, TitleName = "學習性質" });
            this.Cities = this.city.GetCities();
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            this.Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
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
            string sql = @"SELECT  C.*,A.PsnNo,A.PsnName,A.CardNo, CONVERT(NVARCHAR(19),CardTime,121) AS CardTime
                                    FROM 
	                                    B01_CardLog A 
	                                    INNER JOIN B01_EquData B ON A.EquNo=B.EquNo
	                                    INNER JOIN B03_Course C ON B.EquID=C.EquID 
	                                    AND A.CardTime BETWEEN CourseTimeS AND CourseTimeE WHERE CourseTimeS BETWEEN @DateS AND @DateE AND (PsnNo LIKE @PsnNo OR PsnName LIKE @PsnNo)";
            this.ListLog = this.odo.GetQueryResult<CourseEntity>(sql, new { DateS = Request["DateS"], DateE = Request["DateE"],PsnNo=Request["PsnNo"]+"%" }).ToList();
            this.ListLog.ForEach(i => i.City = this.Cities.Where(c => c.CityVal == i.City).First().CityName);
            if (Request["SortName"] != null)
            {
                this.SortName = Request["SortName"];
                this.SortType = Request["SortType"];
            }
            bool boolSort = this.SortType.Equals("ASC");
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog.OrderByField(SortName, boolSort));
            }
            else
            {
                //轉datatable
                this.PagedList = this.ListLog.OrderByField(SortName,boolSort).ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
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
            Response.AddHeader("content-disposition", "attachment; filename=CourseManage.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.CourseEntity()
                {
                    //PsnName = "TEST",
                    CourseTimeS = DateTime.Now,
                    CourseTimeE = DateTime.Now.AddHours(3),
                    CourseRealTimeS = DateTime.Now,
                    CourseRealTimeE = DateTime.Now.AddHours(3)
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
            //this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
            //this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));            
            foreach (DataRow r in this.DataResult.Rows)
            {
                //r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));
                //r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LogTime"]));
            }
        }

    }//end page class
}//end namespace