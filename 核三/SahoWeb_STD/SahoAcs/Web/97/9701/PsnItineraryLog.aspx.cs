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
using System.Xml;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;

namespace SahoAcs
{
    public partial class PsnItineraryLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<Itinerary> ListLog = new List<Itinerary>();
        public List<Itinerary> ListPsnData = new List<Itinerary>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";

        
        public int PageIndex = 1;
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "", PsnNo = "", PsnName = "";
        public string AuthList = "";
        public IPagedList<Itinerary> PagedList;

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
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Save")
            {
                this.SaveData();
            }
            else
            {
                this.SetInitData();
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            if (!IsPostBack)
            {
                this.AuthList = Sa.Web.Fun.GetSessionStr(this, "FunAuthSet");
                if (WebAppService.GetSysParaData("IsShowMap") == "1")
                {
                    this.AuthList += ",ShowMap";
                }
            }
            ClientScript.RegisterClientScriptInclude("9701", "Query.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            this.ListPsnData = this.odo.GetQueryResult<Itinerary>("SELECT * FROM B01_Person").ToList();
            this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM Itinerary WHERE CardTime <= @ZoneTime",new {ZoneTime=this.GetZoneTime()});

            if (dtLastCardTime == DateTime.MinValue)
            {
                this.CardDateS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            else
            {
                this.CardDateS.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd");
            }
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


        private void SetOneCardLog()
        {
            MobileLogModel model = new MobileLogModel();
            string sql = @"SELECT *,PsnName AS EmpName,
                        ISNULL((SELECT TOP 1 StateDesc FROM B00_CardLogState WHERE CONVERT(VARCHAR,Code)=OpType),'') AS OpName
                        FROM FillCardLog INNER JOIN B01_Person ON EmpID=PsnNo WHERE RecordID=@RecordID ";
            var logs = this.odo.GetQueryResult<MobileLogModel>(sql, new { RecordID = Request["RecordID"] });
            foreach(var o in logs)
            {
                model = o;
            }
            XmlDocument xd = new XmlDocument();
            string path = Request.PhysicalPath;
            xd.Load(Request.Url.Scheme + "://" + Request.Url.Authority + "/SysPara.xml");
            XElement doc = XElement.Parse(xd.OuterXml);
            path = doc.Element("MobileImg").Value;
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "", card_log = model, error_msg = "", PsnPicSource = path + model.CardPic, card_time=string.Format("{0:yyyy/MM/dd HH:mm}",model.CardTime) }));
            Response.End();
        }

        private void SetQueryData()
        {
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");
            string WhereSql = "";                       
            if (!string.IsNullOrEmpty(this.PsnID))
            {
                WhereSql = " WHERE PsnID=@PsnID";
            }
            else
            {
                if (Request.Form["QueryPsnNo"] != null)
                {
                    PsnNo = Request.Form["QueryPsnNo"];
                    WhereSql = " WHERE PsnNo=@PsnNo";
                }
            }
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B01_Person " + WhereSql, new { PsnID, PsnNo }))
            {
                this.PsnNo = Convert.ToString(o.PsnNo);
                this.PsnName = Convert.ToString(o.PsnName);
            }
            this.ListLog = this.odo.GetQueryResult<Itinerary>(@"SELECT A.*, Oplatitude, OpLongitude, StateDesc FROM Itinerary A 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code 
                                                INNER JOIN FillCardLog ON RecordID=NowRecordID
                                                WHERE LocalTime BETWEEN @DateS AND @DateE AND PsnNo=@PsnNo ORDER BY CardTime", new
            {
                DateE = Request.Form["DateS"] + " 23:59:59",
                DateS = Request.Form["DateS"] + " 00:00:00",
                PsnNo
            }).ToList();

            //this.ListLog = TempData.Where(i=>i.InTime2nd.;            
            if (Request["PageEvent"] == "Print")
            {
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {                
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

        private void SaveData()
        {
            var NoteList = Request.Form.GetValues("Note");
            var NowRecords = Request.Form.GetValues("NowRecordID");
            int count = 0;
            foreach(var record in NowRecords)
            {
                this.odo.Execute("UPDATE Itinerary SET Note=@Note WHERE NowRecordID=@RecordID", new {Note=NoteList[count], RecordID=record});
                count++;
            }
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "更新完成"}));
            Response.End();
        }


        private void ExportExcel()
        {
            //建立Excel
            ExcelPackage ep = new ExcelPackage();
            //建立第一個Sheet，後方為定義Sheet的名稱
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("個人單日行程表");
            string DateS = "";            
            DateS = Request.Form["DateS"];
            sheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 1, 2, 4].Merge = true;
            sheet.Cells[2, 1].Value = "個 人 單 日 行 程 查 詢";
            sheet.Cells[2, 1].Style.Font.Name = "標楷體";
            sheet.Cells[2, 1].Style.Font.Size = 16;
            sheet.Cells[4, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[4, 1].Value = string.Format("人員編號:{0}    姓名:{1}", PsnNo, PsnName);
            sheet.Cells[4, 4].Value = "距離單位(公里)";
            sheet.Cells[5, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[5, 1].Value = string.Format("打卡日期:{0}", DateS);
            sheet.Cells[5, 4].Value = string.Format("查詢時間:{0:yyyy/MM/dd HH:mm:ss}", this.GetZoneTime());
            sheet.Cells[6, 1].Value = "打卡項目";
            sheet.Cells[6, 2].Value = "打卡時間";
            sheet.Cells[6, 3].Value = "打卡地點";
            sheet.Cells[6, 4].Value = "兩地距離";
            sheet.Cells[6, 1, 6, 4].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            int RowIndex = 7;
            foreach (var o in this.ListLog)
            {
                sheet.Cells[RowIndex, 1].Value = o.StateDesc;
                sheet.Cells[RowIndex, 2].Value = string.Format("{0:HH:mm:ss}", o.CardTime.AddHours(o.TimeZone.Value));
                sheet.Cells[RowIndex, 3].Value = o.Note;
                sheet.Cells[RowIndex, 4].Value = o.Distance;
                RowIndex++;
            }
            sheet.Cells[RowIndex, 3].Value = "總計：";
            sheet.Cells[RowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 4].Value = this.ListLog.Sum(i => i.Distance);
            sheet.Cells[RowIndex, 1, RowIndex, 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //sheet.Cells.AutoFitColumns();
            sheet.Column(1).Width = 10;
            sheet.Column(2).Width = 10;
            sheet.Column(3).Width = 60;
            sheet.Column(4).Width = 10;
            //格線設定
            for (int i = 4; i <= sheet.Dimension.End.Row; i++)
            {
                for (int j = 1; j <= sheet.Dimension.End.Column; j++)
                {
                    //sheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[i, j].Style.Font.Size = 12;
                    sheet.Cells[i, j].Style.Font.Name = "標楷體";
                }
            }
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=PsnItinerary.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(ep.GetAsByteArray());
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