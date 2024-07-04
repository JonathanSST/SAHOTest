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

namespace SahoAcs.Web._06._0661
{
    public partial class QueryAbnormalDetail : System.Web.UI.Page
    {

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<B03WorkAbnormal> ListLog = new List<B03WorkAbnormal>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public string txt_CardNo_PsnName = "";
        public string SortName = "WorkDate";
        public string SortType = "ASC";
        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;
        public string PsnNo = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        public string AuthList = "";
        public IPagedList<B03WorkAbnormal> PagedList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Sa.Web.Fun.GetSessionStr(this, "PsnNo").Equals("") || Sa.Web.Fun.GetSessionStr(this,"UserID").Equals("User"))
            {
                Response.Redirect("~/MainForm.aspx");
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetQueryData();
                this.ExportExcel();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryPerson")
            {
                this.SetQueryPerson();
            }
            else
            {
                this.SetInitData();
                if (Request.UrlReferrer != null && (Request.UrlReferrer.PathAndQuery.Contains("Default.aspx")||Request.UrlReferrer.PathAndQuery.Equals("/")))
                {
                    //Response.Write(Request.UrlReferrer.PathAndQuery);
                    this.SetQueryData();
                }
            }           
            ClientScript.RegisterClientScriptInclude("jsfun", "QueryAbnormalDetail.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScrip
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {

            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"
                                                        select A.* from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID = this.hideUserID.Value }).OrderBy(i => i.PsnName).ToList();
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT ISNULL(MAX(LogDate),GETDATE()) FROM B03_WorkAbnormal WHERE LogDate <= @ZoneTime", new { ZoneTime = this.GetZoneTime().ToString("yyyy/MM/dd") });
            this.PsnNo = Sa.Web.Fun.GetSessionStr(this, "PsnNo");
            if (dtLastCardTime >= this.GetZoneTime())
            {
                this.CardDayS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            else
            {
                this.CardDayS.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            #endregion

        }

        private void SetQueryPerson()
         {
            if (Request["PsnName"] != null && !string.IsNullOrEmpty(Request["PsnName"]))
            {
                //string PsnName = Request["PsnName"];
                string sqlcmd = "SELECT TOP 200 * FROM B01_Person WHERE (PsnNo LIKE @Key or PsnName Like @Key) ORDER BY PsnNo";
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = Request["PsnName"] + "%" }).ToList();
                //sqlcmd = "SELECT TOP 200 * FROM B01_Person WHERE PsnName LIKE @Key ORDER BY PsnName";
                //this.PersonList = this.PersonList.Concat(this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = Request["PsnName"] + "%" })).ToList();
            }
            else
            {
                this.PersonList = this.odo.GetQueryResult<PersonEntity>("SELECT TOP 200 * FROM B01_Person WHERE PsnNo LIKE '%' ORDER BY PsnNo").ToList();
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
            
            StringBuilder sql = new StringBuilder();
            string DateS = (Request["CardDateS"] ?? this.CardDayS.DateValue)+ " 00:00:00";
            string DateE = (Request["CardDateE"] ?? this.CardDayE.DateValue)+ " 23:59:59";
            string PsnName = Request["PsnName"] ?? "";

            sql.Append(
                  @"SELECT 
                    A.RecordID,
                    A.WorkDate,
                    A.PsnNo,
                    A.ClassNo,
                    A.WorkTimeS,
                    A.WorkTimeE,
                    A.RealTimeS,
                    A.RealTimeE,
                    A.WorkTimeO,
                    A.WorkTimeI,
                    A.RestTimeO,
                    A.RestTimeI,
                    A.Delay,
                    A.StealTime,
                    A.OverTime,
                    A.StatuDesc,
                    A.AbnormalDesc,
                    A.IsSend,
                    A.CreateUserID,
                    A.CreateTime,
                    A.UpdateUserID,
                    A.UpdateTime ,
                    B.PsnName
                    From B03_WorkAbnormal A 
                    Inner join B01_Person B On A.PsnNo = B.PsnNo
                    WHERE 1=1 ");

            if (!string.IsNullOrEmpty(DateS))
            {
                sql.Append(" and A.WorkDate >= Convert(Varchar(10),@DateS, 111) ");
            }
            else
            {
                sql.Append(" and A.WorkDate >= convert(varchar(10),Getdate(), 111) ");
            }

            if (!string.IsNullOrEmpty(DateE))
            {
                sql.Append(" and A.WorkDate <= Convert(varchar(10),@DateE, 111) ");
            }
            else
            {
                sql.Append(" and A.WorkDate <= convert(varchar(10),Getdate(), 111) ");
            }

            if (!string.IsNullOrEmpty(PsnName))
            {
                sql.Append(" AND PsnName like @PsnName");
            }


            sql.Append(" And A.AbnormalDesc != '' ");
            sql.Append(" ORDER BY A.PsnNo,A.WorkDate ");
            this.ListLog = this.odo.GetQueryResult<B03WorkAbnormal>(sql.ToString(), new
            {
                DateS = DateS,
                DateE = DateE,
                PsnName = PsnName + "%"
            }).OrderByField("WorkDate", false).ToList();

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
                var DateS = Request["CardDateS"] ?? this.CardDayS.DateValue;
                var DateE = Request["CardDateE"] ?? this.CardDayE.DateValue;
                var PageData = this.ListLog.ToPagedList(1, 30);
                var PsnNoList = this.ListLog.Select(i => i.PsnNo).Distinct<string>().ToList();
                foreach (var p in PsnNoList)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>打卡異常紀錄表</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>打卡時間區間：{0}~{1}</td><td></td></tr></table>", DateS, DateE, ""));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now, p));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:6%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:6%'>工號</td>");
                    sb.Append("<td class='DataBar' style='width:6%'>最後刷卡日期時間</td>");
                    //sb.Append("<td class='DataBar' style='width:6%'></td>");                   
                    sb.Append("<td class='DataBar' style='width:6%'>異常原因</td></tr>");
                    foreach (var o in this.ListLog.Where(i => i.PsnNo.Equals(p)))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkDate + "   " + o.RealTimeS + "~" + o.RealTimeE));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.LogTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.AbnormalDesc));                        
                    }
                    sb.Append("</table>");
                    sb.Append("總筆數：" + this.ListLog.Count);
                    if (PsnNoList.IndexOf(p) < PsnNoList.Count)
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
            Response.AddHeader("content-disposition", "attachment;filename=打卡異常紀錄表.pdf");
            Response.End();
        }
    }
}