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
using ZXing;


namespace SahoAcs
{
    public partial class _0603 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogTrt> ListLog = new List<CardLogTrt>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

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
        
        public IPagedList<CardLogTrt> PagedList;

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
            ClientScript.RegisterClientScriptInclude("0603", "0603.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE() AND EquClass='TRT' ");

            if (dtLastCardTime == DateTime.MinValue)
            {
                Calendar_CardTimeSDate.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
                Calendar_CardTimeEDate.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");                
            }
            else
            {
                Calendar_CardTimeSDate.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd");
                Calendar_CardTimeEDate.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd");
            }
            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttResult").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeVal", DataRealName = "CardTime", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttCardTime").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "First", DataWidth = 74, TitleWidth = 70, TitleName = GetLocalResourceObject("ttFirst").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "Last", DataWidth = 74, TitleWidth = 70, TitleName = GetLocalResourceObject("ttLast").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 94, TitleWidth = 90, TitleName = GetLocalResourceObject("ttCardNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = GetLocalResourceObject("ttDeptName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80, TitleName = GetLocalResourceObject("ttPsnNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 154, TitleWidth = 150, TitleName = GetLocalResourceObject("ttPsnName").ToString() });
                                                
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            #endregion

            CreateDeptDropItem();
        }

        private void CreateDeptDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            DropDownList_Company.Items.Clear();

            //#region Give Empty Item
            //Item = new System.Web.UI.WebControls.ListItem();
            //Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            //Item.Value = "";
            //#endregion

            //#region Process String
            //sql = @"SELECT OrgID, (OrgName + '(' + OrgNo + ')') AS 'OrgName' FROM
            //    (SELECT B00_SysUserMgns.UserID, OrgStrucAllData.OrgID, OrgStrucAllData.OrgNo,
            //     OrgStrucAllData.OrgName FROM B00_SysUserMgns
            //    INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
            //    INNER JOIN B01_Person ON B01_MgnOrgStrucs.OrgStrucID = B01_Person.OrgStrucID
            //    LEFT OUTER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON B01_Person.OrgStrucID = OrgStrucAllData.OrgStrucID
            //    ) AS Mgns
            //    WHERE Mgns.UserID = @UserID GROUP BY OrgID, (OrgName + '(' + OrgNo + ')')";
            //#endregion

            sql = @"SELECT DISTINCT
                    OrgStrucAllData.OrgID AS DepID, OrgStrucAllData.OrgNo AS DepNo, OrgStrucAllData.OrgName AS DepName,UserID
                    FROM  OrgStrucAllData('Company') AS OrgStrucAllData
                    INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                    INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID  
                    WHERE UserID = @UserID AND OrgID <>'' 
                    ORDER BY OrgStrucAllData.OrgNo ";

            var ddlResult = this.odo.GetQueryResult(sql,new {UserID = hideUserID.Value });

            if (ddlResult.Count() > 0)
            {
                foreach (var o in ddlResult)
                {
                    if (Convert.ToString(o.DepName) != "")
                    {

                        Item = new System.Web.UI.WebControls.ListItem();
                        Item.Text = "[" + Convert.ToString(o.DepNo) + "] " + Convert.ToString(o.DepName);
                        Item.Value = Convert.ToString(o.DepID);
                        DropDownList_Company.Items.Add(Item);
                        if (o.DepNo == "C001")
                        {
                            string sql2 = @"SELECT DISTINCT
                                            OrgStrucAllData.OrgID AS DepID, OrgStrucAllData.OrgNo AS DepNo, OrgStrucAllData.OrgName AS DepName,UserID
                                            FROM  OrgStrucAllData('Department') AS OrgStrucAllData
                                            INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                                            INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID  
                                            WHERE UserID = @UserID AND OrgID <>'' AND SUBSTRING(OrgStrucAllData.OrgNoList, 2, 4) = 'C001'
                                            ORDER BY OrgStrucAllData.OrgNo";

                            var result2 = this.odo.GetQueryResult(sql2, new { UserID = this.hideUserID.Value });
                            if (result2.Count() > 0)
                            {
                                foreach (var oDep in result2)
                                {
                                    Item = new System.Web.UI.WebControls.ListItem();
                                    Item.Text = "<span style='color:#FFFFFF'>--</span>" + "[" + Convert.ToString(oDep.DepNo) + "] " + Convert.ToString(oDep.DepName);
                                    Item.Value = Convert.ToString(oDep.DepID);
                                    DropDownList_Company.Items.Add(Item);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DropDownList_Company.Items.Add(new System.Web.UI.WebControls.ListItem() { Text = "尚無資料", Value = "" });
            }

            //this.ddlDept.DataSource = ddlResult;
            //this.ddlDept.DataTextField = "OrgName";
            //this.ddlDept.DataValueField = "OrgID";
            //this.ddlDept.Items.Insert(0, Item);
            //foreach(var o in ddlResult)
            //{
            //    Item = new System.Web.UI.WebControls.ListItem();
            //    Item.Text = Convert.ToString(o.OrgName);
            //    Item.Value = Convert.ToString(o.OrgID);
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
    //        string sql  = @"SELECT 
    //            CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) AS 'CardTime',
    //            --CONVERT(VARCHAR(50),DATEADD(HOUR,-@DbOffset,DATEADD(HOUR,-@DbUtc,B01_CardLog.CardTime)),111) AS 'CardTime',   --處理UTC時區用的，暫時不會用了
    //            B01_CardLog.CardNo, 				
    //            B01_CardLog.PsnNo, 
				//B01_CardLog.PsnName,DepID,DepName				
				//,MIN(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'First'
    //            ,MAX(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'Last'
    //            --,MIN(SUBSTRING(CONVERT(VARCHAR(50),DATEADD(HOUR,-@DbOffset,DATEADD(HOUR,-@DbUtc,B01_CardLog.CardTime)),121),12,8)) AS 'First'    --處理UTC時區用的，暫時不會用了
    //            --,MAX(SUBSTRING(CONVERT(VARCHAR(50),DATEADD(HOUR,-@DbOffset,DATEADD(HOUR,-@DbUtc,B01_CardLog.CardTime)),121),12,8)) AS 'Last'   --處理UTC時區用的，暫時不會用了             
				//FROM B01_CardLog ";
            //this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i=>i.EquNo).ToList();            
            string sqlwhere = " WHERE  CardTime BETWEEN @CardTimeS AND @CardTimeE AND (EquClass='TRT' OR IsAndTrt='1') AND PsnNo IS NOT NULL";
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            string sql = @"SELECT *,CONVERT(VARCHAR(10),CardTime,111) AS CardDate FROM V_CardLogOrg ";
            if (Request["PsnNo"] != "")
            {
                sqlwhere += " AND PsnNo LIKE @PsnNo";
            }
            if (Request["CardNo"] != "")
            {
                sqlwhere += " AND CardNo LIKE @CardNo";
            }
            //if (Request["DepID"] != "")
            //{
            //    sqlwhere += " AND DepID = @DepID";
            //}
            if (Request["PsnID"] != "")
            {
                sqlwhere += " AND PsnNo IN (SELECT PsnNo FROM B01_Person WHERE PsnID=@PsnID)";
            }
            if (Request["oDepID"] != null && Request["oDepID"] != "")
            {
                sqlwhere += " AND (oComID IN @oDepID OR oDepID IN @oDepID )";
            }
            sql += sqlwhere;
            //sql += @" GROUP BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,111),CardNo,PsnNo,PsnName,DepID,DepName ";
            //sql += @" GROUP BY CardTime,CardNo,PsnNo,PsnName,DepID,DepName ";
            
            int TimeDiff = 0;
            if (Session["TimeOffset"] != null && Session["TimeOffset"].ToString() != string.Empty)
            {
                TimeDiff = Convert.ToInt32(Session["TimeOffset"]);
            }
            var CardLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                EquList = EquDatas,
                CardNo = "%" + Request["CardNo"] + "%",
                CardTimeS = DateTime.Parse(Request["CardTimeS"] + " 00:00:00").GetUtcTime(this),
                CardTimeE = DateTime.Parse(Request["CardTimeE"] + " 23:59:59").GetUtcTime(this),
                PsnNo = "%" + Request["PsnNo"] + "%",
                DepID = Request["DepID"],
                PsnID = Request["PsnID"],
                DbUtc = WebAppService.DbUTC,
                DbOffset = TimeDiff / 8,
                oDepID = Request["oDepID"].Split(',')
            }).ToList();
            this.ListLog = (from g in CardLog
                          where !string.IsNullOrEmpty(g.PsnNo)
                          group g by new { g.PsnNo, g.CardDate, g.CardNo, g.DepName, g.PsnName } into groups
                          select new CardLogTrt
                          {
                              PsnNo = groups.Key.PsnNo,
                              CardTime = Convert.ToDateTime(groups.Key.CardDate),
                              CardNo = groups.Key.CardNo,
                              DepName = groups.Key.DepName,
                              PsnName = groups.Key.PsnName
                              //,First = CardLog.Where(i => i.PsnNo.Equals(groups.Key.PsnNo) && i.CardDate.Equals(groups.Key.CardDate)).OrderBy(i => i.CardTime).First().CardTime.ToString("HH:mm:ss"),
                              //Last = CardLog.Where(i => i.PsnNo.Equals(groups.Key.PsnNo) && i.CardDate.Equals(groups.Key.CardDate)).OrderBy(i => i.CardTime).Last().CardTime.ToString("HH:mm:ss")
                          }).ToList();
            this.ListLog.ForEach(p =>
            {
                p.First = CardLog.Where(i => i.PsnNo.Equals(p.PsnNo) && i.CardTime.ToString("yyyy/MM/dd").Equals(i.CardDate)).OrderBy(i=>i.CardTime).First().CardTime.ToString("HH:mm:ss");
                p.Last = CardLog.Where(i => i.PsnNo.Equals(p.PsnNo) && i.CardTime.ToString("yyyy/MM/dd").Equals(i.CardDate)).OrderBy(i=>i.CardTime).Last().CardTime.ToString("HH:mm:ss");
                
            });
            this.ListLog = this.ListLog.OrderByField(SortName, SortType.Equals("ASC")).ToList();

            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                //轉datatable
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }
   
            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
           
            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(r["CardTime"]));
                //r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LogTime"]));
                //r["LogStatus"] = logstatus.Where(i => Convert.ToInt32(i.Code) == Convert.ToInt32(r["LogStatus"])).FirstOrDefault().StateDesc;
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
                this.ListLog.Add(new DBModel.CardLogTrt()
                {
                    //PsnName = "TEST",
                    CardTime = DateTime.Now,
                    LogTime = DateTime.Now
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));            
            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LogTime"]));
            }
        }



        private void ExportPdf()
        {
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");

            if (this.ListLog.Count > 0)
            {
                var PageData = this.ListLog.ToPagedList(1, 25);
                var PsnNoList = this.ListLog.Select(i => i.PsnNo).Distinct<string>().ToList();
             
                for(int i=1;i<=PageData.PageCount;i++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>考勤首末筆紀錄</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));                    
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>頁數：{0}/{1}</td><td style='width:50%'>日期區間 {2}~{3}</td></tr></table>"
                        , i,PageData.PageCount, Request.Form["CardTimeS"], Request.Form["CardTimeE"]));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:9%'>日期</td>");
                    sb.Append("<td class='DataBar' style='width:9%'>首筆</td>");
                    sb.Append("<td class='DataBar' style='width:9%'>末筆</td>");
                    sb.Append("<td class='DataBar' style='width:11%'>卡號</td>");
                    sb.Append("<td class='DataBar' style='width:11%'>部門</td>");
                    sb.Append("<td class='DataBar' style='width:11%'>人員編號</td>");                    
                    sb.Append("<td class='DataBar' style='width:11%'>姓名</td></tr>");
                    foreach (var o in this.ListLog.ToPagedList(i,25))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0:yyyy/MM/dd}</td>", o.CardTime));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.First));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.Last));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.DepName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.PsnName));    //14                        
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
            Response.AddHeader("content-disposition", "attachment;filename=考勤首末筆紀錄.pdf");
            Response.End();
        }


    }//end page class
}//end namespace