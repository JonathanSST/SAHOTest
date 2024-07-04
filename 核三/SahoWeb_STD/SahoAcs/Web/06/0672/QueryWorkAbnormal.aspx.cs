using DapperDataObjectLib;
using PagedList;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

namespace SahoAcs.Web._06._0672
{
    public partial class QueryWorkAbnormal : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string SortName = "WorkDate";
        public string SortType = "ASC";
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<B03WorkAbnormal> ListLog = new List<B03WorkAbnormal>();

        public int PageIndex = 1;
        public string PsnNo = "", PsnID = "", PsnName = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string AuthList = "";
        public IPagedList<B03WorkAbnormal> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryPerson")
            {
                this.SetQueryPerson();
            }
            else if(Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
                if (Request.UrlReferrer != null && (Request.UrlReferrer.PathAndQuery.Contains("Default.aspx") || Request.UrlReferrer.PathAndQuery.Equals("/")))
                {
                    this.SetQueryData();
                }
            }
            ClientScript.RegisterClientScriptInclude("jsfun", "QueryWorkAbnormal.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript
        }

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

            this.OrgDataInit = this.odo.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>("SELECT * FROM OrgStrucAllData('Unit') WHERE OrgNo<>''").ToList();

            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT ISNULL(MAX(WorkDate),GETDATE()) FROM B03_WorkAbnormal WHERE WorkDate <= @ZoneTime", new { ZoneTime = this.GetZoneTime().ToString("yyyy/MM/dd") });
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
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
        }
        private void SetQueryPerson()
        {
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
            if (Request["PsnName"] != null && !string.IsNullOrEmpty(Request["PsnName"]))
            {
                string sqlcmd = "SELECT TOP 200 * FROM B01_Person WHERE (PsnNo LIKE @Key or PsnName Like @Key) ORDER BY PsnNo";
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = Request["PsnName"] + "%" }).ToList();
            }
            else
            {
                this.PersonList = this.odo.GetQueryResult<PersonEntity>("SELECT TOP 200 * FROM B01_Person WHERE PsnNo LIKE '%' ORDER BY PsnNo").ToList();
            }

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
            string DateS = (Request["DateS"] ?? this.CardDayS.DateValue);
            string DateE = (Request["DateE"] ?? this.CardDayE.DateValue);
            string PsnName = this.GetFormEqlValue("PsnName") + "%";
            string PsnNo = Request["PsnNo"] ?? "";
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            if (!PsnNo.Equals(""))
            {
                sql.Append("SELECT * FROM B03_WorkAbnormal WHERE WorkDate BETWEEN @DateS AND @DateE  AND PsnNo = @PsnNo ORDER BY PsnNo,WorkDate");
            }
            else
            {
                sql.Append("SELECT DISTINCT WD.*,P.PsnName FROM B03_WorkAbnormal WD ");
                sql.Append(" INNER JOIN B01_Person P ON WD.PsnNo=P.PsnNo ");
                sql.Append(" INNER JOIN B01_MgnOrgStrucs B ON P.OrgStrucID=B.OrgStrucID ");
                sql.Append(" INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID ");
                sql.Append(" INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID ");
                sql.Append(" WHERE WorkDate BETWEEN @DateS AND @DateE  AND UserID=@UserID");
            }

            if (!this.GetFormEndValue("PsnName").Equals(""))
            {
                PsnName = this.GetFormEqlValue("PsnName") + "%";
                sql.Append(" AND (WD.PsnNo LIKE @PsnName OR PsnName LIKE @PsnName) ");
            }
            if (!this.GetFormEndValue("DeptList").Equals(""))
            {
                sql.Append(" And P.OrgStrucId=@OrgID" );
            }
            
            sql.Append(" Order By PsnNo,WorkDate ");
            this.ListLog = this.odo.GetQueryResult<B03WorkAbnormal>(sql.ToString(), new
            {
                DateS = DateS,
                DateE = DateE,
                PsnName = PsnName,
                UserID = UserID,
                OrgID = this.GetFormEndValue("DeptList")
            }).OrderByField("WorkDate", true).ToList();

            //string _RealTimeS = string.Empty;
            //string _RealTimeE = string.Empty;
            //DateTime _WorkDate = DateTime.Now;

            //for (int i = 0; i < this.ListLog.Count; i++)
            //{
            //    if (!string.IsNullOrEmpty(this.ListLog[i].RealTimeS))
            //    {
            //        _RealTimeS = this.ListLog[i].RealTimeS.ToString();
            //    }
            //    if (!string.IsNullOrEmpty(this.ListLog[i].RealTimeE))
            //    {
            //        _RealTimeE = this.ListLog[i].RealTimeE.ToString();
            //    }
            //    if (!string.IsNullOrEmpty(_RealTimeS) && !string.IsNullOrEmpty(_RealTimeE))
            //    {
            //        DateTime dt1 = Convert.ToDateTime(_RealTimeS);
            //        DateTime dt2 = Convert.ToDateTime(_RealTimeE);
            //        if (DateTime.Compare(dt1, dt2) > 0)
            //        {
            //            _WorkDate = Convert.ToDateTime(this.ListLog[i].WorkDate).AddDays(-1);
            //            this.ListLog[i].WorkDate = _WorkDate.ToString("yyyy/MM/dd");
            //        }
            //    }
            //}

            if (Request["PageEvent"] == "Print")
            {
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

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

        private void ExportExcel()
        {
            //刷卡分析異常表
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");
            if (this.ListLog.Count > 0)
            {
                var PageData = this.ListLog.ToPagedList(1, 30);
                var PsnNoList = this.ListLog.Select(i => i.PsnNo).Distinct<string>().ToList();
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"
                                                        select DISTINCT A.* from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID",
                                                        new
                                                        {
                                                            UserID = Sa.Web.Fun.GetSessionStr(this, "UserID")
                                                        }).OrderBy(i => i.PsnName).ToList();
                foreach (var p in PsnNoList)
                {
                    if (this.PersonList.Count > 0 && this.PersonList.Where(i => i.PsnNo.Equals(p)).Count() > 0)
                    {
                        var psn_data = this.PersonList.Where(i => i.PsnNo.Equals(p)).First();
                        this.PsnName = psn_data.PsnNo + "/" + psn_data.PsnName;
                    }
                    else
                    {
                        this.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName");
                    }
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>刷卡分析異常表</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    //sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>打卡時間區間：{0}~{1}</td><td></td></tr></table>", Request["DateS"], Request["DateE"], ""));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now, p));
                    sb.Append(string.Format("<table style='width:100%;font-size:12pt'><tr><td style='width:50%'>使用者代號及姓名：{0}</td></tr></table>", this.PsnName));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:7%'>工號</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:9%'>工作日</td>");
                    sb.Append("<td class='DataBar' style='width:4%'>班別</td>");
                    sb.Append("<td class='DataBar' style='width:6%'>標上</td>");
                    sb.Append("<td class='DataBar' style='width:6%'>標下</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>刷上</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>刷下</td>");
                    //sb.Append("<td class='DataBar' style='width:7%'>標中上</td>");
                    //sb.Append("<td class='DataBar' style='width:7%'>標中下</td>");
                    //sb.Append("<td class='DataBar' style='width:7%'>刷中上</td>");
                    //sb.Append("<td class='DataBar' style='width:7%'>刷中下</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>遲到</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>早退</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>加班</td>");
                    sb.Append("<td class='DataBar' style='width:7%'>異常說明</td></tr>");
                    foreach (var o in this.ListLog.Where(i => i.PsnNo.Equals(p)))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkDate));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.ClassNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkTimeS));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkTimeE));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RealTimeS)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RealTimeE));    //6
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkTimeO));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkTimeI));    //8
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RestTimeO));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RestTimeI));    //10
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Delay));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.StealTime));    //12
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.OverTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.AbnormalDesc??o.StatuDesc));    //14 
                        //string Desc = "";
                        //if (o.RealTimeS == string.Empty || o.RealTimeE == string.Empty)
                        //{
                        //    Desc = "未打卡";
                        //}
                        //else if (o.RealTimeS.CompareTo("08:31:01") >= 0)
                        //{
                        //    Desc = "遲到";
                        //}
                        //else if (o.RealTimeE.CompareTo("18:26:01") >= 0)
                        //{
                        //    Desc = "下班時間超時";
                        //}
                        //else if (o.RealTimeS.CompareTo("07:30:00") <= 0)
                        //{
                        //    Desc = "提早上班打卡";
                        //}
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>",Desc));    //14 
                    }
                    sb.Append("</table>");
                    //sb.Append("總筆數：" + this.ListLog.Count);       //這裡改成加班統計欄
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
            Response.AddHeader("content-disposition", "attachment;filename=刷卡分析異常表.pdf");
            Response.End();
        }

    }
}