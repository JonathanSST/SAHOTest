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
using System.Web.Script.Serialization;


namespace SahoAcs
{
    public partial class QueryOsListZZ01 : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<OsManageEntity> ListLog = new List<OsManageEntity>();
        public List<OsManageEntity> ListAll = new List<OsManageEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();

        public string SqlMainCmd = @"SELECT PsnNo,CardNo,A.EquNo,CardTime,A.CompNo,TitleNo,PsnName,CardDate,EquName
                       FROM B03_OsManage A 
                        INNER JOIN B01_EquData B ON A.EquNo=B.EquNo                        
                        WHERE CardDate BETWEEN @CardDateS AND @CardDateE AND CompNo IN @CompList  AND TitleNo IN @TitleList ";
        public string SqlOrderCmd = " ORDER BY PsnNo,CardDate,CardTime ";

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

        public IPagedList<OsManageEntity> PagedList;

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
            ClientScript.RegisterClientScriptInclude("0639", "QueryOsListZZ01.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
                this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
            }
            #endregion

            #region 設定欄位寬度、名稱內容預設

            #endregion
            this.LogStatus.Add(new DBModel.LogState() { Code = 0, StateDesc = "有刷進、沒刷出" });
            this.LogStatus.Add(new DBModel.LogState() { Code = 1, StateDesc = "有刷出、沒刷進" });
            this.LogStatus.Add(new DBModel.LogState() { Code = 2, StateDesc = "入廠(二道門禁)超過一小時" });


