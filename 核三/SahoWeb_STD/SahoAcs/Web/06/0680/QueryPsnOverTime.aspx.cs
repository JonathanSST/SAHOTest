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
using System.Web.Script.Serialization;
using System.IO;
using System.Text;

namespace SahoAcs
{
    public partial class QueryPsnOverTime : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogTrt> ListLog = new List<CardLogTrt>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();

        public string txt_inputPsn = "";
        public string txt_inputCardETAG = "";
        public string txt_inputCarNum = "";

        public string SortName = "CardTime";
        public string SortType = "DESC";
        public string CardType = "16";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<CardLogTrt> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "PrintExcel")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
            }
           
            ClientScript.RegisterClientScriptInclude("0680", "QueryPsnOverTime.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            this.Calendar_CardTime.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 00:00:00";
            #endregion

            this.CreateDropDownList_CardTypeItem();
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

            bool boolSort = this.SortType.Equals("ASC");
            string sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID AND B01_EquData.EquClass='Door Access' 
                GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Where(i => Pub.ETAGEquList.Split(',').Contains(i.EquNo)).Select(i => i.EquNo).ToList();
            
            if (Pub.ETAGEquList.Split(',').Contains("IPCAM"))
            {
                this.EquDatas.Add("IPCAM");
            }

            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");

            string sql = "";

            sql = @"
                SELECT A.*, P.Text1 AS CarNum, P.Text2 AS ETC, D.OrgID AS oDepID, D.OrgName AS oDepName
                FROM B01_CardLog A 
                LEFT JOIN B01_Person P ON A.PsnNo = P.PsnNo
                LEFT JOIN OrgStrucAllData('Department') D ON D.OrgStrucID = P.OrgStrucID
                WHERE (A.EquNo IN @EquList)
                AND (A.EquClass='Door Access' OR A.EquClass='IPCAM') 
                AND A.IsAndTrt=0
            ";

            //      sql = @"
            //            SELECT A.*, P.Text1 AS CarNum, P.Text2 AS ETC, D.OrgID AS oDepID, D.OrgName AS oDepName
            //            FROM B01_CardLog A 
            //            LEFT JOIN B01_Person P ON A.PsnNo = P.PsnNo
            //LEFT JOIN B01_EquData E ON A.EquNo = E.EquNo
            //            LEFT JOIN OrgStrucAllData('Department') D ON D.OrgStrucID = P.OrgStrucID
            //            WHERE  ((A.EquClass='Door Access' AND E.EquModel IN ('SC300','SCM320')) OR A.EquClass='IPCAM') 
            //            AND A.IsAndTrt=0
            //      ";

            sql += this.GetConditionCmdStr();

            this.ListLog = this.odo.GetQueryResult<CardLogTrt>(sql, new
            {
                EquList = this.EquDatas,
                CardTimeS = DateTime.Parse(Request["CardDateS"]).GetUtcTime(this),
                CardTimeE = DateTime.Parse(Request["CardDateE"]).GetUtcTime(this),
                LogTimeS = Request["LogTimeS"],
                LogTimeE = Request["LogTimeE"],
                DepName = "%" + Request["DepNoDepName"] + "%",
                EquName = "%" + Request["EquNoEquName"] + "%",
                PsnNo = "%" + this.txt_inputPsn + "%",
                LogStatus = Request["LogStatus"].Split(','),
                DepID = Request["DepNo"].Split(','),
                CardNo = "%" + this.txt_inputCardETAG + "%",
                CarNum = "%" + this.txt_inputCarNum + "%"
            }).ToList();

            this.ListLog = this.ListLog.OrderByField(this.SortName, boolSort).ToList();

            if (Request["PageEvent"] == "PrintExcel")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);

            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);


            }

            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));

            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]).GetZoneTime(this));
                r["LogStatus"] = logstatus.Where(i => Convert.ToInt32(i.Code) == Convert.ToInt32(r["LogStatus"])).FirstOrDefault().StateDesc;
            }
        }

        private string GetConditionCmdStr()
        {
            string sql = "";
            this.txt_inputPsn = Request["inputPsn"];
            this.txt_inputCardETAG = Request["inputCardETAG"];
            this.txt_inputCarNum = Request["inputCarNum"];

            //進階查詢功能設定
            if (Request["QueryMode"] == "2")
            {
                this.txt_inputPsn = Request["ADVPsnNoPsnNam"];
                this.txt_inputCardETAG = Request["ADVCardNoETAG"];
                this.txt_inputCarNum = Request["ADVCarNum"];

                if (Request["DepNoDepName"] != null && Request["DepNoDepName"] != "")
                {
                    sql += " AND D.OrgName LIKE @DepName";
                }
                if (Request["EquNoEquName"] != null && Request["EquNoEquName"] != "")
                {
                    sql += " AND (A.EquNo LIKE @EquName OR A.EquName LIKE @EquName )";
                }
                if (Request["LogTimeS"] != null && Request["LogTimeS"] != "")
                {
                    sql += " AND LogTime >= @LogTimeS";
                }
                if (Request["LogTimeE"] != null && Request["LogTimeE"] != "")
                {
                    sql += " AND A.LogTime <= @LogTimeE";
                }
                if (Request["EquNo"] != null && Request["EquNo"] != "")
                {
                    this.EquDatas = Request["EquNo"].Split(',').ToList();
                }
                if (Request["DepNo"] != null && Request["DepNo"] != "")
                {
                    sql += " AND D.OrgID IN @DepID AND (D.OrgName IS NOT NULL AND D.OrgName !='' )";
                }
                if (Request["LogStatus"] != null && Request["LogStatus"] != "")
                {
                    sql += " AND A.LogStatus IN @LogStatus ";
                }
            }
            //一般查詢的方法
            sql += " AND (A.CardTime BETWEEN @CardTimeS AND @CardTimeE )";
            if (this.txt_inputPsn != "")
            {
                sql += " AND (A.PsnNo LIKE @PsnNo OR A.PsnName LIKE @PsnNo) ";
            }
            if (this.txt_inputCardETAG != "")
            {
                sql += " AND A.CardNo LIKE @CardNo ";
            }
            if (this.txt_inputCarNum != "")
            {
                sql += " AND A.PlateNo LIKE @CarNum ";
            }
            return sql;
        }

        //private void ExportPDF()
        //{
        //    StringBuilder sb = new StringBuilder(@"<html><style> td
        //        {
        //            border-style:solid; border-width:1px; border-color:Black;width:70px;
        //        } </style><body>");

        //    if (this.ListLog.Count > 0)
        //    {
        //        var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
        //        string logdesc = string.Empty;
        //        var PageData = this.ListLog.ToPagedList(1, 35);

        //        for (int i = 1; i <= PageData.PageCount; i++)
        //        {
        //            sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
        //            sb.Append("<tr>");
        //            sb.Append("<td>廠區</td>");
        //            sb.Append("<td>讀卡日期</td>");
        //            sb.Append("<td>人員編號</td>");
        //            sb.Append("<td>人員名稱</td>");
        //            sb.Append("<td>部門</td>");
        //            sb.Append("<td>卡片號碼</td>");
        //            sb.Append("<td>入廠日期</td>");
        //            sb.Append("<td>入廠時間</td>");
        //            sb.Append("</tr>");

        //            foreach (var o in this.ListLog.ToPagedList(i, 35))
        //            {
        //                sb.Append("<tr>");
        //                sb.Append(string.Format("<td>{0}</td>", o.EquName));
        //                sb.Append(string.Format("<td>{0}</td>", string.Format("{0:yyyy/MM/dd}", o.CardTime)));
        //                sb.Append(string.Format("<td>{0}</td>", o.PsnNo));
        //                sb.Append(string.Format("<td>{0}</td>", o.PsnName));
        //                sb.Append(string.Format("<td>{0}</td>", o.DepName));
        //                sb.Append(string.Format("<td>{0}</td>", o.CardNo));
        //                sb.Append(string.Format("<td>{0}</td>", string.Format("{0:yyyy/MM/dd}", o.CardTime)));
        //                sb.Append(string.Format("<td>{0}</td>", string.Format("{0:HH:mm:ss}", o.CardTime)));
        //                sb.Append("</tr>");
        //            }
        //            sb.Append("</table>");
        //            if (i < PageData.PageCount)
        //            {
        //                sb.Append("<p style='page-break-after:always'>&nbsp;</p>");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        sb.Append("查無資料");
        //    }
        //    sb.Append("</body></html>");
        //    Response.Clear();
        //    byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
        //    MemoryStream msInput = new MemoryStream(data);
        //    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 3, 3, 3, 3);
        //    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, Response.OutputStream);
        //    iTextSharp.text.pdf.PdfDestination pdfDest = new iTextSharp.text.pdf.PdfDestination(iTextSharp.text.pdf.PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
        //    doc.Open();
        //    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new SahoAcs.UnicodeFontFactory());
        //    //將pdfDest設定的資料寫到PDF檔
        //    iTextSharp.text.pdf.PdfAction action = iTextSharp.text.pdf.PdfAction.GotoLocalPage(1, pdfDest, writer);
        //    writer.SetOpenAction(action);
        //    doc.Close();
        //    msInput.Close();
        //    //這裡控制Response回應的格式內容

        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=QueryCarInAndOut.pdf");
        //    Response.End();
        //}


        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");
            ws.Cells[1, 1].Value = "讀卡日期";
            ws.Cells[1, 2].Value = "部門名稱[代碼]";
            ws.Cells[1, 3].Value = "人員編號";
            ws.Cells[1, 4].Value = "人員名稱";
            ws.Cells[1, 5].Value = "卡號/ETAG";
            ws.Cells[1, 6].Value = "車號";
            ws.Cells[1, 7].Value = "設備編號";
            ws.Cells[1, 8].Value = "設備名稱";
            ws.Cells[1, 9].Value = "進出狀態";
            ws.Cells[1, 10].Value = "讀卡結果";
            ws.Cells[1, 11].Value = "記錄時間";

            for (int i = 0; i < this.DataResult.Rows.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.DataResult.Rows[i]["CardTime"].ToString());
                ws.Cells[i + 2, 2].Value = this.DataResult.Rows[i]["oDepName"].ToString();
                ws.Cells[i + 2, 3].Value = this.DataResult.Rows[i]["PsnNo"].ToString();
                ws.Cells[i + 2, 4].Value = this.DataResult.Rows[i]["PsnName"].ToString();
                ws.Cells[i + 2, 5].Value = this.DataResult.Rows[i]["CardNo"].ToString();
                ws.Cells[i + 2, 6].Value = this.DataResult.Rows[i]["PlateNo"].ToString();
                ws.Cells[i + 2, 7].Value = this.DataResult.Rows[i]["EquNo"].ToString();
                ws.Cells[i + 2, 8].Value = this.DataResult.Rows[i]["EquName"].ToString();
                ws.Cells[i + 2, 9].Value = this.DataResult.Rows[i]["EquDir"].ToString();
                ws.Cells[i + 2, 10].Value = this.DataResult.Rows[i]["LogStatus"].ToString();
                ws.Cells[i + 2, 11].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.DataResult.Rows[i]["LogTime"].ToString());
            }


            ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = "總筆數：";
            ws.Cells[this.DataResult.Rows.Count + 2, 2].Value = this.DataResult.Rows.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryCarInAndOut.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        #region CreateDropDownList_CardTypeItem

        private void CreateDropDownList_CardTypeItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            List<string> liSqlPara = new List<string>();
            this.DropDownList_CardType.Items.Clear();
            var result = this.odo.GetQueryResult("select ItemNo, ItemName from B00_ItemList where ItemClass='CardType'");
            if (result.Count() > 0)
            {
                foreach (var o in result)
                {
                    Item = new ListItem();
                    var copyItem = new ListItem();
                    Item.Text = Convert.ToString(o.ItemName);
                    Item.Value = Convert.ToString(o.ItemNo);//dr["Code"].ToString();
                    copyItem = Item;
                    this.DropDownList_CardType.Items.Add(Item);
                }
            }
            else
            {
                this.DropDownList_CardType.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

    
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