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

namespace SahoAcs.Web._07._0707
{
    public partial class QueryWorkDetail : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<B03WorkDetail> ListLog = new List<B03WorkDetail>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<B03PsnHoliday> PsnHoliday = new List<B03PsnHoliday>();
        public string txt_CardNo_PsnName = "";
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
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
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetQueryData();
                this.Export();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Save")
            {
                this.SetSaveMemo();
            }
            else
            {
                this.SetInitData();
                this.CardDayS.DateValue = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            }

            ClientScript.RegisterClientScriptInclude("jsfun", "QueryWorkDetail.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案

        }

        private void SetInitData()
        {
            dropDepartment.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), Value = "" });
            dropCompany.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), Value = "" });


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
            string DateS = Request["CardDateS"];
            string DateE = Request["CardDateE"];
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
OrgStrucAllData_2.OrgNoList,
WD.*,
P.PsnName 
FROM B03_WorkDetail WD
Inner Join B01_Person P ON WD.PsnNo=P.PsnNo 
Inner Join OrgStrucAllData('Company') AS OrgStrucAllData_0 ON P.OrgStrucID = OrgStrucAllData_0.OrgStrucID
Inner Join OrgStrucAllData('Department') AS OrgStrucAllData_1 ON P.OrgStrucID = OrgStrucAllData_1.OrgStrucID
Inner Join OrgStrucAllData('') AS OrgStrucAllData_2 ON P.OrgStrucID = OrgStrucAllData_2.OrgStrucID
Where 1=1 And WD.Abnormal = 0 ";

            Sql += "And WorkDate Between @DateS And @DateE ";
            if (!string.IsNullOrEmpty(Company))
            {
                Sql += "And OrgStrucAllData_0.OrgNo='" + Company + "' ";
            }
            if (!string.IsNullOrEmpty(Department))
            {
                Sql += "And OrgStrucAllData_1.OrgNo='" + Department + "' ";
            }

            this.ListLog = this.odo.GetQueryResult<B03WorkDetail>(Sql.ToString(), new
            {
                DateS = DateS,
                DateE = DateE
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
            string sDate = Request["CardDateS"];
            string eDate = Request["CardDateE"];
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    font-size:10pt;vertical-align:top;
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
                var PageData = this.ListLog.ToPagedList(1, 30);
                //var PsnNoList = this.ListLog.Select(i => i.PsnNo).Distinct<string>().ToList();
                var PsnNoList = this.ListLog.Select(i => i.OrgNoList).Distinct<string>().ToList();
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"
                                                        select 
                                                        A.*,
                                                        OrgStrucAllData_1.*,
                                                        OrgStrucAllData_2.*
                                                        from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON A.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                                                        INNER JOIN OrgStrucAllData('Department') AS OrgStrucAllData_2 ON A.OrgStrucID = OrgStrucAllData_2.OrgStrucID
                                                        WHERE D.UserID=@UserID", new { UserID = Sa.Web.Fun.GetSessionStr(this, "UserID") }).OrderBy(i => i.PsnName).ToList();

                foreach (var p in PsnNoList)
                {
                    if (this.PersonList.Count > 0 && this.PersonList.Where(i => i.OrgNoList.Equals(p)).Count() > 0)
                    {
                        //var psn_data = this.PersonList.Where(i => i.PsnNo.Equals(p)).First();
                        //this.PsnName = psn_data.PsnNo + "  " + psn_data.PsnName;
                        var psn_data = this.PersonList.Where(i => i.OrgNoList.Equals(p)).First();
                        this.PsnName = psn_data.OrgName;
                    }
                    else
                    {
                        this.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName");
                    }
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>差　勤　紀　錄　明　細　表</div>");
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>部門名稱：{0}</td></tr></table>", PsnName));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>時間區間：{0}~{1}</td><td></td></tr></table>", sDate, eDate, ""));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='TitleBar' style='width:7%'>工號</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>姓名</td>");
                    sb.Append("<td class='TitleBar' style='width:9%'>日期</td>");
                    //sb.Append("<td class='TitleBar' style='width:5%'>班別</td>");
                    //sb.Append("<td class='DataBar' style='width:6%'>標上</td>");
                    //sb.Append("<td class='DataBar' style='width:6%'>標下</td>");
                    sb.Append("<td class='TitleBar' style='width:6%'>刷上</td>");
                    sb.Append("<td class='TitleBar' style='width:6%'>刷下</td>");
                    //sb.Append("<td class='TitleBar' style='width:14%'>請假啟始時間</td>");
                    //sb.Append("<td class='TitleBar' style='width:14%'>請假結束時間</td>");
                    //sb.Append("<td class='TitleBar' style='width:5%'>天數</td>");
                    //sb.Append("<td class='TitleBar' style='width:5%'>時數</td>");
                    sb.Append("<td class='TitleBar' style='width:5%'>遲到</td>");
                    sb.Append("<td class='TitleBar' style='width:5%'>早退</td>");
                    sb.Append("<td class='TitleBar' style='width:5%'>加班</td>");
                    sb.Append("<td class='TitleBar' style='width:7%'>狀態</td></tr>");
                    //sb.Append("<td class='TitleBar' style='width:15%'>備註</td></tr>");                    
                    //foreach (var o in this.ListLog.Where(i => i.PsnNo.Equals(p)))
                    foreach (var o in this.ListLog.Where(i => i.OrgNoList.Equals(p)))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.WorkDate));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.ClassNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RealTimeS)); //5
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.RealTimeE));    //6
                        //startTime = string.Join("<br/>", o.StartTime.Split(',').ToList().Select(i => i.Length > 16 ? i.Substring(0, 16) : ""));
                        //endTime = string.Join("<br/>", o.EndTime.Split(',').ToList().Select(i => i.Length > 16 ? i.Substring(0, 16) : ""));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", startTime));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", endTime));    //8
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Daily));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Hours));   //10
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Delay));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.StealTime));    //12
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.OverTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.StatuDesc));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.WorkDesc));    //14                        
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
            Response.AddHeader("content-disposition", "attachment;filename=差勤紀錄明細表.pdf");
            Response.End();
        }

        private void SetSaveMemo()
        {
            int arr = 0;
            var RecordInfo = this.Request.Form.GetValues("RecordID");
            var MemoInfo = this.Request.Form.GetValues("Remark");
            Pub.MessageObject sRet = new Pub.MessageObject()
            {
                result = false,
                act = "Save",
                message = ""
            };
            if (RecordInfo.Length != MemoInfo.Length)
            {
                sRet.message = "備註資料格式錯誤，無法修改";
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
                Response.End();
            }
            foreach (var i in RecordInfo)
            {
                this.odo.Execute("UPDATE B03_WorkDetail SET Remark=@Remark WHERE RecordID=@RecordID", new { RecordID = i, Remark = MemoInfo[arr] });
                arr++;
            }
            sRet.message = "備註資料完成更新";
            sRet.result = true;
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }
    }
}