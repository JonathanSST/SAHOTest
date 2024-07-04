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
    public partial class ItineraryDay : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<Itinerary> ListLog = new List<Itinerary>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<OrgDataEntity> OrgDatas = new List<OrgDataEntity>();
        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;
        double price = 8.5, price1 = 7.5;
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
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryOneLog")
            {
                this.SetOneCardLog();
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
            this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");            
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM Itinerary WHERE CardTime <= @ZoneTime",new {ZoneTime=this.GetZoneTime()});
            this.OrgDatas = this.odo.GetQueryResult<OrgDataEntity>(@"SELECT * FROM (SELECT OrgNo,OrgName,OrgStrucID,OrgID,(SELECT COUNT(*) FROM SplitString(OrgIDList,'\',1)) AS OrgArr
                                                                                                                            FROM OrgStrucAllData('Department') 
                                                                                                                            WHERE OrgNo <> '') AS T1 WHERE T1.OrgArr=2 ORDER BY OrgNo ").ToList();
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
            string sql = @"SELECT A.*,StateDesc,O.OrgName FROM Itinerary A 
                                                 INNER JOIN B01_Person P ON A.PsnNo = P.PsnNo
                                                INNER JOIN OrgStrucAllData('Department') O ON P.OrgStrucID = O.OrgStrucID
                                                INNER JOIN B00_CardLogState ON LogStatus = Code AND Code BETWEEN 84 AND 89
                                                WHERE LocalTime BETWEEN @DateS AND @DateE ";
            string DeptNo = "0";
            if (Request.Form["DeptNo"] != null && Request.Form["DeptNo"] != "0")
            {
                DeptNo = Request.Form["DeptNo"].ToString();
                sql += " AND P.OrgStrucID IN @OrgStrucID ";
            }
            sql += " ORDER BY OrgName,PsnName,CardTime";
            /* DATEDIFF(HOUR,InTime2nd,ISNULL(OutTime2nd,GETDATE()))>1 */
            this.ListLog = this.odo.GetQueryResult<Itinerary>(sql, new
            {
                DateE = Request.Form["DateS"] + " 23:59:59",
                DateS = Request.Form["DateS"] + " 00:00:00",
                OrgStrucID = this.odo.GetQueryResult(@"SELECT A.OrgStrucID
                                                                                    FROM OrgStrucAllData('Department') A
                                                                                    INNER JOIN 
                                                                                    (SELECT OrgNo,OrgName,OrgStrucID,OrgID
                                                                                    FROM OrgStrucAllData('Department') 
                                                                                    WHERE OrgNo <> '' AND OrgStrucID=@Key1) AS B ON A.OrgID=B.OrgID",
                                                                                    new { Key1 = this.Request.Form["DeptNo"].ToString() }).Select(i => i.OrgStrucID)}).ToList();
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN @ParaNos",new {ParaNos=new string[] { "Itinerary", "Itinerary1" } }))
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
            this.ListLog.ForEach(i => {
                i.CardDate = i.LocalTime.Value.ToString("yyyy/MM/dd");
                i.Note = string.IsNullOrEmpty(i.Note) ? "未填寫" : i.Note;
            });
            if (Request["PageEvent"] == "Print")
            {               
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                //this.ListLog = this.GetReWork();
                //this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

        private List<Itinerary> GetReWork()
        {
            //以人員資料做 Group 
            var result = new List<Itinerary>(this.ListLog.GroupBy(i => new { i.PsnNo, i.CardDate }).Select(i => i.First())).ToList();
            bool Check85 = false;            
            //以每個人的Group 結果做處理
            string Note = "";
            result.ForEach(i =>
            {
                i.ItinAmount = this.ListLog.Where(log => log.PsnNo.Equals(i.PsnNo) && log.CardDate.Equals(i.CardDate)).Sum(log => log.Distance.Value);
                if (i.TravelMode.Equals("Driving"))
                    i.Amount = i.ItinAmount * this.price;
                else
                    i.Amount = i.ItinAmount * this.price1;
                Check85 = false;
                Note = "";
                //Note 以項目打卡項目85 為一個區間
                foreach(var o in this.ListLog.Where(log => log.CardDate.Equals(i.CardDate) && log.PsnNo.Equals(i.PsnNo)))
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
                //i.Note = string.Join("=>", this.ListLog.Where(log => log.CardDate.Equals(i.CardDate) && log.PsnNo.Equals(i.PsnNo)).Select(log => log.Note));
                i.Note = Note;
            });
            return result;
        }


        private void ExportExcel()
        {
            //建立Excel
            ExcelPackage ep = new ExcelPackage();
            //建立第一個Sheet，後方為定義Sheet的名稱
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("單日行程表");
            string DateS = "";            
            DateS = Request.Form["DateS"];
            sheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 1, 2, 6].Merge = true;
            sheet.Cells[2, 1].Value = "單 日 行 程 查 詢";
            sheet.Cells[2, 1].Style.Font.Name = "標楷體";
            sheet.Cells[2, 1].Style.Font.Size = 16;
            sheet.Cells[4, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;            
            sheet.Cells[4, 6].Value = string.Format("距離單位(公里)每單位補貼汽車({0})、機車({1})", this.price, this.price1);
            sheet.Cells[5, 1].Value = string.Format("打卡日期:{0}", DateS);
            sheet.Cells[5, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;            
            sheet.Cells[5, 6].Value = string.Format("查詢時間:{0:yyyy/MM/dd HH:mm:ss}", this.GetZoneTime());
            sheet.Cells[6, 1].Value = "部門";
            sheet.Cells[6, 2].Value = "人員編號";
            sheet.Cells[6, 3].Value = "姓名";
            sheet.Cells[6, 4].Value = "行程內容";
            sheet.Cells[6, 5].Value = "單日行程";
            sheet.Cells[6, 6].Value = "補貼費用";
            sheet.Cells[6, 7].Value = " * 汽車";
            sheet.Cells[6, 1, 6, 6].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            int RowIndex = 7;
            var ReList = this.GetReWork();
            foreach (var o in ReList)
            {
                sheet.Cells[RowIndex, 5, RowIndex, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                sheet.Cells[RowIndex, 1, RowIndex, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                sheet.Cells[RowIndex, 1].Value = o.OrgName;
                sheet.Cells[RowIndex, 2].Value = o.PsnNo;
                sheet.Cells[RowIndex, 3].Value = o.PsnName;
                sheet.Cells[RowIndex, 4].Value = o.Note;
                sheet.Cells[RowIndex, 4].Style.WrapText = true;
                sheet.Cells[RowIndex, 5].Value = Math.Round(o.ItinAmount, 2, MidpointRounding.AwayFromZero);    // string.Format("{0:0.00}", o.ItinAmount);
                sheet.Cells[RowIndex, 6].Value = Math.Round(o.Amount, 1, MidpointRounding.AwayFromZero);        // string.Format("{0:0.0}", o.Amount);
                sheet.Cells[RowIndex, 5, RowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[RowIndex, 7].Value = o.TravelMode.Equals("Driving") ? "*" : "";
                RowIndex++;
            }
            sheet.Cells[RowIndex, 4].Value = "總計：";
            sheet.Cells[RowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 5].Value = Math.Round(ReList.Sum(i => i.ItinAmount), 2, MidpointRounding.AwayFromZero);  // string.Format("{0:0.00}", this.ListLog.Sum(i => i.Distance));
            sheet.Cells[RowIndex, 6].Value = Math.Round(ReList.Sum(i => i.Amount), 1, MidpointRounding.AwayFromZero); // string.Format("{0:0.0}", this.ListLog.Sum(i => i.Distance * Price));
            sheet.Cells[RowIndex, 5, RowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 1, RowIndex, 6].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //sheet.Cells.AutoFitColumns();
            sheet.Column(1).Width = 15;
            sheet.Column(2).Width = 10;
            sheet.Column(3).Width = 10;
            sheet.Column(4).Width = 60;            
            sheet.Column(5).Width = 10;
            sheet.Column(6).Width = 10;
            sheet.Column(7).Width = 10;
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
            Response.AddHeader("content-disposition", "attachment; filename=ItineraryDay.xlsx");
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