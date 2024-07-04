using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using PagedList;
using OfficeOpenXml;
using iTextSharp;
using iTextSharp.text;


namespace SahoAcs.Web._0677
{
    public partial class PsnCardDblRecord : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> PersonList = new List<CardLogModel>();
        public List<EquGroupData> EquList = new List<EquGroupData>();
        public DataTable DataResult = new DataTable();
        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public string txt_CardNo_PsnName = "";

        public string PsnID = "";

        public string SortName = "PsnNo";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        
        public IPagedList<CardLogModel> PagedList;

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
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Pdf")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportPdf();
            }
            else
            {
                this.SetInitData();
                //this.EmptyCondition();
                this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            }
            ClientScript.RegisterClientScriptInclude("IncludeScript", "PsnCardDblRecord.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            this.Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            this.Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");

            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttResult").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataRealName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "人員編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 144, TitleWidth = 140, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 144, TitleWidth = 140, TitleName = "卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardDate", DataWidth = 144, TitleWidth = 140, TitleName = "日期" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardQty", DataWidth = 144, TitleWidth = 140, TitleName = "重複次數" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 144, TitleWidth = 140, TitleName = "燈號狀態" });              
                                                
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            CreateGroupList();
        }

        private void CreateGroupList()
        {
            
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
            bool boolSort = this.SortType.Equals("ASC");
            foreach (var s in Request.Form.GetValues("ColName"))
            {                
                collist.Add(this.ListCols.Where(i => i.ColName == s).FirstOrDefault());
            }          
            this.ListCols = collist;            
            string sql  = @"SELECT *,Convert(varchar,CardTime,111) AS CardDate FROM B01_CardLog WHERE
                CardTime BETWEEN CONVERT(DATETIME, @DateS) AND CONVERT(DATETIME,@DateE)
                AND (PsnName LIKE @PsnNo OR CardNo LIKE @PsnNo OR PsnNo LIKE @PsnNo)";
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
           
            this.PersonList = this.odo.GetQueryResult<CardLogModel>(sql, 
                new {PsnNo=this.GetFormEqlValue("PsnNo")+"%", DateS=this.GetFormEqlValue("DateS"), DateE=this.GetFormEqlValue("DateE") }).OrderByField(SortName, SortType.Equals("ASC")).ToList();
            var groupresult = from g in this.PersonList
                              group g by new { g.PsnNo, g.LogStatus, g.CardNo, g.CardDate, g.PsnName }
                                               into groups
                              select new CardLogModel
                              {
                                  CardNo = groups.Key.CardNo,
                                  PsnName = groups.First().PsnName,
                                  LogStatus = logstatus.Where(i=>Convert.ToString(i.Code).Equals(groups.Key.LogStatus)).First().StateDesc,
                                  CardDate = groups.Key.CardDate,                                  
                                  PsnNo = groups.First().PsnNo,
                                  CardQty = groups.Count()-1
                                  //ITEM_NAME = groups.Key.CsdItemName,
                                  //Price = groups.Average(s => s.CsdSalePrice),
                                  //Amount = groups.Average(s => s.CsdAmount)
                              };
            this.PersonList = groupresult.Where(i=>i.CardQty>0).ToList();
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.PersonList);
            }
            else
            {
                //轉datatable
                this.PagedList = this.PersonList.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }
        }
        
        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");


            for (int i = 0; i < this.ListCols.Count; i++)
            {
                ws.Cells[1, i + 1].Value = this.ListCols[i].TitleName;
            }

            //Content
            for (int i = 0; i < this.DataResult.Rows.Count; i++)
            {
                for (int col = 0; col < this.ListCols.Count(); col++)
                {
                    ws.Cells[i + 2, col + 1].Value = this.DataResult.Rows[i][this.ListCols[col].ColName].ToString();
                }
            }
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=0601.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.PersonList.Add(new DBModel.CardLogModel()
                {
                    
                });
            }
            //轉datatable
            this.PagedList = this.PersonList.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
        }



        private void ExportPdf()
        {
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                }
                span {
                    background-color:#ffcc00;
                    display:-moz-inline-box;
                    display:inline-block;
                    width:150px;
                }
                </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");
            string EquGrpName = "";
            if (!string.IsNullOrEmpty(this.GetFormEndValue("ddlEquGrpNo")))
            { 
                foreach(var o in this.odo.GetQueryResult("SELECT * FROM B01_EquGroup WHERE EquGrpID=@GrpID", new {GrpID=this.GetFormEndValue("ddlEquGrpNo")}))
                {
                    EquGrpName = string.Format("{0}({1})", o.EquGrpName, o.EquGrpNo);
                }
            }
            if (this.PersonList.Count > 0)
            {
                var PageData = this.PersonList.ToPagedList(1, 25);
                for(int i=1;i<=PageData.PageCount;i++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>重複打卡紀錄查詢</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));                    
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:30%'>頁數：{0}/{1}</td><td style='width:50%'>打卡日期區間：{2}~{3}</td><td style='width:50%'>人員編號、卡號：{4}</td></tr></table>"
                        , i,PageData.PageCount, this.GetFormEqlValue("DateS"), this.GetFormEqlValue("DateE"), this.GetFormEqlValue("PsnNo")));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:12%'>人員編號</td>");
                    sb.Append("<td class='DataBar' style='width:13%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>卡號</td>");                    
                    sb.Append("<td class='DataBar' style='width:15%'>打卡日期</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>燈號狀態</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>重複次數</td></tr>");
                    foreach (var o in this.PersonList.ToPagedList(i,25))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardNo));                        
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardDate));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.LogStatus));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.CardQty));                        
                    }
                    sb.Append("</table>");
                    sb.Append("<p style='page-break-after:always'>&nbsp;</p>");
                }
            }
            else
            {
                sb.Append("查無資料");
            }
            sb.Append("</body></html>");
            Response.Clear();
            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            MemoryStream msInput = new MemoryStream(data);
            iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 5, 5, 5, 5);
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, Response.OutputStream);
            iTextSharp.text.pdf.PdfDestination pdfDest = new iTextSharp.text.pdf.PdfDestination(iTextSharp.text.pdf.PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            doc.Open();
            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new SahoAcs.UnicodeFontFactory());
            //將pdfDest設定的資料寫到PDF檔
            iTextSharp.text.pdf.PdfAction action = iTextSharp.text.pdf.PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            msInput.Close();
            //這裡控制Response回應的格式內容
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=重複刷卡紀錄.pdf");
            Response.End();
        }


    }//end page class
}//end namespace