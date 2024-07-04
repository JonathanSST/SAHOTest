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
using System.Text;
using System.IO;
using SahoAcs.DBClass;

using PagedList;
using OfficeOpenXml;



namespace SahoAcs
{
    public partial class PersonAreaInfo : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<PsnExtData> ListLog = new List<PsnExtData>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "PsnNo";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        
        public IPagedList<PsnExtData> PagedList;

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
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "QueryOneLog")
            {
                this.SetOneCardLog();
            }
            else
            {
                this.SetInitData();
                this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            ClientScript.RegisterClientScriptInclude("MainJs", "PersonAreaInfo.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion           

            #region 設定欄位寬度、名稱內容預設                        
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80,TitleName=GetLocalResourceObject("ttPsnNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100,TitleName=GetLocalResourceObject("ttPsnName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CtrlAreaNo", DataWidth = 74, TitleWidth = 70,TitleName=GetLocalResourceObject("ttAreaNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CtrlAreaName", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttAreaName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = GetLocalResourceObject("ttEquNo").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 104, TitleWidth = 100, TitleName = GetLocalResourceObject("ttEquName").ToString() });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeVal", DataRealName = "LastTime", DataWidth = 123, TitleWidth = 120, TitleName = GetLocalResourceObject("ttCardTime").ToString() });
            foreach (var o in this.ListCols.Where(i => i.DataRealName==null||i.DataRealName == ""))
                o.DataRealName = o.ColName;
            
            #endregion            
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
            if (!String.IsNullOrEmpty(this.GetFormEqlValue("SortName")))
            {
                this.SortName = this.GetFormEqlValue("SortName");
                this.SortType = Request["SortType"];
            }
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");
            //foreach (var s in Request.Form.GetValues("ColName"))
            //{                
            //    collist.Add(this.ListCols.Where(i => i.ColName == s).FirstOrDefault());
            //}
            //this.ListCols = collist;            
            string sql = "";                        
            sql = @"SELECT P.*,CtrlAreaName,EquNo,EquName,LastTime,CA.CtrlAreaNo FROM 
                        B01_PersonExt A 
                            INNER JOIN B01_Person P ON A.PsnID=P.PsnID
                                INNER JOIN B01_EquData E ON LastDoorNo=EquNo 
                                INNER JOIN B01_CtrlArea CA ON P.PsnAreaNo=CA.CtrlAreaNo WHERE (PsnName LIKE @PsnNo OR PsnNo LIKE @PsnNo ) AND LastTime>=@NowDate";
            this.txt_CardNo_PsnName = this.GetFormEqlValue("CardNoPsnNo");
            this.ListLog = this.odo.GetQueryResult<PsnExtData>(sql, new
            {
                PsnNo = this.GetFormEqlValue("CardNoPsnNo") + "%", NowDate=this.GetZoneTime().AddDays(-1)
            }).ToList();            
            this.ListLog = this.ListLog.OrderByField(this.SortName, boolSort).ToList();
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
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LastTime"]));                
            }
        }




        private void ExportExcel()
        {
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt;margin-left:0px;margin-right:0px'>");

            if (this.ListLog.Count > 0)
            {
                var PageData = this.ListLog.ToPagedList(1, 30);

                for (int p = 1; p <= PageData.PageCount; p++)
                {                  
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>人員刷卡區域明細</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    //sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>打卡時間區間：{0}~{1}</td><td></td></tr></table>", Request["DateS"], Request["DateE"], ""));
                    sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now, p));                    
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:8%'>工號</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>姓名</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>區域編號</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>區域名稱</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>設備編號</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>設備名稱</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>最後刷卡時間</td></tr>");
                    foreach (var o in this.ListLog.ToPagedList(p, 30))
                    {
                        sb.Append(string.Format("<tr><td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));                        
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CtrlAreaNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CtrlAreaName));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EquNo));
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EquName));                        
                        sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0:yyyy/MM/dd HH:mm:ss}</td></tr>", o.LastTime));                        
                    }
                    sb.Append("</table>");
                    if (p < PageData.PageCount)
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
            Response.AddHeader("content-disposition", "attachment;filename=人員刷卡區域明細.pdf");
            Response.End();
        }//end method



        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new DBModel.PsnExtData()
                {
                    //PsnName = "TEST",
                    LastTime = DateTime.Now                    
                });
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
            //this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));            
            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LastTime"]));
            }
        }

        void SetOneCardLog()
        {
            
        }


    }//end page class
}//end namespace