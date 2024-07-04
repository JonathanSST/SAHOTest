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


namespace SahoAcs.Web._0676
{
    public partial class PsnCardNoneRecord : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<PersonEntity> PersonList = new List<PersonEntity>();
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
        
        public IPagedList<PersonEntity> PagedList;

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
            ClientScript.RegisterClientScriptInclude("IncludeScript", "PsnCardNoneRecord.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
           
            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttResult").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataRealName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "人員編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 144, TitleWidth = 140, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 144, TitleWidth = 140, TitleName = "卡號" });            
            this.ListCols.Add(new ColNameObj() { ColName = "OrgNameList", DataWidth = 144, TitleWidth = 140, TitleName = "單位部門" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnETime", DataWidth = 144, TitleWidth = 140, TitleName = "最後刷卡時間" });

            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            CreateGroupList();
        }

        private void CreateGroupList()
        {
            this.EquList = this.odo.GetQueryResult<EquGroupData>(@"SELECT 
                        E.* FROM B01_EquData E 
                        INNER JOIN B01_EquGroupData E2 ON E2.EquID=E.EquID
                        INNER JOIN B01_MgnEquGroup E3 ON E2.EquGrpID=E3.EquGrpID 
                        INNER JOIN B00_SysUserMgns E4 ON E3.MgaID=E4.MgaID WHERE E4.UserID=@UserID",new {UserID=Sa.Web.Fun.GetSessionStr(this, "UserID")}).ToList();
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
            string sql  = @"SELECT A.*,OrgNameList,ISNULL(CONVERT(VARCHAR(10),CardTime,111),'') AS PsnETime
                                            FROM V_PsnCard A 
                                            INNER JOIN OrgStrucAllData('') AS OrgStrucAllData ON OrgStrucAllData.OrgStrucID = A.OrgStrucID
                                            LEFT JOIN (SELECT CardNo, CONVERT(VARCHAR(10),MAX(CardTime),111) AS CardTime FROM B01_CardLog WHERE EquNo=@EquNo GROUP BY CardNo) Sub1 ON A.CardNo=Sub1.CardNo
                                            WHERE A.CardNo NOT IN (SELECT CardNo FROM B01_CardLog
                                            WHERE CardTime BETWEEN DATEADD(day,@Daily,GETDATE()) AND GETDATE() AND EquNo=@EquNo)";

            this.PersonList = this.odo.GetQueryResult<PersonEntity>(sql, new {Daily=int.Parse(this.GetFormEqlValue("ddlRangeDay")), EquNo=this.GetFormEqlValue("EquNoName") }).OrderByField(SortName,SortType.Equals("ASC")).ToList();
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
                this.PersonList.Add(new DBModel.PersonEntity()
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
            string EquName = "";
            if (!string.IsNullOrEmpty(this.GetFormEndValue("EquNoName")))
            { 
                foreach(var o in this.odo.GetQueryResult("SELECT * FROM B01_EquData WHERE EquNo=@EquNo", new {EquNo=this.GetFormEndValue("EquNoName") }))
                {
                    EquName = string.Format("{0}({1})", o.EquName, o.EquNo);
                }
            }
            if (this.PersonList.Count > 0)
            {
                var PageData = this.PersonList.ToPagedList(1, 25);
                for(int i=1;i<=PageData.PageCount;i++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>門禁權限審查報表</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));                    
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:40%'>頁數：{0}/{1}</td><td style='width:20%'>未打卡天數累積：{2}</td><td style='width:40%'>查核設備資訊：{3}</td></tr></table>"
                        , i,PageData.PageCount, this.GetFormEqlValue("ddlRangeDay"), EquName));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:15%'>人員編號</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>卡號</td>");
                    sb.Append("<td class='DataBar' style='width:30%'>單位</td>");
                    sb.Append("<td class='DataBar' style='width:20%'>前次打卡時間</td></tr>");                    
                    foreach (var o in this.PersonList.ToPagedList(i,25))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.OrgNameList));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.PsnETime));                        
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
            Response.AddHeader("content-disposition", "attachment;filename=門禁權限審查報表.pdf");
            Response.End();
        }


    }//end page class
}//end namespace