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
    public partial class QueryWorkAbnormal : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<B03WorkDetail> ListLog = new List<B03WorkDetail>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public string txt_CardNo_PsnName = "";

        public string SortName = "WorkDate";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;
        public string PsnNo = "", PsnID = "", PsnName = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;        
        public string AuthList = "";
        public IPagedList<B03WorkDetail> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                //this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                //this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "QueryPerson")
            {
                this.SetQueryPerson();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryOneLog")
            {
                
            }
            else
            {
                this.SetInitData();
                if (Request.UrlReferrer != null && (Request.UrlReferrer.PathAndQuery.Contains("Default.aspx") || Request.UrlReferrer.PathAndQuery.Equals("/")))
                {
                    this.SetQueryData();
                }
            }
            if (!IsPostBack)
            {
                this.AuthList = Sa.Web.Fun.GetSessionStr(this, "FunAuthSet");
                if (WebAppService.GetSysParaData("IsShowMap") == "1")
                {
                    this.AuthList += ",ShowMap";
                }
            }
            ClientScript.RegisterClientScriptInclude("jsfun", "QueryWorkAbnormal.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            //查詢組織相關資料
            this.OrgDataInit = this.odo.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>("SELECT * FROM OrgStrucAllData('Unit') WHERE OrgNo<>''").ToList();
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new {UserID= this.hideUserID.Value }).OrderBy(i => i.PsnName).ToList();
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT ISNULL(MAX(WorkDate),GETDATE()) FROM B03_WorkDetail WHERE WorkDate <= @ZoneTime", new {ZoneTime=this.GetZoneTime().ToString("yyyy/MM/dd")});
            this.PsnNo = Sa.Web.Fun.GetSessionStr(this, "PsnNo");
            
            if (dtLastCardTime >= this.GetZoneTime())
            {
                this.CardDayS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            else
            {
                this.CardDayS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            #endregion

        }


        /// <summary>暫時用不到</summary>
        private void SetQueryPerson()
        {
            if(Request["PsnName"]!=null && !string.IsNullOrEmpty(Request["PsnName"]))
            {
                string sqlcmd = "SELECT TOP 200 * FROM B01_Person WHERE PsnNo LIKE @Key ORDER BY PsnNo";
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = Request["PsnName"] + "%" }).ToList();
                sqlcmd = "SELECT TOP 200 * FROM B01_Person WHERE PsnName LIKE @Key ORDER BY PsnName";
                this.PersonList = this.PersonList.Concat(this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = Request["PsnName"] + "%" })).ToList();
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
            
            string sql = "";
            PsnNo = Sa.Web.Fun.GetSessionStr(this, "PsnNo");
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            if (!PsnNo.Equals(""))
            {
                sql = @"SELECT * FROM B03_WorkDetail WHERE WorkDate BETWEEN @DateS AND @DateE  AND PsnNo = @PsnNo ORDER BY PsnNo,WorkDate";
            }
            else
            {
                sql = @"SELECT WD.*,P.PsnName FROM B03_WorkDetail WD
                                   INNER JOIN B01_Person P ON WD.PsnNo=P.PsnNo
                                  INNER JOIN B01_MgnOrgStrucs B ON P.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                WHERE WorkDate BETWEEN @DateS AND @DateE  AND UserID=@UserID AND StatuDesc='異常' ";
            }
            if (!this.GetFormEndValue("PsnName").Equals(""))
            {
                PsnName = this.GetFormEndValue("PsnName");
                sql += " AND (WD.PsnNo LIKE @PsnName OR PsnName LIKE @PsnName)";
            }
            if (!this.GetFormEndValue("DeptList").Equals(""))
            {
                sql += " AND P.OrgStrucId=@OrgID";
            }
            sql += " ORDER BY PsnNo,WorkDate";
            /* DATEDIFF(HOUR,InTime2nd,ISNULL(OutTime2nd,GETDATE()))>1 */
            this.ListLog = this.odo.GetQueryResult<B03WorkDetail>(sql, new
            {
                DateS = Request["DateS"] ?? this.CardDayS.DateValue,
                DateE = Request["DateE"] ?? this.CardDayE.DateValue,
                PsnName = PsnName + "%", UserID,
                OrgID=this.GetFormEndValue("DeptList")
            }).OrderByField("WorkDate",true).ToList();
        
            //this.ListLog = TempData.Where(i=>i.InTime2nd.;            
            if (Request["PageEvent"] == "Print")
            {                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {                
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 15);
            }
        }

        private void ExportExcel()
        {
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    font-size:10pt
                } 
                .TitleBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                }
                .FootBar
                {
                    border-top-style:solid; border-top-width:1px; border-top-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");
         
            if (this.ListLog.Count > 0)
            {
                var PageData = this.ListLog.ToPagedList(1, 15);
                var PsnNoList = this.ListLog.Select(i => i.PsnNo).Distinct<string>().ToList();
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"
                                                        select A.* from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID = Sa.Web.Fun.GetSessionStr(this,"UserID") }).OrderBy(i => i.PsnNo).ToList();
                //this.PagedList = this.ListLog.ToPagedList(1, 15);
                for(int i=1; i<=PageData.PageCount; i++)
                {
                    this.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName");
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>差　勤　異　常　明　細　表</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}，   頁次：{1}</td></tr></table>", DateTime.Now, i));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='TitleBar' style='width:7%'>工號</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>姓名</td>");
                    sb.Append("<td class='TitleBar' style='width:9%'>日期</td>");
                    sb.Append("<td class='TitleBar' style='width:6%'>班別</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>刷上</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>刷下</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>遲到</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>早退</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>狀態</td>");
                    sb.Append("<td class='TitleBar' style='width:17%'>備註</td></tr>");
                    foreach (var o in this.ListLog.ToPagedList(i,15))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkDate));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.ClassNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RealTimeS)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RealTimeE));    //6
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Delay));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.StealTime));    //12
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.StatuDesc));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.WorkDesc));    //14                        
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
            Response.AddHeader("content-disposition", "attachment;filename=差勤異常明細表.pdf");
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