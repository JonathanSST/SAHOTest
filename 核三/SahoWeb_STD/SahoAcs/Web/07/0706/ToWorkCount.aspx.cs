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
using Sa.DB;

namespace SahoAcs.Web._07._0706
{
    public partial class ToWorkCount : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public int PageIndex = 1;
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public DataTable DataResult = new DataTable();
        public IPagedList<B03PsnHoliday> PagedList;
        public List<B03PsnHoliday> ListLog = new List<B03PsnHoliday>();
        public string SortName = "StartTime";
        public string SortType = "ASC";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryData")
            {
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetQueryData();
                this.Export();
            }
            else
            {
                this.SetInitData();
                this.QueryTimeS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                this.QueryTimeE.DateValue = DateTime.Now.AddDays(7).ToString("yyyy/MM/dd 23:59:59");
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
            }
         
            ClientScript.RegisterClientScriptInclude("jsfun", "ToWorkCount.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案

        }

        private void SetInitData()
        {
            dropCompany.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), Value = "" });
            dropDepartment.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), Value = "" });

            var CompanyResult = this.odo.GetQueryResult("SELECT OrgID, OrgNo, OrgName FROM B01_OrgData WHERE OrgClass = 'Company' and OrgName != '無組織'");
            if (CompanyResult.Count() != 0)
            {
                foreach (var c in CompanyResult)
                {
                    ListItem oItem = new ListItem();
                    oItem.Text = c.OrgName + "." + c.OrgNo;
                    oItem.Value = c.OrgNo.ToString();
                    this.dropCompany.Items.Add(oItem);
                }
            }

            var DepResult = this.odo.GetQueryResult("SELECT OrgID, OrgNo, OrgName FROM B01_OrgData WHERE OrgClass = 'Department'");
            if (DepResult.Count() != 0)
            {
                foreach (var d in DepResult)
                {
                    ListItem oItem = new ListItem();
                    oItem.Text = d.OrgName + "." + d.OrgNo;
                    oItem.Value = d.OrgNo.ToString();
                    this.dropDepartment.Items.Add(oItem);
                }
            }

            this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
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

        private void SetQueryData()
        {
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");

            string Sql = string.Empty;
            string StartTime = Request["StartTime"];
            string EndTime = Request["EndTime"];
            string Company = Request["Company"];
            string Department = Request["Department"];

            Sql =
                @"Select
OrgStrucAllData_0.OrgNo As CompanyNo,
OrgStrucAllData_0.OrgClass As CompanyClass,
OrgStrucAllData_0.OrgName As CompanyName,
OrgStrucAllData_1.OrgNo As DepartmentNo,
OrgStrucAllData_1.OrgClass As DepartmentClass,
OrgStrucAllData_1.OrgName As DepartmentName,
A.*,P.PsnName,VName AS HoliNo 
FROM B03_PsnHoliday A
Inner Join B01_Person P ON A.PsnNo=P.PsnNo 
Inner Join B00_VacationData V ON A.HoliNo=V.VNo 
Inner Join OrgStrucAllData('Company') AS OrgStrucAllData_0 ON P.OrgStrucID = OrgStrucAllData_0.OrgStrucID
Inner Join OrgStrucAllData('Department') AS OrgStrucAllData_1 ON P.OrgStrucID = OrgStrucAllData_1.OrgStrucID
Where 1=1 ";

            Sql += "And ((StartTime BETWEEN @StartTime AND @EndTime) OR(EndTime BETWEEN @StartTime AND @EndTime)) ";
            if (!string.IsNullOrEmpty(Company))
            {
                Sql += "And OrgStrucAllData_0.OrgNo='" + Company + "' ";
            }
            if (!string.IsNullOrEmpty(Department))
            {
                Sql += "And OrgStrucAllData_1.OrgNo='" + Department + "' ";
            }

            this.ListLog = this.odo.GetQueryResult<B03PsnHoliday>(Sql.ToString(), new
            {
                StartTime = StartTime,
                EndTime = EndTime
            }).OrderByField("PsnNo", true).ToList();

            if (Request["PageEvent"] == "Print")
            {
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

        private void Export()
        {
            DateTime sDate = Convert.ToDateTime(Request["StartTime"]);
            DateTime eDate = Convert.ToDateTime(Request["EndTime"]);

            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");

            if (this.ListLog.Count > 0)
            {
                var OrgNoList = this.ListLog.Select(i => i.OrgNo).Distinct<string>().ToList();

                foreach (var p in OrgNoList)
                {
                    string OrgSql = "Select distinct O.OrgName from OrgStrucAllData('Department') O Where O.OrgNo='" + p + "' ";
                    string OrgName = odo.GetStrScalar(OrgSql);
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>休假記錄明細表</div>");
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>單位代號名稱：{0}  {1}</td></tr></table>", p,OrgName));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>請假時間區間：{0}~{1}</td><td></td></tr></table>", sDate.ToString("yyyy/MM/dd"), eDate.ToString("yyyy/MM/dd"), ""));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:10%'>人員編號</td>");
                    sb.Append("<td class='DataBar' style='width: 10%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:21%'>起始時間</td>");
                    sb.Append("<td class='DataBar' style='width:21%'>結束時間</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>假別</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>天數</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>時數</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>備註</td></tr>");
                    foreach (var o in this.ListLog.Where(i => i.OrgNo.Equals(p)))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.StartTime.ToString("yyyy/MM/dd HH:mm")));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EndTime.ToString("yyyy/MM/dd HH:mm")));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.HoliNo)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Daily)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Hours)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", ""));
                    }
                    sb.Append("</table>");
                    //sb.Append("總筆數：" + this.ListLog.Count);       //這裡改成加班統計欄
                    if (OrgNoList.IndexOf(p) < OrgNoList.Count)
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
            Response.AddHeader("content-disposition", "attachment;filename=休假記錄明細表.pdf");
            Response.End();
        }
    }
}