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
    public partial class QueryStayZZ03 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<ClassRoomTable> ListLog = new List<ClassRoomTable>();
        public List<ClassRoomTable> ListLogOut = new List<ClassRoomTable>();

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
        
        public IPagedList<ClassRoomTable> PagedList;

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
            ClientScript.RegisterClientScriptInclude("ZZ03", "QueryStayZZ03.js?"+DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
            this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
            this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "Company", DataWidth = 124, TitleWidth = 120, TitleName = "公司" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = "寢室" });            
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 103, TitleWidth = 100,TitleName="姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 74, TitleWidth = 70,TitleName="卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardDate", DataWidth = 123, TitleWidth = 120, TitleName = "刷卡日期" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 123, TitleWidth = 120, TitleName = "刷卡時間" });            
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
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");           
            string sql = "";
            sql = @"SELECT DISTINCT
	                    B.*,Convert(DATETIME,InOutDate) AS CardDate,Org.OrgName AS DepName,InOutDate,InTime
                        FROM 
	                        B03_ClassRoomTable A
	                        INNER JOIN V_PsnCard B ON A.PsnNo=B.PsnNo
							LEFT JOIN OrgStrucAllData('Unit') AS Org ON B.OrgStrucID=Org.OrgStrucID
                        WHERE
	                        Convert(DATETIME,InOutDate) BETWEEN @CardDateS AND @CardDateE ORDER BY CardDate,CardNo";
            //廠方班表匯入紀錄
            this.ListLog = this.odo.GetQueryResult<ClassRoomTable>(sql,new { CardDateS = Request["CardDateS"], CardDateE = Request["CardDateE"]}).ToList();
            sql = @"SELECT DISTINCT A.CardNo,A.EquDir,EquNo,EquName
				,CONVERT(VARCHAR(10),A.CardTime,111) AS CardDate,B.PsnName
				,SUBSTRING(CONVERT(VARCHAR(50),A.CardTime,121),12,8) AS CardTime
				 FROM B01_CardLog A 
	                    INNER JOIN (
                    SELECT 
	                    CardNo,MAX(CardTime) AS CardTime,CONVERT(VARCHAR(10),CardTime,111) AS CardDate
                    FROM 
	                    B01_CardLog A
                        INNER JOIN B01_EquData E ON A.EquNo=E.EquNo WHERE EquDir='出'
                    GROUP BY CardNo,CONVERT(VARCHAR(10),CardTime,111)) B2 ON A.CardNo=B2.CardNo AND A.CardTime=B2.CardTime 
	                     INNER JOIN V_PsnCard B ON A.PsnNo=B.PsnNo 
					WHERE A.CardTime BETWEEN @CardDateS AND @CardDateE";
            string sql2 = sql.Replace("MAX(CardTime)", "MIN(CardTime)");
            //宿舍刷出記錄
            var newlogs = this.odo.GetQueryResult<ClassRoomTable>(sql, new { CardDateS = Request["CardDateS"], CardDateE = Request["CardDateE"],Dir="出" }).ToList();
            //var newlogs2 = this.odo.GetQueryResult<ClassRoomTable>(sql2, new { CardDateS = Request["CardDateS"], CardDateE = Request["CardDateE"], Dir = "進" });            
            this.ListLog.ForEach(i =>
            {
                i.CardTime = "";
                if (newlogs.Where(no => no.CardNo == i.CardNo && no.CardDate == i.InOutDate).Count() > 0)
                {
                    i.CardTime = newlogs.Where(no => no.CardNo == i.CardNo && no.CardDate == i.InOutDate).First().CardTime;                    
                }
            });
            //無刷出紀錄，有上班
            var result1 = this.ListLog.Where(i => i.InTime != null && i.CardTime == "").ToList();
            //有刷出，無上班
            var result2 = this.ListLog.Where(i => i.InTime == null && i.CardTime != "").ToList();
            //無刷出，無上班
            var result3 = this.ListLog.Where(i => i.InTime == null && i.CardTime == "").ToList();
            //this.ListLog = result1.Union(result2).Union(result3).ToList();
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog.OrderByField("CardDate",boolSort).ToList());
            }
            else
            {
                //轉datatable
                //this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                //this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog.OrderByField(SortName,boolSort).ToList());
            }                   
        }



        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");


            ws.Cells[1, 1].Value = "日期";
            ws.Cells[1, 2].Value = "人員編號";
            ws.Cells[1, 3].Value = "姓名";
            ws.Cells[1, 4].Value = "卡號";
            ws.Cells[1, 5].Value = "寢室";
            ws.Cells[1, 6].Value = "刷出時間";
            ws.Cells[1, 7].Value = "入廠時間";
            //ws.Cells[1, this.ListCols.Count].Value = "進出類型";
            //Content
            for (int i = 0; i < this.ListLog.Count; i++)
            {
                ws.Cells[i + 2,1].Value = this.ListLog[i].InOutDate;
                ws.Cells[i + 2, 2].Value = this.ListLog[i].PsnNo;
                ws.Cells[i + 2, 3].Value = this.ListLog[i].PsnName;
                ws.Cells[i + 2, 4].Value = this.ListLog[i].CardNo;
                ws.Cells[i + 2, 5].Value = this.ListLog[i].DepName;
                ws.Cells[i + 2, 6].Value = this.ListLog[i].CardTime;
                ws.Cells[i + 2, 7].Value = this.ListLog[i].InTime!=null?ListLog[i].InTime.Value.ToString("HH:mm:ss"):"";
            }
            //ws.Cells[this.DataResult.Rows.Count+2, 1, this.DataResult.Rows.Count+2, this.ListCols.Count].Merge = true;
            //ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = string.Format("未出人數：{0}，未進人數：{1}",this.ListLog.Where(i=>i.EquDir=="進").Count(),this.ListLog.Where(i=>i.EquDir=="出").Count());
            ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = "總筆數";
            ws.Cells[this.DataResult.Rows.Count + 2, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=ZZ03.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.ClassRoomTable()
                {
                    //PsnName = "TEST",
                    CardTime = DateTime.Now.ToString("HH:mm:ss"),
                    CardDate = DateTime.Now.ToString("yyyy/MM/dd")
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }
        


    }//end page class
}//end namespace