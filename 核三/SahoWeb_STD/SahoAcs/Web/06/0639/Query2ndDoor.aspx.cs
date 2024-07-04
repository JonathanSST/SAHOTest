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
    public partial class Query2ndDoor : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<OracleTemp> ListLog = new List<OracleTemp>();

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

        public IPagedList<OracleTemp> PagedList;

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
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryOneLog")
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
            ClientScript.RegisterClientScriptInclude("0639", "Query2ndDoor.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
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
                OutId=0
                
            };
            return obj;
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
            sql = @"SELECT * FROM B00_OracleRecord WHERE (InTime2nd BETWEEN @CardDateS AND @CardDateE) OR (OutTime2nd BETWEEN @CardDateS AND @CardDateE) ORDER BY IPLTTM";
            this.ListLog = this.odo.GetQueryResult<OracleTemp>(sql, new
            {
                CardDateS = Request["CardDateS"],
                CardDateE = Request["CardDateE"],
                AbnormalType = Request["AbnormalType"]
            }).ToList();
            this.ListLog.ForEach(i => i.IPLTTM2 = i.IPLTTM.Substring(0, 7));
            this.ListLog = this.ListLog.GroupBy(i => new { i.IDNO, i.IPLTTM2 }).Select(i => i.First()).ToList();
            var CardLogs = odo.GetQueryResult<CardLogModel>("SELECT * FROM B01_CardLog WHERE CardTime BETWEEN @CardTimeS AND @CardTimeE", new
            {
                CardTimeS = Request["CardDateS"],
                CardTimeE = Request["CardDateE"],
            });
            List<OracleTemp> TempData = new List<OracleTemp>();            
            foreach(var o in this.ListLog)
            {
                OracleTemp newlog = this.GetDataCopy(o);
                foreach (var log in CardLogs.Where(i=> ((i.CardTime.Year - 1911) + i.CardTime.ToString("MMdd")).Equals(o.IPLTTM.Substring(0,7))
                    && (i.CardNo==AppServiceExtension.GetCardNo(o.IDNO)||i.CardNo==AppServiceExtension.GetCardNo(o.IPLTLIC)+"0019")
                    &&i.PsnNo == o.IDNO).OrderBy(i=>i.CardTime))
                {
                    if (log.EquDir == "進")
                    {
                        newlog = this.GetDataCopy(o);
                        newlog.InTime2nd = log.CardTime;
                        newlog.InId = log.RecordID;
                        TempData.Add(newlog);
                    }                    
                }
                DateTime? InputTime = null;
                if (TempData.Where(i=>i.IPLTTM == o.IPLTTM && i.IDNO==o.IDNO).Count() > 0)
                {
                    InputTime = TempData.Where(i => i.IPLTTM == o.IPLTTM && i.IDNO == o.IDNO).First().InTime2nd;
                }
                foreach (var log in CardLogs.Where(i => ((i.CardTime.Year - 1911) + i.CardTime.ToString("MMdd")).Equals(o.IPLTTM.Substring(0,7))
                    && (i.CardNo == AppServiceExtension.GetCardNo(o.IDNO) || i.CardNo == AppServiceExtension.GetCardNo(o.IPLTLIC) + "0019")
                    && i.PsnNo == o.IDNO).OrderBy(i => i.CardTime))
                {
                    if (log.EquDir == "出")
                    {
                        if (TempData.Where(i => i.IDNO == o.IDNO && i.IPLTTM == o.IPLTTM && i.InTime2nd<log.CardTime && InputTime<=i.InTime2nd && i.OutTime2nd==null).Count() > 0)
                        {
                            var last = TempData.Where(i => i.IDNO == o.IDNO && i.IPLTTM == o.IPLTTM && i.InTime2nd < log.CardTime && InputTime<=i.InTime2nd && i.OutTime2nd == null).OrderBy(i=>i.InTime2nd).Last();
                            last.OutId = log.RecordID;
                            last.OutTime2nd = log.CardTime;
                            InputTime = last.InTime2nd.Value;
                            //TempData.Where(i => i.IDNO == o.IDNO && i.IPLTTM == o.IPLTTM && i.InTime2nd < log.CardTime && i.OutTime2nd == null).Last().OutId = log.RecordID;
                        }
                        else
                        {
                            newlog = this.GetDataCopy(o);
                            newlog.OutTime2nd = log.CardTime;
                            newlog.OutId = log.RecordID;
                            TempData.Add(newlog);
                        }
                    }
                }
            }
            this.ListLog = TempData;            
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
            ws.Cells[1, 1].Value = "2道入廠時間";
            ws.Cells[1, 2].Value = "2道出廠時間";
            ws.Cells[1, 3].Value = "1道入廠時間";
            ws.Cells[1, 4].Value = "人員ID";
            ws.Cells[1, 5].Value = "姓名";
            ws.Cells[1, 6].Value = "廠商名稱";
            ws.Cells[1, 7].Value = "廠商編號";
            ws.Cells[1, 8].Value = "工程編號";
            ws.Cells[1, 9].Value = "入廠卡號";
            ws.Cells[1, 10].Value = "部門";
            ws.Cells[1, 11].Value = "部門名稱";
            //Content
            for (int i = 0; i < this.ListLog.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.ListLog[i].InTime2nd);
                ws.Cells[i + 2, 2].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.ListLog[i].OutTime2nd);
                ws.Cells[i + 2, 3].Value = this.ListLog[i].IPLTTM;
                ws.Cells[i + 2, 4].Value = this.ListLog[i].IDNO;
                ws.Cells[i + 2, 5].Value = this.ListLog[i].NM;
                ws.Cells[i + 2, 6].Value = this.ListLog[i].VNDNM;
                ws.Cells[i + 2, 7].Value = this.ListLog[i].VNDID;
                ws.Cells[i + 2, 8].Value = this.ListLog[i].VHNO;
                ws.Cells[i + 2, 9].Value = this.ListLog[i].IPLTLIC;
                ws.Cells[i + 2, 10].Value = this.ListLog[i].DPID;
                ws.Cells[i + 2, 11].Value = this.ListLog[i].DPNM;
            }
            ws.Cells[this.ListLog.Count + 2, 1].Value = "總筆數：";
            ws.Cells[this.ListLog.Count + 2, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=Query2ndDoor.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }




        private void EmptyCondition()
        {
            for (int i = 0; i < 1; i++)
            {

            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
        }

    }//end page class
}//end namespace