            //確認系統參數是否有加入進出門禁定義
            var ParaList = this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='InOutDir' ");
            var ParaValue = new { In = "I05_1", Out = "L05_1" };
            string StrValue = new JavaScriptSerializer().Serialize(ParaValue);
            if (ParaList.Count() == 0)
            {
                //this.odo.Execute("INSERT INTO ");
                this.odo.Execute(@"INSERT INTO B00_SysParameter 
                        (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                        VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                           new { ParaValue = StrValue, ParaNo = "InOutDir", User = "Saho", ParaName = "InOutDir的參數" });
            }
            foreach (var o in ParaList)
            {
                if (Convert.ToString(o.ParaValue) == string.Empty)
                {
                    this.odo.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue WHERE ParaNo='InOutDir' ", new { ParaValue = StrValue });
                }
            }
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
            //將來要保留的區塊
            string sql = this.SqlMainCmd;
            if (Request["QueryComp"] != null && Request["QueryComp"].ToString() != "0")
            {
                sql += " AND CompNo=@CompNo ";
            }
            sql += this.SqlOrderCmd;
            var OrgData = this.odo.GetQueryResult<OrgDataEntity>("SELECT * FROM B01_OrgData WHERE OrgClass IN ('Title','Company')").ToList();
            var CompList = this.odo.GetQueryResult<OrgDataEntity>(@"SELECT 
	                                                                                                        B.* 
                                                                                                        FROM 
	                                                                                                        OrgStrucAllData('Title') AS A
	                                                                                                        INNER JOIN OrgStrucAllData('Company') AS B ON A.OrgStrucID=B.OrgStrucID
                                                                                                        WHERE A.OrgNo IN @TitleList ", new { TitleList = WebAppService.GetSysParaData("OsTitle").Split(',') }).ToList();            
            this.ListLog = this.odo.GetQueryResult<OsManageEntity>(sql, new
            {
                CardDateS = Convert.ToDateTime(Request.Form["CardDateS"]).ToString("yyyy/MM/dd"),
                CardDateE = Convert.ToDateTime(Request.Form["CardDateE"]).ToString("yyyy/MM/dd"),
                TitleList = WebAppService.GetSysParaData("TitleList").Split(',').ToList(),
                CompList=CompList.Select(i=>i.OrgNo), PsnNo = Request["inputPsnCard"]+"%", CompNo=Request["QueryComp"]
            }).ToList();
            List<OsManageEntity> OutCardLog = new List<OsManageEntity>(this.ListLog.Where(i => WebAppService.GetSysParaData("EquOutList").Split(',').Contains(i.EquNo)))
                .OrderByDescending(i=>i.CardTime).OrderBy(i=>i.PsnNo).ToList();
            OsManageEntity OldLog = new OsManageEntity();
            var NewOutLog = new List<OsManageEntity>();            
            foreach (var o in OutCardLog)
            {
                if (OldLog.PsnNo == null || o.PsnNo == "")
                {
                    NewOutLog.Add(o);
                    OldLog = o;
                }
                else
                {
                    if (OldLog.PsnNo != o.PsnNo || OldLog.CardDate != o.CardDate)
                    {
                        NewOutLog.Add(o);
                        OldLog = o;
                    }                    
                }                
            }
            List<OsManageEntity> InCardLog = new List<OsManageEntity>(this.ListLog.Where(i => WebAppService.GetSysParaData("EquInList").Split(',').Contains(i.EquNo)));
            OldLog = new OsManageEntity();
            var NewIntLog = new List<OsManageEntity>();
            foreach (var o in InCardLog)
            {
                if (OldLog.PsnNo == null || o.PsnNo == "")
                {
                    //o.CompNo = OrgData.Where(i => i.OrgNo == o.CompNo).Count() > 0 ?  : "";
                    NewIntLog.Add(o);
                    OldLog = o;
                }
                else
                {
                    if (OldLog.PsnNo != o.PsnNo || OldLog.CardDate != o.CardDate)
                    {
                        NewIntLog.Add(o);
                        OldLog = o;
                    }
                }
            }
            //NewIntLog.ForEach(i => { i.First = i.CardTime.ToString("HH:mm:ss"); i.Last = ""; });
            //NewOutLog.ForEach(i => { i.Last = i.CardTime.ToString("HH:mm:ss"); i.First = ""; });
            int StartMin = int.Parse(WebAppService.GetSysParaData("StartMin"));
            int EndMin = int.Parse(WebAppService.GetSysParaData("EndMin"));
            NewIntLog.ForEach(i => {
                i.First = WebAppService.GetSysParaData("OsTitle").Split(',').Contains(i.TitleNo) ? i.CardTime.AddMinutes(-StartMin).ToString("HH:mm:ss")
                : i.CardTime.ToString("HH:mm:ss"); i.Last = "";
                i.FirstReal = i.CardTime.ToString("HH:mm:ss");
                i.CardTimeReal = i.CardTime;
                i.CardTime = WebAppService.GetSysParaData("OsTitle").Split(',').Contains(i.TitleNo) ? i.CardTime.AddMinutes(-StartMin)
                : i.CardTime;
            });
            NewOutLog.ForEach(i => {
                i.Last = WebAppService.GetSysParaData("OsTitle").Split(',').Contains(i.TitleNo) ? i.CardTime.AddMinutes(EndMin).ToString("HH:mm:ss")
                : i.CardTime.ToString("HH:mm:ss"); i.First = "";
                i.LastReal = i.CardTime.ToString("HH:mm:ss");
                i.CardTimeReal = i.CardTime;
                i.CardTime = WebAppService.GetSysParaData("OsTitle").Split(',').Contains(i.TitleNo) ? i.CardTime.AddMinutes(EndMin)
                : i.CardTime;
            });
            string last_time = "00:00:00";      
            string card_no = "";
            string card_date = "";
            //以出廠資料清單進行迭代處理
            foreach (var o in NewOutLog)
            {
                if(o.PsnNo!= card_no || o.CardDate!=card_date)
                {
                    last_time = "00:00:00";
                }
                //判斷入廠資料清單
                //判斷依據: 1.卡號與日期同一天 2.尚未記載出廠時間，出廠時間必須大於入廠時間 3.取得的入廠時間也必須大於前次的出廠時間，符合先進後出
                if (NewIntLog.Where(i =>i.CardDate==o.CardDate && i.PsnNo == o.PsnNo && i.LogStatus == o.LogStatus && i.Last == "" && i.CardTime < o.CardTime && i.First.CompareTo(last_time) >= 0).Count() > 0)
                {
                    //取得符合條件的入廠資料
                    var last = NewIntLog.Where(i => i.PsnNo == o.PsnNo && i.LogStatus == o.LogStatus && i.Last == "" && i.CardTime < o.CardTime && i.First.CompareTo(last_time) >= 0).Last();
                    last.Last = o.CardTime.ToString("HH:mm:ss");    //將本次的迭代變數，記載到資料的出廠時間欄位變數
                    last.LastReal = o.CardTimeReal.ToString("HH:mm:ss");                    
                    last.EquName2 = o.EquName;
                    last_time = last.CardTime.ToString("HH:mm:ss");	//將新的入廠時間，代入到資料比較的時間變數
                }
                else
                {
                    //若查無條件符合的入廠資料進行設置，則直接本次的紀錄出廠紀錄，以新的紀錄append到 入廠資料清單 
                    o.Last = o.CardTime.ToString("HH:mm:ss");                    
                    o.LastReal = o.CardTimeReal.ToString("HH:mm:ss");
                    o.EquName2 = o.EquName;
                    o.EquName = "";
                    NewIntLog.Add(o);
                }
            }
            this.ListLog.ForEach(i => {
                i.CompName = OrgData.Where(j => j.OrgNo == i.CompNo).Count() > 0 ? OrgData.Where(j => j.OrgNo == i.CompNo).First().OrgName :"";
                i.TitleName = OrgData.Where(j => j.OrgNo == i.TitleNo).Count() > 0 ? OrgData.Where(j => j.OrgNo == i.TitleNo).First().OrgName : "";
            });            
            this.ListLog = NewIntLog;
            //重新進行，產生資料要輸出的格式
            this.SetReOutput();
        }



        ///<summary>重新進行，產生資料要輸出的格式</summary>
        private void SetReOutput()
        {      
            //var comp_list = this.ListLog.Where(i => WebAppService.GetSysParaData("OsTitle").Split(',').Contains(i.TitleNo)).Select(i=>i.CompNo).ToList();
            //this.ListLog = this.ListLog.Where(i => comp_list.Contains(i.CompNo)).ToList();
            this.ListLog.ForEach(i => {
                if (!WebAppService.GetSysParaData("OsTitle").Split(',').Contains(i.TitleNo))
                {
                    i.NewTitle = "L";
                }
            });
            foreach (var o in this.ListLog.OrderBy(i => i.PsnName).OrderBy(i => i.NewTitle).OrderBy(i => i.CardDate).OrderBy(i => i.CompNo).ToList())
            {
                if (WebAppService.GetSysParaData("OsTitle").Split(',').Contains(o.TitleNo))
                {
                    o.LogStatus = "正常";
                }
                if ((WebAppService.GetSysParaData("OsTitle").Split(',').Contains(o.TitleNo)
                            && this.ListLog.Where(i => i.CardDate == o.CardDate && i.CompNo == o.CompNo && i.NewTitle == "L"
                            && (i.First.CompareTo(o.First) < 0 || i.Last.CompareTo(o.Last) > 0)).Count() > 0) || o.First == "" || o.Last == "")
                {
                    o.LogStatus = "異常";
                }
            }
            this.ListAll = new List<OsManageEntity>(this.ListLog);
            if (Request["inputPsnCard"] != null && Request["inputPsnCard"] != "")
            {
                string PsnName = Request["inputPsnCard"].ToString();
                this.ListLog = this.ListLog.Where(i => i.PsnName.StartsWith(PsnName) || i.CardNo.StartsWith(PsnName) || i.PsnNo.StartsWith(PsnName)).ToList();
            }
            if (Request["QueryLogStatus"] != null && Request["QueryLogStatus"] != "")
            {
                string Status = Request["QueryLogStatus"];
                this.ListLog = this.ListLog.Where(i => i.LogStatus == Status).ToList();
                //if (Status=="異常")
                //    this.ListLog = this.ListLog.Where(i => i.LogStatus == Status).ToList();
                //else
                //    this.ListLog = this.ListLog.Where(i => i.LogStatus != Status).ToList();
            }
            if (Request["QueryTitle"] != null && Request["QueryTitle"] != "0")
            {
                this.ListLog = this.ListLog.Where(i => i.TitleNo==Request["QueryTitle"].ToString()).ToList();
            }
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
        }     

        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Detail");
            ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add("Total");
            ws.Cells[1, 1].Value = string.Format("稽核日期區間：{0} ~ {1}", Request.Form["CardDateS"], Request.Form["CardDateE"]);
            ws.Cells[1, 1, 1, 10].Merge = true;
            ws2.Cells[1, 1].Value = string.Format("稽核日期區間：{0} ~ {1}", Request.Form["CardDateS"], Request.Form["CardDateE"]);
            ws.Cells[2, 1].Value = "公司";
            ws.Cells[2, 2].Value = "職稱";
            ws.Cells[2, 3].Value = "姓名";            
            ws.Cells[2, 4].Value = "狀態";
            ws.Cells[2, 5].Value = "入廠日期";
            ws.Cells[2, 6].Value = "入廠時間";
            ws.Cells[2, 7].Value = "入廠設備";
            ws.Cells[2, 8].Value = "出廠日期";
            ws.Cells[2, 9].Value = "出廠時間";
            ws.Cells[2, 10].Value = "出廠設備";
            ws2.Cells[1, 1, 1, 8].Merge = true;
            ws2.Cells[2, 1].Value = "公司";
            ws2.Cells[2, 2].Value = "職稱";
            ws2.Cells[2, 3].Value = "姓名";
            ws2.Cells[2, 4].Value = "異常次數";
            ws2.Cells[2, 5].Value = "正常次數";
            ws2.Cells[2, 6].Value = "總次數";
            ws2.Cells[2, 7].Value = "平日_無刷卡次數";
            ws2.Cells[2, 8].Value = "平日_應刷卡次數";
            //ws2.Cells[2, 9].Value = "假日刷卡次數";
            var DailyDatas = this.odo.GetQueryResult(@"SELECT WorkDay,DATEPART(WEEKDAY,WORKDAY) AS DayOf FROM (
                                                    SELECT CONVERT(VARCHAR(20),dateadd(d,rows-1,@CardDateS),111) WorkDay
                                                    FROM
                                                    (SELECT 
                                                        id,row_number()over(order by id) rows  
                                                      FROM 
                                                        sysobjects) Tmp 
                                                    WHERE Tmp.rows <= datediff(d,@CardDateS, @CardDateE) + 1) AS Result1",
                                    new
                                    {
                                        CardDateS = Convert.ToDateTime(Request.Form["CardDateS"]), CardDateE = Convert.ToDateTime(Request.Form["CardDateE"])
                                    }).ToList();
            string[] Weekends = { "1", "7" };
            DailyDatas = DailyDatas.Where(i => !Weekends.ToList().Contains(Convert.ToString(i.DayOf))).ToList();
            var groupresult = from g in this.ListAll
                              where WebAppService.GetSysParaData("OsTitle").Split(',').Contains(g.TitleNo)
                              group g by new { g.CompName, g.TitleName, g.PsnName } into groups
                              select new OsManageEntity
                              {
                                  CompName = groups.Key.CompName,
                                  TitleName = groups.Key.TitleName,
                                  PsnName = groups.Key.PsnName,
                                  WorkDay = groups.Count(i => Convert.ToDateTime(i.CardDate).DayOfWeek != DayOfWeek.Sunday && Convert.ToDateTime(i.CardDate).DayOfWeek != DayOfWeek.Saturday),
                                  Holiday = groups.Count(i => Convert.ToDateTime(i.CardDate).DayOfWeek == DayOfWeek.Sunday || Convert.ToDateTime(i.CardDate).DayOfWeek == DayOfWeek.Saturday),
                                  OkCount = groups.Count(i => i.LogStatus != "異常"),
                                  ErrCount = groups.Count(i => i.LogStatus == "異常"),
                                  AllCount = groups.Count()
                              };
            int count0 = 1;
            foreach (var o in groupresult.OrderBy(i => i.PsnName).OrderBy(i => i.CompName))
            {
                ws2.Cells[count0 + 2, 1].Value = o.CompName;
                ws2.Cells[count0 + 2, 2].Value = o.TitleName;
                ws2.Cells[count0 + 2, 3].Value = o.PsnName;
                ws2.Cells[count0 + 2, 6].Value = o.AllCount;
                ws2.Cells[count0 + 2, 4].Value = o.ErrCount;
                ws2.Cells[count0 + 2, 5].Value = o.OkCount;
                ws2.Cells[count0 + 2, 7].Value = DailyDatas.Count - o.WorkDay;
                ws2.Cells[count0 + 2, 8].Value = DailyDatas.Count;
                //ws2.Cells[count0 + 2, 9].Value = o.Holiday;
                count0++;
            }
            int count = 1;
            foreach (var o in this.ListLog.OrderBy(i => i.PsnName).OrderBy(i => i.NewTitle).OrderBy(i => i.CardDate).OrderBy(i => i.CompNo).ToList())
            {
                ws.Cells[count + 2, 1].Value = o.CompName;
                ws.Cells[count + 2, 2].Value = o.TitleName;
                ws.Cells[count + 2, 3].Value = o.PsnName;
                ws.Cells[count + 2, 4].Value = o.LogStatus;
                ws.Cells[count + 2, 5].Value = o.First != "" ? o.CardDate : "";                                                    
                if (o.First != "")
                {
                    ws.Cells[count + 2, 6].Value = DateTime.Parse(o.CardDate+" "+o.FirstReal);
                    ws.Cells[count + 2, 5].Style.Numberformat.Format = "yyyy/m/d";
                    ws.Cells[count + 2, 6].Style.Numberformat.Format = "hh:mm:ss";
                }
                ws.Cells[count + 2, 7].Value = o.EquName;
                ws.Cells[count + 2, 8].Value = o.Last != "" ? o.CardDate : "";                
                if (o.Last != "")
                {
                    ws.Cells[count + 2, 9].Value = DateTime.Parse(o.CardDate+" "+o.LastReal);
                    ws.Cells[count + 2, 8].Style.Numberformat.Format = "yyyy/m/d";
                    ws.Cells[count + 2, 9].Style.Numberformat.Format = "hh:mm:ss";
                }
                ws.Cells[count + 2, 10].Value = o.EquName2;
                count++;
            }
            ws.Cells[this.ListLog.Count + 4, 1].Value = "總筆數：";
            ws.Cells[this.ListLog.Count + 4, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            ws2.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=OsManageReport.xlsx");
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