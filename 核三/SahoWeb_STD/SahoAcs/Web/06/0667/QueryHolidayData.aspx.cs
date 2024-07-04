using DapperDataObjectLib;
using PagedList;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace SahoAcs.Web._06._0667
{
    public partial class QueryHolidayData : System.Web.UI.Page
    {
        #region Global block
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public string SortName = "StartTime";
        public string SortType = "ASC";
        public string PsnName = "";
        public List<B03PsnHoliday> ScheduleDatas = new List<B03PsnHoliday>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        #region 分頁參數
        public int PageIndex = 1;
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<B03PsnHoliday> PagedList;
        #endregion End 分頁參數


        #endregion end Global block
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                //按下查詢時 query 結果
                //this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                //按下查詢時 query 結果
                //this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();

            }
            else
            {
                this.QueryTimeS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                this.QueryTimeE.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
                this.PagedList = this.ScheduleDatas.ToPagedList(PageIndex, 100);
            }
        
            ClientScript.RegisterClientScriptInclude("JsInclude1", "QueryHolidayData.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion



            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "2道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 104, TitleWidth = 100, TitleName = "2道出廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "1道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 104, TitleWidth = 100, TitleName = "人員ID" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 124, TitleWidth = 120, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 124, TitleWidth = 120, TitleName = "廠商名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "廠商編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 124, TitleWidth = 120, TitleName = "工程編號" });
            foreach (var o in this.ListCols.Where(i => i.DataRealName == null || i.DataRealName == ""))
                o.DataRealName = o.ColName;

            #endregion         
        }

        private void SetQueryData()
        {
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }

            string QueryPsnsql = @"Select A.*,P.PsnName,VName AS HoliNo FROM B03_PsnHoliday A 
                                                Inner JOIN B01_Person P ON A.PsnNo=P.PsnNo 
                                                Inner JOIN B00_VacationData V ON A.HoliNo=V.VNo 
                                                Where (PsnName LIKE @PsnNo OR A.PsnNo LIKE @PsnNo) 
                                                And ((StartTime BETWEEN @QryTimeS AND @QryTimeE) OR (EndTime BETWEEN @QryTimeS AND @QryTimeE))";
            //bool boolSort = this.SortType.Equals("ASC");
            QueryPsnsql += " Order by A.StartTime desc ";
            this.ScheduleDatas = this.odo.GetQueryResult<B03PsnHoliday>(QueryPsnsql,
               new
               {
                   PsnNo = Request["PsnNo"] + "%",
                   QryTimeS = Request["QryTimeS"],
                   QryTimeE = Request["QryTimeE"]
               }).ToList();
            //this.ScheduleDatas.ForEach(i => i.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName"));
            this.PagedList = this.ScheduleDatas.ToPagedList(PageIndex, 100);
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
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");

            if (this.ScheduleDatas.Count > 0)
            {
                var PsnNoList = this.ScheduleDatas.Select(i => i.PsnNo).Distinct<string>().ToList();

                foreach (var p in PsnNoList)
                {

                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>員工差假明細表</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    //sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>打卡時間區間：{0}~{1}</td><td></td></tr></table>", Request["DateS"], Request["DateE"], ""));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now, p));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>使用者代號：{0}</td></tr></table>", p));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:10%'>人員編號</td>");
                    sb.Append("<td class='DataBar' style='width: 10%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:16%'>起始時間</td>");
                    sb.Append("<td class='DataBar' style='width:16%'>結束時間</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>單位編號</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>假別</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>天數</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>時數</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>備註</td></tr>");
                    foreach (var o in this.ScheduleDatas.Where(i => i.PsnNo.Equals(p)))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.StartTime.ToString("yyyy/MM/dd HH:mm:ss")));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EndTime.ToString("yyyy/MM/dd HH:mm:ss")));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.OrgNo));    //6
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.HoliNo)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Daily)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Hours)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", ""));
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
            Response.AddHeader("content-disposition", "attachment;filename=員工差假明細表.pdf");
            Response.End();
        }


        protected List<string> FinduserMgarea(string CurrentSysid)
        {
            var OrgMgn = this.odo.GetQueryResult("select a.UserID,a.MgaID,c.OrgStrucID from B00_SysUserMgns a inner join B00_ManageArea b on a.MgaID = b.MgaID inner join B01_MgnOrgStrucs c on b.MgaID = c.MgaID");
            var Orgfind = OrgMgn.Where(x => x.UserID == CurrentSysid).ToList();
            List<string> OrgIDdatas = new List<string>();
            for (int i = 0; i < Orgfind.Count; i++)
            {
                OrgIDdatas.Add(Convert.ToString(Orgfind[i].OrgStrucID));
            }
            return OrgIDdatas;
        }


    }
}