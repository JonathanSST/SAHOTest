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
    public partial class QueryDoorList : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogTrt> ListLog = new List<CardLogTrt>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
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

        public IPagedList<CardLogTrt> PagedList;

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
            ClientScript.RegisterClientScriptInclude("0639", "QueryDoorList.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "2道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 104, TitleWidth = 100, TitleName = "2道出廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "1道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 104, TitleWidth = 100, TitleName = "人員ID" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 124, TitleWidth = 120, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 124, TitleWidth = 120, TitleName = "廠商名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "廠商編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 124, TitleWidth = 120, TitleName = "工程編號" });
            foreach (var o in this.ListCols.Where(i => i.DataRealName == null || i.DataRealName == ""))
                o.DataRealName = o.ColName;

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

        private OracleTemp GetDataCopy(OracleTemp o)
        {
            OracleTemp obj = new OracleTemp()
            {
                OPLTTM = o.OPLTTM,
                NM = o.NM,
                DPNM = o.DPNM,
                IPLTLIC = o.IPLTLIC,
                IDNO = o.IDNO,
                IPLTTM = o.IPLTTM,
                RecDesc = o.RecDesc,
                DPID = o.DPID,
                VHNO = o.VHNO,
                VNDID = o.VNDID,
                VNDNM = o.VNDNM,
                InId = 0,
                OutId = 0

            };
            return obj;
        }


        private void SetQueryData()
        {
            string In = "I05_1";
            string Out = "L05_1";
            var ParaList = this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='InOutDir' ");
            foreach (var p in ParaList)
            {
                var o = new JavaScriptSerializer().Deserialize<dynamic>(Convert.ToString(p.ParaValue));
                In = Convert.ToString(o["In"]);
                Out = Convert.ToString(o["Out"]);
            }
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");         
            //將來要保留的區塊
            string sql = @"SELECT PsnNo,CardNo,EquNo,EquName,CardTime,DepName,PsnName
                       FROM B01_CardLog WHERE CardTime BETWEEN @CardDateS AND @CardDateE AND EquNo IN (@Door1,@Door2) AND (PsnNo LIKE @PsnNo OR PsnName LIKE @PsnNo OR CardNo LIKE @PsnNo) 
                        ORDER BY CardNo,CardTime ";           
            this.ListLog = this.odo.GetQueryResult<CardLogTrt>(sql, new
            {
                CardDateS = Request["CardDateS"],
                CardDateE = Request["CardDateE"],Door1=In,Door2=Out,
                PsnNo = Request["inputPsnCard"]+"%"
            }).ToList();
            this.ListLog.ForEach(i => { i.LogStatus = i.CardTime.ToString("yyyyMMdd"); });
            List<CardLogTrt> OutCardLog = new List<CardLogTrt>(this.ListLog.Where(i => i.EquNo == In));
            //每一筆的CardTime在同天的紀錄，每筆要間隔>120秒
            CardLogTrt OldLog = new CardLogTrt();
            var NewOutLog = new List<CardLogTrt>();            
            foreach (var o in OutCardLog)
            {
                if (OldLog.CardNo == null || o.CardNo == "")
                {
                    NewOutLog.Add(o);
                    OldLog = o;
                }
                else
                {
                    if (OldLog.CardNo != o.CardNo || OldLog.LogStatus != o.LogStatus)
                    {
                        NewOutLog.Add(o);
                        OldLog = o;
                    }
                    if (OldLog.CardTime.AddSeconds(120) < o.CardTime)
                    {
                        NewOutLog.Add(o);
                        OldLog = o;
                    }                    
                }                
            }
            List<CardLogTrt> InCardLog = new List<CardLogTrt>(this.ListLog.Where(i => i.EquNo == Out));
            OldLog = new CardLogTrt();
            var NewIntLog = new List<CardLogTrt>();
            foreach (var o in InCardLog)
            {
                if (OldLog.CardNo == null || o.CardNo == "")
                {
                    NewIntLog.Add(o);
                    OldLog = o;
                }
                else
                {
                    if (OldLog.CardNo != o.CardNo || OldLog.LogStatus != o.LogStatus)
                    {
                        NewIntLog.Add(o);
                        OldLog = o;
                    }
                    if (OldLog.CardTime.AddSeconds(120) < o.CardTime)
                    {
                        NewIntLog.Add(o);
                        OldLog = o;
                    }
                }
            }

            NewIntLog.ForEach(i => { i.First = i.CardTime.ToString("HH:mm:ss"); i.Last = ""; });
            string first_time = "00:00:00";
            string card_no = "";
            string card_date = "";
            foreach (var o in NewOutLog)
            {
                if (o.CardNo != card_no||o.CardTime.ToString("yyyyMMdd")!=card_date)
                {
                    first_time = "00:00:00";                    
                }
                if (NewIntLog.Where(i => i.CardNo == o.CardNo && i.LogStatus == o.LogStatus && i.Last == "" && i.CardTime < o.CardTime && i.First.CompareTo(first_time)>=0).Count() > 0)
                {
                    var last = NewIntLog.Where(i => i.CardNo == o.CardNo && i.LogStatus == o.LogStatus && i.Last == "" && i.CardTime < o.CardTime && i.First.CompareTo(first_time) >= 0).Last();
                    last.Last = o.CardTime.ToString("HH:mm:ss");
                    first_time = last.CardTime.ToString("HH:mm:ss");
                }
                else
                {
                    o.Last = o.CardTime.ToString("HH:mm:ss");
                    NewIntLog.Add(o);
                }
                card_no = o.CardNo;
                card_date = o.CardTime.ToString("yyyyMMdd");
            }

            this.ListLog = NewIntLog.OrderByDescending(i=>i.CardTime).OrderByDescending(i=>i.PsnNo).ToList();
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
        }        

        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");            
            ws.Cells[1, 1].Value = "讀卡日期";
            ws.Cells[1, 2].Value = "人員編號";
            ws.Cells[1, 3].Value = "人員名稱";            
            ws.Cells[1, 4].Value = "部門";
            ws.Cells[1, 5].Value = "卡片號碼";
            ws.Cells[1, 6].Value = "出廠時間";
            ws.Cells[1, 7].Value = "入廠時間";
            //Content
            for (int i = 0; i < this.ListLog.Count; i++)
            {
                ws.Cells[i+2, 1].Value = string.Format("{0:yyyy/MM/dd}",this.ListLog[i].CardTime);
                ws.Cells[i + 2, 2].Value = this.ListLog[i].PsnNo;
                ws.Cells[i+2, 3].Value = this.ListLog[i].PsnName;                
                ws.Cells[i+2, 4].Value = this.ListLog[i].DepName;
                ws.Cells[i+2, 5].Value = this.ListLog[i].CardNo;
                ws.Cells[i + 2, 6].Value = this.ListLog[i].First;
                ws.Cells[i + 2, 7].Value = this.ListLog[i].Last;
                //i++;
            }
            ws.Cells[this.ListLog.Count + 2, 1].Value = "總筆數：";
            ws.Cells[this.ListLog.Count + 2, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryInOutList.xlsx");
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