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
    public partial class QueryHolidayDetail : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<B03PsnHoliday> ListLog = new List<B03PsnHoliday>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<dynamic> VacationList = new List<dynamic>();
        public string txt_CardNo_PsnName = "";

        public string SortName = "StartTime";
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
        public IPagedList<B03PsnHoliday> PagedList;

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
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            if (!IsPostBack)
            {
                this.AuthList = Sa.Web.Fun.GetSessionStr(this, "FunAuthSet");            
            }
            ClientScript.RegisterClientScriptInclude("jsfun", "QueryHolidayDetail.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new {UserID= this.hideUserID.Value }).OrderBy(i => i.PsnName).ToList();
            #endregion
            //查詢組織相關資料
            this.OrgDataInit = this.odo.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>("SELECT * FROM OrgStrucAllData('Unit') WHERE OrgNo<>''").ToList();
            //查詢假別相關資料
            this.VacationList = this.odo.GetQueryResult("SELECT * FROM B00_VacationData ORDER BY VNo").ToList();
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
            //PsnNo = Sa.Web.Fun.GetSessionStr(this, "PsnNo");
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            if (!PsnNo.Equals(""))
            {
                sql = @"SELECT * FROM B03_PsnHoliday WHERE (StartTime BETWEEN @DateS AND @DateE) OR (EndTime BETWEEN @DateS AND @DateE) ORDER BY PsnNo,WorkDate";
            }
            else
            {
               
            }
            sql = @"SELECT WD.*,VName AS HoliNo,PsnName FROM B03_PsnHoliday  WD
                                   INNER JOIN B01_Person P ON WD.PsnNo=P.PsnNo
                                  INNER JOIN B01_MgnOrgStrucs B ON P.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                LEFT JOIN B00_VacationData V ON HoliNo=VNo
                                WHERE  ((StartTime BETWEEN @DateS AND @DateE) OR (EndTime BETWEEN @DateS AND @DateE))  AND UserID=@UserID ";
            if (!this.GetFormEndValue("PsnName").Equals(""))
            {
                PsnName = this.GetFormEndValue("PsnName");
                sql += " AND (WD.PsnNo LIKE @PsnName OR PsnName LIKE @PsnName)";
            }
            if (!this.GetFormEndValue("DeptList").Equals(""))
            {
                sql += " AND P.OrgStrucId=@OrgID";
            }
            if (!this.GetFormEndValue("VNoData").Equals(""))
            {
                sql += " AND V.VNo=@VNo";
            }
            sql += " ORDER BY PsnNo,StartTime";
            /* DATEDIFF(HOUR,InTime2nd,ISNULL(OutTime2nd,GETDATE()))>1 */
            this.ListLog = this.odo.GetQueryResult<B03PsnHoliday>(sql, new
            {
                DateS = Request["DateS"] ?? this.CardDayS.DateValue,
                DateE = Request["DateE"] ?? this.CardDayE.DateValue,
                PsnNo, UserID, PsnName=this.GetFormEndValue("PsnName")+"%",
                OrgID = this.GetFormEndValue("DeptList"), VNo=this.GetFormEndValue("VNoData")
            }).OrderByField(this.SortName, true).ToList();
        
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
                    font-size:10pt
                } 
                .TitleBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                }
                .FootBar
                {
                    border-top-style:solid; border-top-width:1px; border-top-color:Black;width:70px;font-size:10pt
                }
                </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");
         
            if (this.ListLog.Count > 0)
            {
                var PageData = this.ListLog.ToPagedList(1, 30);
                var PsnNoList = this.ListLog.Select(i => i.PsnNo).Distinct<string>().ToList();
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"
                                                        select A.* from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID = Sa.Web.Fun.GetSessionStr(this,"UserID") }).OrderBy(i => i.PsnName).ToList();                
                foreach (var p in PsnNoList)
                {
                    if (this.PersonList.Count > 0 && this.PersonList.Where(i=>i.PsnNo.Equals(p)).Count()>0)
                    {
                        var psn_data = this.PersonList.Where(i => i.PsnNo.Equals(p)).First();
                        this.PsnName = psn_data.PsnNo + "　" + psn_data.PsnName;
                    }
                    else
                    {
                        this.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName");
                    }
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>請　假　明　細　表</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    //sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>打卡時間區間：{0}~{1}</td><td></td></tr></table>", Request["DateS"], Request["DateE"], ""));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now, p));
                    sb.Append(string.Format("<table style='width:100%;font-size:12pt'><tr><td style='width:50%'>使用者代號及姓名：{0}</td></tr></table>", this.PsnName));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='TitleBar' style='width:10%'>工號</td>");
                    sb.Append("<td class='TitleBar' style='width:8%'>姓名</td>");
                    sb.Append("<td class='TitleBar' style='width:15%'>起始日期</td>");
                    sb.Append("<td class='TitleBar' style='width:15%'>結束日期</td>");
                    sb.Append("<td class='TitleBar' style='width:10%'>假別</td>");
                    //sb.Append("<td class='TitleBar' style='width:10%'>啟始時間</td>");
                    //sb.Append("<td class='TitleBar' style='width:10%'>結束時間</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>請假天數</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>請假時數</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>請假分鐘</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>備註</td></tr>");
                    foreach (var o in this.ListLog.Where(i => i.PsnNo.Equals(p)))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));        //1
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));        
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:yyyy/MM/dd HH:mm}</td>", o.StartTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:yyyy/MM/dd HH:mm}</td>", o.EndTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.HoliNo));//4
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:HH:mm}</td>", o.StartTime));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:HH:mm}</td>", o.EndTime)); //6
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:0}</td>", double.Parse(o.Daily)));    //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:0}</td>", o.Hours));    //6
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:0}</td>", o.Minutes));    //7
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", ""));    //8           
                    }
                    /*
                    sb.Append("<tr><td class='FootBar' style='width:10%'>小計</td>");
                    sb.Append("<td class='FootBar' style='width:10%'></td>");
                    sb.Append("<td class='FootBar' style='width:10%'></td>");
                    sb.Append("<td class='FootBar' style='width:10%'></td>");
                    sb.Append("<td class='FootBar' style='width:10%'></td>");
                    var hours = this.ListLog.Where(i => i.PsnNo.Equals(p)).Sum(i => double.Parse(i.Hours));
                    var daily = this.ListLog.Where(i => i.PsnNo.Equals(p)).Sum(i => double.Parse(i.Daily));
                    sb.Append(string.Format("<td class='FootBar' style='width:7%'>{0}</td>", daily));
                    sb.Append(string.Format("<td class='FootBar' style='width:7%'>{0}</td>", hours));
                    sb.Append("<td class='FootBar' style='width:7%'></td></tr>");
                    */
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
            Response.AddHeader("content-disposition", "attachment;filename=請假明細表.pdf");
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