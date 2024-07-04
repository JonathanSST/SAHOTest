using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using System.Data;
using PagedList;
using OfficeOpenXml;


namespace SahoAcs
{
    public partial class PsnItinerary : System.Web.UI.Page
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
        double price = 8.5, price1 = 7.5;


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
            this.CardDateS.DateValue = this.GetZoneTime().ToString("yyyy/MM" + "/01");
            if (dtLastCardTime <= this.GetZoneTime())
            {
                this.CardDateE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            else
            {
                this.CardDateE.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd");
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
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN @ParaNo", new { ParaNo=new string[] { "Itinerary", "Itinerary1" } }))
            {
                try
                {
                    if (Convert.ToString(o.ParaNo).Equals("Itinerary"))
                        this.price = Convert.ToDouble(o.ParaValue);
                    if (Convert.ToString(o.ParaNo).Equals("Itinerary1"))
                        this.price1 = Convert.ToDouble(o.ParaValue);
                }
                catch (Exception ex)
                {

                }
            }
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
            this.ListLog = this.odo.GetQueryResult<Itinerary>(@"SELECT A.*, StateDesc FROM Itinerary A 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code AND Code BETWEEN 84 AND 89
                                                WHERE LocalTime BETWEEN @DateS AND @DateE AND PsnNo=@PsnNo ORDER BY CardTime", new
            {
                DateE = Request.Form["DateE"] + " 23:59:59",
                DateS = Request.Form["DateS"] + " 00:00:00",
                PsnNo
            }).ToList();           
            if (Request["PageEvent"] == "Print")
            {
                this.ListLog.ForEach(i => {
                    i.CardDate = i.LocalTime.Value.ToString("yyyy/MM/dd");
                    i.Note = string.IsNullOrEmpty(i.Note) ? "未填寫" : i.Note;
                });
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

       

        private List<Itinerary> GetReWork()
        {
            //以人員資料做 Group 
            var result = new List<Itinerary>(this.ListLog.ToArray());
            result = result.GroupBy(i => new { i.PsnNo, i.CardDate }).Select(i => i.First()).ToArray().ToList();            
            bool Check85 = false;
            string Note = "";
            //以每個人的Group 結果做處理
            result.ForEach(i =>
            {
                i.ItinAmount = this.ListLog.Where(log =>log.CardDate.Equals(i.CardDate)).Sum(log => log.Distance.Value);
                if (i.TravelMode.Equals("Driving"))
                    i.Amount = i.ItinAmount * this.price;
                else
                    i.Amount = i.ItinAmount * this.price1;
                Check85 = false;
                Note = "";
                //Note 以項目打卡項目85 為一個區間
                foreach (var o in this.ListLog.Where(log => log.CardDate.Equals(i.CardDate)))
                {
                    if (!string.IsNullOrEmpty(Note) && !Check85)
                    {
                        Note += "=>";
                    }
                    Note += o.Note;
                    Check85 = false;
                    if (o.LogStatus == 85)
                    {
                        Note += "\n";
                        Check85 = true;
                    }
                }
                i.Note = Note;
            });
            return result;
        }


        private void ExportExcel()
        {
            //建立Excel
            ExcelPackage ep = new ExcelPackage();           
            //建立第一個Sheet，後方為定義Sheet的名稱
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("個人期間行程表");                        
            string DateS = "", DateE = "";
            DateS = Request.Form["DateS"];
            DateE = Request.Form["DateE"];
            sheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 1, 2, 3].Merge = true;
            sheet.Cells[2, 1].Value = "個 人 期 間 行 程 查 詢";
            sheet.Cells[2, 1].Style.Font.Name = "標楷體";
            sheet.Cells[2, 1].Style.Font.Size = 16;
            sheet.Cells[4, 1].Value = string.Format("人員編號:{0}    姓名:{1}", PsnNo, PsnName);
            sheet.Cells[4, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;            
            sheet.Cells[4, 3].Value = string.Format("距離單位(公里)每單位補貼汽車({0:0.0})、機車({1:0.0})", this.price, this.price1);
            sheet.Cells[5, 1].Value = string.Format("打卡期間:{0}~{1}", DateS, DateE);
            sheet.Cells[5, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;            
            sheet.Cells[5, 3].Value = string.Format("查詢時間:{0:yyyy/MM/dd HH:mm:ss}", this.GetZoneTime());
            sheet.Cells[6, 1].Value = "日期";            
            sheet.Cells[6, 2].Value = "打卡地點";
            sheet.Cells[6, 3].Value = "兩地距離";
            sheet.Cells[6, 4].Value = " * 汽車";
            sheet.Cells[6, 1, 6, 3].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            int RowIndex = 7;
            var RstLog = this.GetReWork();
            foreach (var o in RstLog)
            {
                sheet.Cells[RowIndex, 1].Value = o.CardDate;
                sheet.Cells[RowIndex, 2].Value = o.Note;
                sheet.Cells[RowIndex, 2].Style.WrapText = true;
                sheet.Cells[RowIndex, 3].Value = o.ItinAmount;
                sheet.Cells[RowIndex, 4].Value = o.TravelMode.Equals("Driving") ? " *" : "";
                RowIndex++;
            }
            sheet.Cells[RowIndex, 1, RowIndex, 3].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[RowIndex, 2].Value = "總計：";
            sheet.Cells[RowIndex, 2, RowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 3].Value = RstLog.Sum(i => i.ItinAmount);
            RowIndex++;
            sheet.Cells[RowIndex, 2].Value = "補助金額：";
            sheet.Cells[RowIndex, 2, RowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 3].Value = string.Format("NT$ {0:0}", RstLog.Sum(i => i.Amount)); 
            sheet.Column(1).Width = 12;  
            sheet.Column(2).Width = 66;
            sheet.Column(3).Width = 12;
            RowIndex += 2;
            sheet.Cells[RowIndex, 2].Value = "＊補助金額數據僅供參考，實際數據以會計審核金額為基準＊";
            RowIndex += 6;
            sheet.Cells[RowIndex, 2].Value = "副總：              主管：                 製表：";
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
            Response.AddHeader("content-disposition", "attachment; filename=PsnPeriodItinerary.xlsx");
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