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
    public partial class AllItinerary : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<Itinerary> ListLog = new List<Itinerary>();
        public List<Itinerary> ListPsnData = new List<Itinerary>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<OrgDataEntity> OrgData = new List<OrgDataEntity>();
        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";
        double price = 8.5, price1 = 7.9;
        
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
            this.OrgData = this.odo.GetQueryResult<OrgDataEntity>(@"SELECT * FROM (SELECT OrgNo,OrgName,OrgStrucID,OrgID,(SELECT COUNT(*) FROM SplitString(OrgIDList,'\',1)) AS OrgArr
                                                                                                                            FROM OrgStrucAllData('Department') 
                                                                                                                            WHERE OrgNo <> '') AS T1 WHERE T1.OrgArr=2 ORDER BY OrgNo ").ToList();
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
        



        private void SetQueryData()
        {
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN @ParaNo", new { ParaNo = new string[] { "Itinerary", "Itinerary1" } }))
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
            
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B01_Person " + WhereSql, new { PsnID, PsnNo }))
            {
                this.PsnNo = Convert.ToString(o.PsnNo);
                this.PsnName = Convert.ToString(o.PsnName);
            }
            string sqlstr = @"SELECT A.*, StateDesc,O.OrgName FROM Itinerary A 
                                                INNER JOIN B01_Person P ON A.PsnNo=P.PsnNo
                                                INNER JOIN OrgStrucAllData('Department') O ON P.OrgStrucID=O.OrgStrucID
                                                INNER JOIN B00_CardLogState ON LogStatus=Code  AND Code BETWEEN 84 AND 89
                                                WHERE LocalTime BETWEEN @DateS AND @DateE ";
            if (this.Request.Form["DeptNo"] != null && this.Request.Form["DeptNo"] != "0")
            {
                sqlstr += " AND P.OrgStrucID IN @OrgStrucID ";
            }
            sqlstr += " ORDER BY OrgName,PsnName,CardTime ";
            this.ListLog = this.odo.GetQueryResult<Itinerary>(sqlstr, new
            {
                DateE = Request.Form["DateE"] + " 23:59:59",
                DateS = Request.Form["DateS"] + " 00:00:00",
                PsnNo,
                OrgStrucID = this.odo.GetQueryResult(@"SELECT A.OrgStrucID
                                                                                    FROM OrgStrucAllData('Department') A
                                                                                    INNER JOIN 
                                                                                    (SELECT OrgNo,OrgName,OrgStrucID,OrgID
                                                                                    FROM OrgStrucAllData('Department') 
                                                                                    WHERE OrgNo <> '' AND OrgStrucID=@Key1) AS B ON A.OrgID=B.OrgID",
                                                                                    new { Key1 = this.Request.Form["DeptNo"].ToString() }).Select(i=>i.OrgStrucID)}).ToList();           
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
            result = result.GroupBy(i => new { i.PsnNo}).Select(i => i.First()).ToArray().ToList();                        
            //以每個人的Group 結果做處理
            result.ForEach(i =>
            {
                i.ItinAmount = this.ListLog.Where(log=>log.PsnNo==i.PsnNo).Sum(log => log.Distance.Value);
                i.Amount = this.ListLog.Where(log => log.PsnNo == i.PsnNo && log.TravelMode.Equals("Driving")).Aggregate(seed: 0.0, func: (r_count, item) =>
                      {
                          return r_count + item.Distance.Value * price;
                      }) +
                      this.ListLog.Where(log => log.PsnNo == i.PsnNo && !log.TravelMode.Equals("Driving")).Aggregate(seed: 0.0, func: (r_count, item) =>
                      {
                          return r_count + item.Distance.Value * price1;
                      });

            });
            return result;
        }


        private void ExportExcel()
        {
            //建立Excel
            ExcelPackage ep = new ExcelPackage();
            //建立第一個Sheet，後方為定義Sheet的名稱
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("期間行程表");            
            string DateS = "", DateE = "";
            DateE = Request.Form["DateE"];
            DateS = Request.Form["DateS"];
            sheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 1, 2, 5].Merge = true;
            sheet.Cells[2, 1].Value = "期 間 行 程 查 詢";
            sheet.Cells[2, 1].Style.Font.Name = "標楷體";
            sheet.Cells[2, 1].Style.Font.Size = 16;
            sheet.Cells[4, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[4, 5].Value = string.Format("距離單位(公里)每單位補貼汽車({0})、機車({1})", this.price, this.price1);
            sheet.Cells[5, 1].Value = string.Format("打卡日期:{0}", DateS);
            sheet.Cells[5, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[5, 5].Value = string.Format("查詢時間:{0:yyyy/MM/dd HH:mm:ss}", this.GetZoneTime());
            sheet.Cells[6, 1].Value = "部門";
            sheet.Cells[6, 2].Value = "人員編號";
            sheet.Cells[6, 3].Value = "姓名";            
            sheet.Cells[6, 4].Value = "期間行程";
            sheet.Cells[6, 5].Value = "補貼費用";
            sheet.Cells[6, 1, 6, 5].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            int RowIndex = 7;
            var ReList = this.GetReWork();
            foreach (var o in ReList)
            {
                sheet.Cells[RowIndex, 1].Value = o.OrgName;
                sheet.Cells[RowIndex, 2].Value = o.PsnNo;
                sheet.Cells[RowIndex, 3].Value = o.PsnName;
                sheet.Cells[RowIndex, 4].Value = Math.Round(o.ItinAmount, 2, MidpointRounding.AwayFromZero);    // string.Format("{0:0.00}", o.ItinAmount);
                sheet.Cells[RowIndex, 5].Value = Math.Round(o.Amount, 1, MidpointRounding.AwayFromZero);    // string.Format("{0:0.0}", o.Amount);
                sheet.Cells[RowIndex, 4, RowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                RowIndex++;
            }
            sheet.Cells[RowIndex, 3].Value = "總計：";
            sheet.Cells[RowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 4].Value = Math.Round(ReList.Sum(i => i.ItinAmount), 2, MidpointRounding.AwayFromZero); // string.Format("{0:0.00}", this.ListLog.Sum(i => i.Distance.Value));
            sheet.Cells[RowIndex, 5].Value = Math.Round(this.ListLog.Sum(i => i.Amount), 1, MidpointRounding.AwayFromZero); // string.Format("{0:0.0}", this.ListLog.Sum(i => i.Distance.Value * Price));
            //sheet.Cells[RowIndex, 5].Formula = "SUM(E7:E" + (RowIndex-1) + ")";
            sheet.Cells[RowIndex, 4, RowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[RowIndex, 1, RowIndex, 5].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //sheet.Cells.AutoFitColumns();
            sheet.Column(1).Width = 15;
            sheet.Column(2).Width = 10;            
            sheet.Column(3).Width = 60;
            sheet.Column(4).Width = 10;
            sheet.Column(5).Width = 10;
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
            Response.AddHeader("content-disposition", "attachment; filename=AllItinerary.xlsx");
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