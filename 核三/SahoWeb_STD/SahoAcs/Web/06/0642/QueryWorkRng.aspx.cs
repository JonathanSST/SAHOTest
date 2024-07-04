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
    public partial class QueryWorkRng : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<StayCardLog> ListLog = new List<StayCardLog>();
        public List<StayCardLog> ListLogOut = new List<StayCardLog>();

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
        
        public IPagedList<StayCardLog> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PickDateS.DateValue = this.GetZoneTime().ToString("yyyy/MM") + "/01";
            this.PickDateE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
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
            ClientScript.RegisterClientScriptInclude("ZZ01", "QueryWorkRng.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
            //this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
            //this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
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
            string CardDateS = Request["ctl00$ContentPlaceHolder1$PickDateS$CalendarTextBox"];
            string CardDateE = Request["ctl00$ContentPlaceHolder1$PickDateE$CalendarTextBox"];
            //string CardMonth = Request["QueryMonth"].ToString();
            //DateTime start = DateTime.Parse(CardMonth + "/01");
            //DateTime end = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));
            if (Request["SortName"] != null)
            {
                this.SortName = Request["SortName"];
                this.SortType = Request["SortType"];
            }
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }            
            string sql = "";
            sql = @"SELECT * FROM (
                        SELECT PsnNo,PsnName,CardNo,MIN(CardTime) AS CardTimeS,MAX(CardTime) AS CardTimeE,CardDate,OrgNo,OrgName                             
							,CASE WHEN MIN(CardTime)<'11:00:00' THEN MIN(CardTime) ELSE '' END AS RealTimeS
							,CASE WHEN MAX(CardTime)<'12:00:00' THEN '' ELSE MAX(CardTime) END AS RealTimeE
                            FROM (
                            SELECT 
	                            P.PsnNo,
	                            P.PsnName,
	                            P.CardNo,
								D.OrgName,OrgNo,
	                            CONVERT(VARCHAR(10),C.CardTime,111) AS 'CardDate',
	                            CONVERT(VARCHAR(10),C.CardTime,108) AS CardTime
                            FROM 			
	                            V_PsnCard P
								INNER JOIN OrgStrucAllData('Department') D ON P.OrgStrucID=D.OrgStrucID AND P.PsnAuthAllow=1
	                            INNER JOIN FillCardLog C ON C.EmpID=P.PsnNo AND CardTime BETWEEN @CardDateS AND @CardDateE
								UNION 
								SELECT 
	                            P.PsnNo,
	                            P.PsnName,
	                            P.CardNo,
								D.OrgName,OrgNo,
	                            CONVERT(VARCHAR(10),C.CardTime,111) AS 'CardDate',
	                            CONVERT(VARCHAR(10),C.CardTime,108) AS CardTime
                            FROM 			
	                            V_PsnCard P
								INNER JOIN OrgStrucAllData('Department') D ON P.OrgStrucID=D.OrgStrucID AND P.PsnAuthAllow=1
	                            INNER JOIN B01_CardLog C ON C.PsnNo=P.PsnNo AND CardTime BETWEEN @CardDateS AND @CardDateE AND EquClass='TRT'
                            ) AS CardLog WHERE PsnName NOT LIKE '臨時卡%'					
                            GROUP BY PsnNo,PsnName,CardNo,CardDate,OrgNo,OrgName) AS ResultAll 
                WHERE (RealTimeS>='08:31:00' OR RealTimeS<='07:30:00' OR RealTimeS='' OR RealTimeE='' OR RealTimeE>='18:26:00') ORDER BY OrgNo,PsnNo";
            this.ListLog = this.odo.GetQueryResult<StayCardLog>(sql,new { CardDateS = CardDateS+" 00:00:00", CardDateE =CardDateE +" 23:59:59" }).ToList();
            //2021/01/20 增加星期假日控制
            var EmptyCardLog = this.odo.GetQueryResult<StayCardLog>(@"SELECT DISTINCT
	                P.PsnNo,
	                P.PsnName,
	                P.CardNo,OrgNo,OrgName,WorkDay AS CardDate,C.PsnNo,'' AS RealTimeS,'' AS RealTimeE
                FROM 			
	                V_PsnCard P
	                INNER JOIN OrgStrucAllData('Department') O ON P.OrgStrucID=O.OrgStrucID	AND P.PsnAuthAllow=1
	                INNER JOIN (SELECT DISTINCT CONVERT(VARCHAR(10),CardTime,111) AS WorkDay FROM FillCardLog WHERE CardTime BETWEEN @CardDateS AND @CardDateE 
                    UNION
					SELECT DISTINCT CONVERT(VARCHAR(10),CardTime,111) AS WorkDay FROM B01_CardLog WHERE CardTime BETWEEN @CardDateS AND @CardDateE) AS WorkList ON 1=1
	                LEFT JOIN 
	                (SELECT PsnNo,CardTime FROM B01_CardLog WHERE EquClass='TRT' UNION SELECT EmpID,CardTime FROM FillCardLog) AS C ON WorkDay=CONVERT(VARCHAR(10),CardTime,111) AND P.PsnNo=C.PsnNo	
                WHERE C.PsnNo IS NULL AND P.PsnName NOT LIKE '臨時卡%' AND  (DatePart(WeekDay,CONVERT(DATETIME,WorkDay)) IN (6,2,3,4,5))
                ORDER BY OrgNo,OrgName,WorkDay", new { CardDateS = CardDateS + " 00:00:00", CardDateE = CardDateE + " 23:59:59" }).ToList();
            //this.ListLog = this.ListLog.Concat(this.ListLogOut).OrderByField(SortName, boolSort).ToList();
            var OrgPsnList = this.odo.GetQueryResult(@"SELECT P.*,O.OrgNameList FROM V_PsnCard P INNER JOIN OrgStrucAllData('') O ON P.OrgStrucID=O.OrgStrucID 
                WHERE (OrgNameList LIKE '%經理%' OR OrgNameList LIKE '%協理%' OR OrgNameList LIKE '%副總%') ").ToList();                                    
            this.ListLog = this.ListLog.Union(EmptyCardLog).OrderBy(i => i.CardDate).OrderBy(i => i.OrgName).OrderBy(i => i.OrgNo).ToList().ToList();
            this.ListLog = this.ListLog.Where(i => !OrgPsnList.Select(o => Convert.ToString(o.PsnNo)).Contains(i.PsnNo)).OrderBy(i => i.CardDate).OrderBy(i => i.OrgName).OrderBy(i => i.OrgNo).ToList().ToList();
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                //轉datatable
                //this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                
            }                   
        }


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
            //string CardMonth = Request["QueryMonth"].ToString();
            //DateTime now = DateTime.Parse(CardMonth+"/01");
            ws.Cells[1, 1, 1, 10].Merge = true;            
            ws.Cells[1, 1].Value = string.Format("{0}~{1} 刷卡異常管理明細表",Request["ctl00$ContentPlaceHolder1$PickDateS$CalendarTextBox"],Request["ctl00$ContentPlaceHolder1$PickDateE$CalendarTextBox"]);
            ws.Cells[1, 1].Style.Font.Size = 20;//字体大小
            ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.WrapText = true;
            ws.Cells[2, 1].Value = "部門";
            ws.Cells[2, 2].Value = "部門名稱";
            ws.Cells[2, 3].Value = "工號";
            ws.Cells[2, 4].Value = "姓名";
            ws.Cells[2, 5].Value = "日期";
            ws.Cells[2, 6].Value = "上班";
            ws.Cells[2, 7].Value = "下班";
            ws.Cells[2, 8].Value = "原因";
            ws.Cells[2, 9].Value = "簽名";
            ws.Cells[2, 10].Value = "備註";
            //Content
            int RowIndex = 1;
            for (int i = 1; i < this.ListLog.Count; i++)
            {
                var o = this.ListLog[i];
                ws.Cells[i + 2, 1].Value = this.ListLog[i].OrgNo;
                ws.Cells[i + 2, 2].Value = this.ListLog[i].OrgName;
                ws.Cells[i + 2, 3].Value = this.ListLog[i].PsnNo;
                ws.Cells[i + 2, 4].Value = this.ListLog[i].PsnName;
                ws.Cells[i + 2, 5].Value = this.ListLog[i].CardDate;
                ws.Cells[i + 2, 6].Value = this.ListLog[i].RealTimeS;
                ws.Cells[i + 2, 7].Value = this.ListLog[i].RealTimeE;
                ws.Cells[i + 2, 8].Value = "";
                ws.Cells[i + 2, 9].Value = "";
               
                string Desc = "";
                if (o.RealTimeS == string.Empty || o.RealTimeE == string.Empty)
                {                    
                    Desc = "未打卡";
                }
                else if (o.RealTimeS.CompareTo("08:31:01") >= 0)
                {
                    Desc = "遲到";
                }
                else if (o.RealTimeE.CompareTo("18:26:01") >= 0)
                {
                    Desc = "下班時間超時";
                }
                else if (o.RealTimeS.CompareTo("07:30:00") <= 0)
                {
                    Desc = "提早上班打卡";
                }
                this.ListLog[i].LogStatus = Desc;
                ws.Cells[i + 2, 10].Value = Desc;
                RowIndex = i + 3;
            }
            var groupresult = from g in this.ListLog where g.LogStatus=="未打卡"
                              group g by new { g.PsnNo, g.PsnName } into groups
                              select new StayCardLog { PsnNo = groups.Key.PsnNo, PsnName = groups.Key.PsnName, TotalIn = groups.Count() };
            var groupresult2 = from g in this.ListLog
                              where g.LogStatus == "遲到"
                              group g by new { g.PsnNo, g.PsnName } into groups
                              select new StayCardLog { PsnNo = groups.Key.PsnNo, PsnName = groups.Key.PsnName, TotalIn = groups.Count() };
            ws.Cells[RowIndex, 1].Value = "未打卡統計";
            RowIndex++;
            foreach(var o in groupresult)
            {                
                ws.Cells[RowIndex, 1].Value = o.PsnNo;
                ws.Cells[RowIndex, 2].Value = o.PsnName;
                ws.Cells[RowIndex, 3].Value = o.TotalIn;
                RowIndex++;
            }
            RowIndex++;
            ws.Cells[RowIndex, 1].Value = "遲到統計";
            foreach (var o in groupresult2)
            {
                ws.Cells[RowIndex, 1].Value = o.PsnNo;
                ws.Cells[RowIndex, 2].Value = o.PsnName;
                ws.Cells[RowIndex, 3].Value = o.TotalIn;
                RowIndex++;
            }
            //var GroupData = this.ListLog.Where(i=>i.LogStatus=="未打卡").GroupBy(i=>i.PsnNo,i.)
            //ws.Cells[this.DataResult.Rows.Count+2, 1, this.DataResult.Rows.Count+2, this.ListCols.Count].Merge = true;
            //ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = string.Format("未出人數：{0}，未進人數：{1}",this.ListLog.Where(i=>i.EquDir=="進").Count(),this.ListLog.Where(i=>i.EquDir=="出").Count());            
            ws.Cells.AutoFitColumns(); //自動欄寬
            ws.Row(1).Height = 22;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=RngData.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.StayCardLog()
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