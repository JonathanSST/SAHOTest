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
using iTextSharp;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf;

namespace SahoAcs
{
    public partial class QueryPersonMobileLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<MobileLogModel> ListLog = new List<MobileLogModel>();

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
        public string PsnName = "";
        public string AuthList = "";
        public IPagedList<MobileLogModel> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
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
                this.SetOneCardLog();
            }
            else
            {                
                this.SetInitData();                                
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
            ClientScript.RegisterClientScriptInclude("0650", "QueryPersonMobileLog.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string PsnNo = "";
            if (PsnID != "" && PsnID != "0")
            {
                PsnNo = this.odo.GetStrScalar("SELECT TOP 1 PsnNo FROM B01_Person WHERE PsnID=@PsnID", new { PsnID = PsnID });
                this.PsnName = this.odo.GetStrScalar("SELECT TOP 1 PsnName FROM B01_Person WHERE PsnID=@PsnID", new { PsnID = PsnID });
            }
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM FillCardLog WHERE EmpID=@PsnNo ",new { PsnNo = PsnNo });

            if (dtLastCardTime == DateTime.MinValue)
            {
                this.CalendarS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd")+" 00:00:00";
                this.CalendarE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd")+" 23:59:59";
            }
            else
            {
                this.CalendarS.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                this.CalendarE.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
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
            string sql = "";
            sql = @"SELECT *,PsnName AS EmpName,ISNULL(CardPic,'') AS CardPic2,
                        ISNULL((SELECT TOP 1 StateDesc FROM B00_CardLogState WHERE CONVERT(VARCHAR,Code)=OpType),'') AS OpName
                        FROM FillCardLog INNER JOIN B01_Person ON EmpID=PsnNo WHERE CardTime BETWEEN @CardTimeS AND @CardTimeE AND PsnID=@PsnID ORDER BY CardTime DESC";
            /* DATEDIFF(HOUR,InTime2nd,ISNULL(OutTime2nd,GETDATE()))>1 */
            this.ListLog = this.odo.GetQueryResult<MobileLogModel>(sql, new
            {
                CardTimeS = Request["CardDateS"],
                CardTimeE = Request["CardDateE"],
                PsnID = Request["PsnID"]
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
        
        private void ExportExcel()
        {
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");
            if (this.ListLog.Count > 0)
            {
                var PageData = this.ListLog.ToPagedList(1, 30);
                for (int p = 1; p <= PageData.PageCount; p++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>個人行動打卡紀錄明細</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>打卡時間區間：{0}~{1}</td><td></td></tr></table>", Request["CardDateS"], Request["CardDateE"], ""));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>列印日期：{0:yyyy/MM/dd HH:mm:ss}</td><td>頁次：{1}</td></tr></table>", DateTime.Now, p));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:15%'>員工編號</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:20%'>打卡時間</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>打卡類型</td>");                    
                    sb.Append("<td class='DataBar' style='width:15%'>備註</td></tr>");
                    foreach (var o in this.ListLog.ToPagedList(p, 30))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>",o.EmpID));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EmpID));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EmpName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", string.Format("{0:yyyy/MM/dd HH:mm:ss}",o.CardTime)));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.OpName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.Note));
                    }
                    sb.Append("</table>");
                    sb.Append("總筆數：" + this.ListLog.Count);
                    if (p < PageData.PageCount)
                    {
                        sb.Append("<p style='page-break-after:always'>&nbsp;</p>");
                    }
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
            Response.AddHeader("content-disposition", "attachment;filename=個人讀卡記錄報表.pdf");
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