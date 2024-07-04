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
    public partial class QueryCourseLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CourseEntity> ListCourseLog = new List<CourseEntity>();
        public City city = new City();
        public List<City> Cities = new List<City>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<CourseEntity> ListCourse = new List<CourseEntity>();
        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "PsnNo";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        
        public IPagedList<CourseEntity> PagedList;

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
            ClientScript.RegisterClientScriptInclude("0643", "QueryCourseLog.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));            
            //this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            //this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "Company", DataWidth = 124, TitleWidth = 120, TitleName = "公司" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "身份證號" });           
            this.ListCols.Add(new ColNameObj() { ColName = "CourseName", DataWidth = 124, TitleWidth = 120, TitleName = "課程名稱" });            
            this.ListCols.Add(new ColNameObj() { ColName = "CourseTs", DataRealName = "CourseTs", DataWidth = 144, TitleWidth = 140, TitleName = "開課起始時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseTe", DataRealName = "CourseTe", DataWidth = 144, TitleWidth = 140, TitleName = "開課結束時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 84, TitleWidth = 80, TitleName = "姓名"});
            this.ListCols.Add(new ColNameObj() { ColName = "CourseScore", DataWidth = 84, TitleWidth = 80, TitleName = "學位學分" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseType", DataWidth = 84, TitleWidth = 80, TitleName = "課程類別" });
            this.ListCols.Add(new ColNameObj() { ColName = "City", DataWidth = 74, TitleWidth = 70, TitleName = "上課縣市" });
            this.ListCols.Add(new ColNameObj() { ColName = "Season", DataWidth = 74, TitleWidth = 70, TitleName = "期別" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseAmount", DataWidth = 74, TitleWidth = 70, TitleName = "訓練總數" });
            this.ListCols.Add(new ColNameObj() { ColName = "TrainScore", DataWidth = 0, TitleWidth = 0, TitleName = "訓練成績" });
            this.ListCols.Add(new ColNameObj() { ColName = "LicenceNo", DataWidth = 0, TitleWidth = 0, TitleName = "證件字號" });
            this.ListCols.Add(new ColNameObj() { ColName = "TrainStatu", DataWidth = 0, TitleWidth = 0, TitleName = "出勤上課狀況" });
            this.ListCols.Add(new ColNameObj() { ColName = "BirthDay", DataWidth = 0, TitleWidth = 0, TitleName = "生日" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseUnit", DataWidth = 104, TitleWidth = 100, TitleName = "訓練總數單位" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseProp", DataWidth = 84, TitleWidth = 80, TitleName = "學習性質" });
            this.ListCols.Add(new ColNameObj() { ColName = "DigitHour", DataWidth = 74, TitleWidth = 70, TitleName = "數位時數" });
            this.ListCols.Add(new ColNameObj() { ColName = "RealHour", DataWidth = 74, TitleWidth = 70, TitleName = "實體時數" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseNo", DataWidth = 0, TitleWidth = 0, TitleName = "課程代碼" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseRealTimeS", DataWidth = 0, TitleWidth = 0, TitleName = "實際上課起始時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CourseRealTimeE", DataWidth = 0, TitleWidth = 0, TitleName = "實際上課結束時間" });
            
            this.Cities = this.city.GetCities();
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            //設定課程名稱
            this.ListCourse = this.odo.GetQueryResult<CourseEntity>(@"SELECT CourseNo,CourseName,CourseID,City,CourseType,Season,B.EquID
                    ,B.EquName,B.EquNo,CourseUnit,RealHour,DigitHour,CourseScore,CourseAmount,CourseProp 
                FROM B03_Course A
                    INNER JOIN B01_EquData B ON A.EquID=B.EquID ORDER BY CourseTimeS DESC").ToList();
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
            //List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");            
            string sql = @"SELECT  A.PsnNo,A.PsnName,A.CardNo, CONVERT(NVARCHAR(19),CardTime,121) AS CardTime
                                                ,CourseNo,CourseName,CourseID
		                            ,CONVERT(NVARCHAR(19),CourseTimeS,121) AS CourseTs , CONVERT(NVARCHAR(19),CourseTimeE,121) AS CourseTe
		                            ,CONVERT(NVARCHAR(19),CourseRealTimeS,121) AS CourseRealTimeS,CONVERT(NVARCHAR(19),CourseRealTimeE,121) AS CourseRealTimeE
		                            ,City,CourseType,Season,CourseUnit,RealHour,DigitHour,CourseScore,CourseAmount,CourseProp
                                    ,0 AS TrainScore,'' AS LicenceNo,'' AS TrainStatu,'' AS BirthDay
                                    FROM 
	                                    B01_CardLog A 
	                                    INNER JOIN B01_EquData B ON A.EquNo=B.EquNo
	                                    INNER JOIN B03_Course C ON B.EquID=C.EquID 
	                                    AND A.CardTime BETWEEN CourseTimeS AND CourseTimeE 
                                    WHERE CourseID=@CourseID";
            //this.ListModify = this.odo.GetQueryResult<PersonEntity>(sql, new {DateS= Request["CardDateS"]+" 00:00:00", DateE= Request["CardDateE"]+" 23:59:59", }).ToList();
            this.ListCourseLog = this.odo.GetQueryResult<CourseEntity>(sql, new { CourseID=Request["CourseName"]}).ToList();
            this.ListCourseLog.ForEach(i => i.City = this.Cities.Where(c => c.CityVal == i.City).First().CityName);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.ListCourseLog.OrderByField(SortName, boolSort).ToList());
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
            ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = "合計人數";
            ws.Cells[this.DataResult.Rows.Count + 2, 2].Value = this.ListCourseLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=CourseLogRecord.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListCourseLog.Add(new DBModel.CourseEntity()
                {
                    //PsnName = "TEST",
                    CourseTimeS = DateTime.Now
                });
            }
            //轉datatable
            this.PagedList = this.ListCourseLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }
        


    }//end page class
}//end namespace