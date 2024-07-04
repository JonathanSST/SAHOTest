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


namespace SahoAcs.Web._0675
{
    public partial class EquGroupDetail : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<EquGroupData> GrpLis = new List<EquGroupData>();
        public DataTable DataResult = new DataTable();
        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public string txt_CardNo_PsnName = "";

        public string PsnID = "";

        public string SortName = "EquGrpNo";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        
        public IPagedList<EquGroupData> PagedList;

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
            ClientScript.RegisterClientScriptInclude("IncludeScript", "EquGroupDetail.js");//加入同一頁面所需的JavaScript檔案
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
            this.ListCols.Add(new ColNameObj() { ColName = "EquGrpNo", DataRealName = "EquGrpNo", DataWidth = 104, TitleWidth = 100, TitleName = "群組編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquGrpName", DataWidth = 144, TitleWidth = 140, TitleName = "群組名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 74, TitleWidth = 70, TitleName = "設備編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 144, TitleWidth = 140, TitleName = "設備名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquModel", DataWidth = 124, TitleWidth = 120, TitleName = "設備名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "OwnerID", DataWidth = 104, TitleWidth = 100, TitleName = "創建人" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardRule", DataWidth = 124, TitleWidth = 120, TitleName = "讀卡規則" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardExtData", DataWidth = 84, TitleWidth = 80, TitleName = "開放樓層" });            
                                                
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
            sql = @"SELECT EquGrpName+' ('+EquGrpNo+')' AS EquGrpNo, EquGrpID FROM B01_EquGroup";
            #endregion

            var ddlResult = this.odo.GetQueryResult(sql,new {UserID = hideUserID.Value });

            this.ddlEquGrpNo.DataSource = ddlResult;
            this.ddlEquGrpNo.DataTextField = "EquGrpNo";
            this.ddlEquGrpNo.DataValueField = "EquGrpID";
            this.ddlEquGrpNo.Items.Insert(0, Item);
            foreach(var o in ddlResult)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = Convert.ToString(o.EquGrpNo);
                Item.Value = Convert.ToString(o.EquGrpID);
                this.ddlEquGrpNo.Items.Add(Item);
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
            var EquElevData = this.odo.GetQueryResult("SELECT * FROM B01_ElevatorFloor").ToList();
            this.ListCols = collist;            
            string sql  = @"SELECT EquModel,OwnerID,EG.EquGrpNo,EquGrpName,EquNo,EquName,EGD.* from B01_EquGroup EG
                                INNER JOIN B01_EquGroupData EGD ON EG.EquGrpID=EGD.EquGrpID
                                INNER JOIN B01_EquData ED ON EGD.EquID=ED.EquID";                 
            string sqlwhere = " WHERE  (EquNo LIKE @Key OR EquName LIKE @Key OR EquGrpNo LIKE @Key OR EquGrpName LIKE @Key)";
            if (!string.IsNullOrEmpty(this.GetFormEndValue("ddlEquGrpNo")))
            {
                sqlwhere += " AND EG.EquGrpID=@EquGrpID";
            }
            sql += sqlwhere;
            
            
            this.GrpLis = this.odo.GetQueryResult<EquGroupData>(sql, new
            {
                Key=this.GetFormEqlValue("EquGrpNo")+"%",
                EquGrpID = this.GetFormEndValue("ddlEquGrpNo"),
            }).ToList();
            var CardRuleList = this.odo.GetQueryResult("SELECT EquID, ParaValue FROM B01_EquParaData A INNER JOIN B01_EquParaDef B ON A.EquParaID=B.EquParaID AND ParaName='CardRule'").ToList();
            var CardRuleDef = this.odo.GetQueryResult("SELECT * FROM B01_CardRuleDef");
            this.GrpLis = this.GrpLis.OrderByField(SortName, SortType.Equals("ASC")).ToList();
            Dictionary<string, string> ParaDict = new Dictionary<string, string>();
            string CardRule = "";
            this.GrpLis.Where(i => !string.IsNullOrEmpty(i.CardExtData)).ToList().ForEach(i =>
            {                
                var str16To2Ext = string.Join("", Sa.Change.HexToBin(i.CardExtData, 48).Reverse());
                i.CardExtData = "";
                List<string> FloorOpenList = new List<string>();
                for (int j = 0; j < str16To2Ext.Length; j++)
                {
                    if (str16To2Ext.Substring(j, 1) == "1" && j < EquElevData.Where(floor=>Convert.ToInt32(floor.EquID)==i.EquID).Count())
                    {
                        FloorOpenList.Add(EquElevData.Where(floor => Convert.ToInt32(floor.EquID) == i.EquID).ElementAt(j).FloorName);
                    }
                }
                i.CardExtData = string.Join(",", FloorOpenList);
            });
            this.GrpLis.Where(i => !string.IsNullOrEmpty(i.CardRule)).ToList().ForEach(i =>
            {
                if (CardRuleList.Where(rule => rule.EquID == i.EquID).Count() > 0 && CardRuleDef .Where(cr=>cr.EquModel.Equals(i.EquModel)).Count()>0)
                {
                    ParaDict.Clear();
                    CardRule = CardRuleList.Where(rule => rule.EquID == i.EquID).First().ParaValue;
                    foreach(var RuleValue in CardRule.Split(','))
                    {
                        if (RuleValue.Split(':').Count() > 1 && CardRuleDef.Where(cr => cr.EquModel.Equals(i.EquModel) && Convert.ToString(cr.RuleID).Equals(RuleValue.Split(':')[1])).Count()>0)
                        {
                            i.CardRule = CardRuleDef.Where(cr => cr.EquModel.Equals(i.EquModel) && Convert.ToString(cr.RuleID).Equals(RuleValue.Split(':')[1])).First().RuleName;
                        }
                    }                    
                }
            });
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.GrpLis);
            }
            else
            {
                //轉datatable
                this.PagedList = this.GrpLis.ToPagedList(PageIndex, 100);
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
                this.GrpLis.Add(new DBModel.EquGroupData()
                {
                    EquGrpID = 0, EquGrpNo="",EquNo="",EquModel=""
                });
            }
            //轉datatable
            this.PagedList = this.GrpLis.ToPagedList(1, 100);
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
            string EquGrpName = "";
            if (!string.IsNullOrEmpty(this.GetFormEndValue("ddlEquGrpNo")))
            { 
                foreach(var o in this.odo.GetQueryResult("SELECT * FROM B01_EquGroup WHERE EquGrpID=@GrpID", new {GrpID=this.GetFormEndValue("ddlEquGrpNo")}))
                {
                    EquGrpName = string.Format("{0}({1})", o.EquGrpName, o.EquGrpNo);
                }
            }
            if (this.GrpLis.Count > 0)
            {
                var PageData = this.GrpLis.ToPagedList(1, 25);
                for(int i=1;i<=PageData.PageCount;i++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>設備群組設備明細</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));                    
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>頁數：{0}/{1}</td><td style='width:25%'>群組條件：{3}</td><td style='width:25%'>關鍵字：{2}</td></tr></table>"
                        , i,PageData.PageCount, this.GetFormEqlValue("EquGrpNo"), EquGrpName));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:15%'>設備群組</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>設備名稱</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>型號</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>規則</td>");
                    sb.Append("<td class='DataBar' style='width:20%'>開放樓層</td></tr>");                    
                    foreach (var o in this.GrpLis.ToPagedList(i,25))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}({1})</td>", o.EquGrpName, o.EquGrpNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}({1})</td>", o.EquName, o.EquNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EquModel));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardRule));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.CardExtData));                        
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
            Response.AddHeader("content-disposition", "attachment;filename=設備群組設備明細.pdf");
            Response.End();
        }


    }//end page class
}//end namespace