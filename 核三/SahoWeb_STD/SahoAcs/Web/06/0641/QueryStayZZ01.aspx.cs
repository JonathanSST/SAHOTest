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
    public partial class QueryStayZZ01 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<StayCardLog> ListLog = new List<StayCardLog>();
        public List<StayCardLog> ListLogOut = new List<StayCardLog>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardDate";
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
        
        public IPagedList<StayCardLog> PagedList;

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
            ClientScript.RegisterClientScriptInclude("ZZ01", "QueryStayZZ01.js?"+DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100,TitleName="姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "工號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 74, TitleWidth = 70,TitleName="卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardDate", DataWidth = 124, TitleWidth = 120, TitleName = "日期" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 124, TitleWidth = 120, TitleName = "刷卡時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeIn", DataWidth = 124, TitleWidth = 120, TitleName = "入廠時間" });
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

        private void SetWorkTable()
        {
            //查詢班表 B03_ClassRoomTable
            string sqlcmd = @"SELECT 
	                    B.*,Convert(VARCHAR(10),InOutDate,111) AS CardDate,Org.OrgName AS DepName,InOutDate,InTime
                        FROM 
	                        B03_ClassRoomTable A
	                        INNER JOIN V_PsnCard B ON A.PsnNo=B.PsnNo
							LEFT JOIN OrgStrucAllData('Unit') AS Org ON B.OrgStrucID=Org.OrgStrucID
                        WHERE
	                        Convert(DATETIME,InOutDate) BETWEEN @CardDateS AND @CardDateE ORDER BY CardDate,CardNo";
            //取得匯入的上下班紀錄
            var ClassLog = this.odo.GetQueryResult<ClassRoomTable>(sqlcmd, new { CardDateS = Request["CardDateS"], CardDateE = Request["CardDateE"] }).ToList();
            //根據當日班表與上班紀錄進行處理
            //若有上班紀錄，則排除在宿及非在宿
            this.ListLog.ForEach(o =>
            {
                //有當日上班卡的紀錄則須要排除
                if (ClassLog.Where(i => i.CardNo == o.CardNo && i.CardDate == o.CardDate && i.InTime != null).Count() == 0)
                {
                    o.SyncMark3 = 1;         //註記為未在宿
                }
                if (ClassLog.Where(i => i.CardNo == o.CardNo && i.CardDate == o.CardDate && i.InTime != null).Count() > 0)
                {
                    TimeSpan TimeAll = new TimeSpan(DateTime.Parse(o.CardDate+" "+o.CardTime).Ticks- ClassLog.Where(i => i.CardNo == o.CardNo && i.CardDate == o.CardDate && i.InTime != null).First().InTime.Value.Ticks);
                    if (TimeAll.Minutes >= 5)
                    {
                        o.SyncMark3 = 1;         //註記為未在宿
                        o.CardTimeIn = ClassLog.Where(i => i.CardNo == o.CardNo && i.CardDate == o.CardDate && i.InTime != null).First().InTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    }                    
                }
            });
            this.ListLog = this.ListLog.Where(i => i.SyncMark3 == 1).ToList();
            if (Request["InOutType"] == "出")
            {
                
                //執行非在宿設定
                var InCardLog = this.odo.GetQueryResult<StayCardLog>(sql, new { CardDateS = Request["CardDateS"], CardDateE = Request["CardDateE"], InOutType = "進" }).ToList();
                /*  這邊20210104 有拿掉                */
                InCardLog.ForEach(o =>
                {
                    //有當日上班卡的紀錄則須要排除
                    if (ClassLog.Where(i => i.CardNo == o.CardNo && i.CardDate == o.CardDate && i.InTime != null).Count() == 0)
                    {
                        o.SyncMark3 = 1;         //註記為未在宿
                    }
                });
                InCardLog = InCardLog.Where(i => i.SyncMark3 == 1).ToList();


                //比對人員名單，cardlog 進出都沒刷的人也顯示非在宿
                var PsnCards = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.*,Org.OrgName FROM V_PsnCard A
                                                                                                            LEFT JOIN OrgStrucAllData('Unit') AS Org ON A.OrgStrucID = Org.OrgStrucID").ToList();
                var MergeData = InCardLog.Union(this.ListLog).ToList();
                var CardDateInfo = MergeData.Select(i => i.CardDate).Distinct();
                foreach (var dateinfo in CardDateInfo)
                {
                    //加入當日未刷卡的人員至非在宿紀錄
                    var DateLogs = MergeData.Where(i => i.CardDate == dateinfo).Select(i => i.PsnNo);
                    PsnCards.Where(i => !MergeData.Where(d => d.CardDate == dateinfo).Select(ns => ns.PsnNo).Contains(i.PsnNo)).ToList().ForEach(i =>
                    {
                        //有當日上班卡的紀錄則須要排除
                        if (ClassLog.Where(log => log.CardNo == i.CardNo && log.CardDate == dateinfo && log.InTime != null).Count() == 0)
                        {
                            this.ListLog.Add(new DBModel.StayCardLog()
                            {
                                DepName = i.OrgName,
                                PsnNo = i.PsnNo,
                                CardNo = i.CardNo,
                                CardDate = dateinfo,
                                PsnName = i.PsnName
                            });
                        }
                    });
                }
            }//若查詢非在宿，要合併計入當入無刷卡的人員資料
          
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
            sql = @"SELECT DISTINCT C.OrgName AS DepName,A.CardNo,A.EquDir
				,CONVERT(VARCHAR(10),A.CardTime,111) AS CardDate,B.PsnName,B.PsnNo
				,SUBSTRING(CONVERT(VARCHAR(50),A.CardTime,121),12,8) AS CardTime
				 FROM B01_CardLog A 
	                    INNER JOIN (
                    SELECT 
	                    CardNo,MAX(CardTime) AS CardTime,CONVERT(VARCHAR(10),CardTime,111) AS CardDate
                    FROM 
	                    B01_CardLog A
                        INNER JOIN B01_EquData E ON A.EquNo=E.EquNo				
                    GROUP BY CardNo,CONVERT(VARCHAR(10),CardTime,111)) B2 ON A.CardNo=B2.CardNo AND A.CardTime=B2.CardTime
	                     INNER JOIN V_PsnCard B ON A.PsnNo=B.PsnNo
	                     LEFT JOIN OrgStrucAllData('Unit') C ON B.OrgStrucID=C.OrgStrucID WHERE EquDir=@InOutType AND A.CardTime BETWEEN @CardDateS AND @CardDateE";
          
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");            
            //取得在宿與非在宿的紀錄
            this.ListLog = this.odo.GetQueryResult<StayCardLog>(sql,new { CardDateS = Request["CardDateS"], CardDateE = Request["CardDateE"], InOutType=Request["InOutType"] }).ToList();
            this.SetWorkTable();
            bool boolSort = this.SortType.Equals("ASC");
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog.OrderByField(SortName, boolSort).ToList());
            }
            else
            {
                //轉datatable
                //this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog.OrderByField(SortName, boolSort).ToList());
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
            ws.Cells[1, this.ListCols.Count+1].Value = "進出類型";
            //Content
            for (int i = 0; i < this.DataResult.Rows.Count; i++)
            {
                for (int col = 0; col < this.ListCols.Count(); col++)
                {
                    ws.Cells[i + 2, col + 1].Value = this.DataResult.Rows[i][this.ListCols[col].ColName].ToString(); 
                }
                ws.Cells[i + 2, this.ListCols.Count + 1].Value = Request["InOutType"].ToString()=="進"?"在宿":"非在宿";
            }
            //ws.Cells[this.DataResult.Rows.Count+2, 1, this.DataResult.Rows.Count+2, this.ListCols.Count].Merge = true;
            //ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = string.Format("未出人數：{0}，未進人數：{1}",this.ListLog.Where(i=>i.EquDir=="進").Count(),this.ListLog.Where(i=>i.EquDir=="出").Count());
            ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = "總人數";
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