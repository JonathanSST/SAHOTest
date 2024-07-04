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


namespace SahoAcs.Web._0678
{
    public partial class ViolateQuery : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> CardLogList = new List<CardLogModel>();
        public List<CardLogModel> ViolateList = new List<CardLogModel>();
        public DataTable DataResult = new DataTable();
        public List<ColNameObj> ListCols = new List<ColNameObj>();        
        public string txt_CardNo_PsnName = "";

        public string PsnID = "";

        public string SortName = "CardTime";
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
            ClientScript.RegisterClientScriptInclude("IncludeScript", "ViolateQuery.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            Calendar_CardTimeSDate.DateValue = DateTime.Now.GetUtcToZone(this).ToString("yyyy/MM/dd");
            Calendar_CardTimeEDate.DateValue = DateTime.Now.GetUtcToZone(this).ToString("yyyy/MM/dd");
            #endregion

            #region 設定欄位寬度、名稱內容預設            
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeVal", DataRealName = "CardTime", DataWidth = 123, TitleWidth = 120, TitleName = "讀卡時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = "部門" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80, TitleName = "人員編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 74, TitleWidth = 70, TitleName = "卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "TempCardNo", DataWidth = 74, TitleWidth = 70, TitleName = "臨時卡" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardVer", DataWidth = 64, TitleWidth = 60, TitleName = "卡片版次" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "設備編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 104, TitleWidth = 100, TitleName = "設備名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = "讀卡結果" });            
            

            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            CreateGroupList();
        }

        private void CreateGroupList()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            #endregion

            #region Process String
            sql = @"SELECT EquNo, EquName + ' ('+EquNo+')' AS EquName FROM B01_EquData";
            #endregion

            var ddlResult = this.odo.GetQueryResult(sql,new {UserID = hideUserID.Value });

            this.ddlEquNo.DataSource = ddlResult;
            this.ddlEquNo.DataTextField = "EquNo";
            this.ddlEquNo.DataValueField = "EquNme";
            this.ddlEquNo.Items.Insert(0, Item);
            foreach(var o in ddlResult)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = Convert.ToString(o.EquName);
                Item.Value = Convert.ToString(o.EquNo);
                this.ddlEquNo.Items.Add(Item);
            }
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Item = new System.Web.UI.WebControls.ListItem();
            //    Item.Text = dr["OrgName"].ToString();
            //    Item.Value = dr["OrgID"].ToString();
            //    this.ddlDept.Items.Add(Item);
            //}
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
            string sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i => i.EquNo).ToList();
            string sql  = @"SELECT * FROM B01_CardLog C WHERE LogStatus = 160 AND CardTime BETWEEN @DateS AND @DateE AND EquNo IN @EquList";
            string sqlwhere = "";
            if (!string.IsNullOrEmpty(this.GetFormEndValue("ddlEquNo")))
            {
                sqlwhere += " AND EquNo=@EquNo ";
            }
            if (!string.IsNullOrEmpty(this.GetFormEndValue("PsnCardNo")))
            {
                sqlwhere += " AND (PsnNo LIKE @PsnCardNo OR CardNo LIKE @PsnCardNo OR PsnName LIKE @PsnCardNo) ";
            }
            sql += sqlwhere;
                        
            this.CardLogList = this.odo.GetQueryResult<CardLogModel>(sql, new
            {                
                EquNo = this.GetFormEndValue("ddlEquNo"),
                EquList = this.EquDatas,
                DateS = this.GetFormEqlValue("CardTimeS") + " 00:00:00",
                DateE = this.GetFormEqlValue("CardTimeE") + " 23:59:59",
                PsnCardNo = this.GetFormEndValue("PsnCardNo")+"%"
            }).ToList();
            this.CardLogList = this.CardLogList.OrderByField(SortName, SortType.Equals("ASC")).ToList();
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            this.CardLogList.ForEach(i => i.LogStatus = logstatus.Where(o => o.Code == int.Parse(i.LogStatus)).First().StateDesc);
            CardLogModel cardlog = new CardLogModel();
            if (CardLogList.Count > 0)
            {
                //cardlog = this.CardLogList.First();
            }
            var ChkList = new List<CardLogModel>();
            foreach(var o in this.CardLogList)
            {                
                cardlog = o;
                ChkList = new List<CardLogModel>(this.CardLogList.Where(i => i.CardNo.Equals(cardlog.CardNo) && i.EquNo.Equals(cardlog.EquNo) && i.CardTime < cardlog.CardTime.AddMinutes(1) && i.CardTime > cardlog.CardTime));
                if (ChkList.Count>0)
                {
                    this.ViolateList.Add(cardlog);
                    foreach(var data in ChkList)
                    {
                        this.ViolateList.Add(data);
                    }
                }               
            }
            //這邊在做同RecordID 過濾的處理

            Dictionary<string, string> ParaDict = new Dictionary<string, string>();
            string CardRule = "";
           
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ViolateList);
            }
            else
            {
                //轉datatable
                this.PagedList = this.ViolateList.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }
            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));            

            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));             
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
            Response.AddHeader("content-disposition", "attachment; filename=Abnormal.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.CardLogList.Add(new DBModel.CardLogModel()
                {
                    PsnName = "", CardNo="", EquNo="", PsnNo="", CardTime=DateTime.Now
                });
            }
            //轉datatable
            this.PagedList = this.CardLogList.ToPagedList(1, 100);
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
            if (!string.IsNullOrEmpty(this.GetFormEndValue("ddlEquNo")))
            { 
                foreach(var o in this.odo.GetQueryResult("SELECT * FROM B01_EquData WHERE EquNo=@EquNo", new {GrpID=this.GetFormEndValue("ddlEquNo")}))
                {
                    EquName = string.Format("{0}({1})", o.EquName, o.EquNo);
                }
            }
            if (this.ViolateList.Count > 0)
            {
                var PageData = this.ViolateList.ToPagedList(1, 18);
                for(int i=1;i<=PageData.PageCount;i++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>違規讀卡管報</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));                    
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:60%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td><td style='width:40%'>頁數：{1}/{2}</td></tr></table>", DateTime.Now, i, PageData.PageCount));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:30%'>卡號、人員編號：{0}</td><td style='width:30%'>設備名稱：{1}</td><td style='width:40%'>日期區間：{2}~{3}</td></tr></table>"
                       , this.GetFormEndValue("PsnCardNo"), this.GetFormEqlValue("EquNo"), this.GetFormEqlValue("CardTimeS"), this.GetFormEqlValue("CardTimeE")));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td>卡號</td>");
                    sb.Append("<td>工號</td>");
                    sb.Append("<td>人員姓名</td>");
                    sb.Append("<td>單位</td>");
                    //sb.Append("<td>版次</td>");
                    //sb.Append("<td>臨時卡號</td>");
                    sb.Append("<td>讀卡時間</td>");                    
                    sb.Append("<td>設備編號</td>");
                    sb.Append("<td>設備名稱</td>");
                    sb.Append("<td>讀卡結果</td></tr>");
                    foreach (var o in this.ViolateList.ToPagedList(i, 18))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.DepName));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardVer));
                        //sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.TempCardNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:yyyy/MM/dd HH:mm:ss}</td>", o.CardTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EquNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EquName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.LogStatus));                        
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
            Response.AddHeader("content-disposition", "attachment;filename=違規讀卡管報.pdf");
            Response.End();
        }


    }//end page class
}//end namespace