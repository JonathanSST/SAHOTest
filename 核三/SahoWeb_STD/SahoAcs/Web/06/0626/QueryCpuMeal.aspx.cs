using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using OfficeOpenXml;

namespace SahoAcs.Web
{
    public partial class QueryCpuMeal : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogMap> logmaps = new List<CardLogMap>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryCardLogSpec1", "QueryCpuMeal.js");        //加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null&&Request["PageEvent"]=="Query")
            {
                this.SetQuery();
            }
            else
            {
                if (!IsPostBack)
                {
                    var logs = this.odo.GetQueryResult<CardLogMap>("SELECT TOP 1 * FROM B01_CardLog ORDER BY RecordID DESC").ToList();
                    if (logs.Count > 0)
                    {
                        this.Calendar_CardTimeSDate.DateValue = string.Format("{0:yyyy/MM/dd 00:00:00}", logs.First().CardTime);
                        this.Calendar_CardTimeEDate.DateValue = string.Format("{0:yyyy/MM/dd 23:59:59}", logs.First().CardTime);
                    }
                    else
                    {
                        this.Calendar_CardTimeSDate.DateValue = string.Format("{0:yyyy/MM/dd 00:00:00}", DateTime.Now);
                        this.Calendar_CardTimeEDate.DateValue = string.Format("{0:yyyy/MM/dd 23:59:59}", DateTime.Now);
                    }
                    var mga_list = this.odo.GetQueryResult("SELECT * FROM B00_ManageArea WHERE MgaID>1");                    
                }
                
            }
            //this.SetQuery();
        }

        protected void ExportButton_Click(object sender,EventArgs e)
        {
            this.SetQueryByReport();
            this.ExportExcel();
        }

        public void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CardLog");
            //DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "中央警察大學 實際用餐統計表";
            ws.Cells[1, 1, 1, 10].Merge = true;
            ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.Font.Size = 20;
            ws.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            for(int intC = 1; intC <= 10; intC++)
            {
                ws.Cells[2, intC].Value = " ";
            }
            ws.Cells[3, 1].Value = string.Format("查詢起迄時間：{0}~{1}", this.Calendar_CardTimeSDate.DateValue,this.Calendar_CardTimeEDate.DateValue);
            ws.Cells[3, 1, 3, 4].Merge = true;
            ws.Cells[3, 5].Value = string.Format("列印時間：{0:yyyy/MM/dd HH:mm:ss}",DateTime.Now);
            ws.Cells[3, 5, 3, 7].Merge = true;
            ws.Cells[3, 8].Value = string.Format("列印者：{0}",Session["UserName"]);
            ws.Cells[3, 8, 3, 10].Merge = true;
            ws.Cells[4, 1].Value = "餐別";            
            ws.Cells[4, 2].Value = "早餐(40)人";            
            ws.Cells[4, 2, 4, 4].Merge = true;            
            ws.Cells[4, 5].Value = "午餐(65)人";
            ws.Cells[4, 5, 4, 7].Merge = true;            
            ws.Cells[4, 8].Value = "晚餐(65)人";
            ws.Cells[4, 8, 4, 10].Merge = true;
            var MgaNames = this.logmaps.Select(i => i.MgaName).Distinct().OrderBy(q=>q).ToList();
            while (MgaNames.Count < 2)
            {
                MgaNames.Add("");
            }
            ws.Cells[5, 1].Value = "日期";
            ws.Cells[5, 2].Value = MgaNames[0];
            ws.Cells[5, 3].Value = MgaNames[1];
            ws.Cells[5, 4].Value = "小計";
            ws.Cells[5, 5].Value = MgaNames[0];
            ws.Cells[5, 6].Value = MgaNames[1];
            ws.Cells[5, 7].Value = "小計";
            ws.Cells[5, 8].Value = MgaNames[0];
            ws.Cells[5, 9].Value = MgaNames[1];
            ws.Cells[5, 10].Value = "小計";
            for(int i = 1; i <= 10; i++)
            {
                ws.Cells[4, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[4, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                ws.Cells[4, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[5, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[5, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                ws.Cells[5, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
            
            //Content
           
            var ListDays = this.logmaps.Select(i => string.Format("{0:yyyy/MM/dd}",i.CardTime)).Distinct();
           
            int RowStage = 6;
            foreach(var day in ListDays)
            {
                ws.Cells[RowStage, 1].Value = string.Format("{0:yyyy/MM/dd}",day);
                ws.Cells[RowStage, 2].Value = this.logmaps.Where(i => i.MgaName==MgaNames[0] && i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "早餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 3].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "早餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 4].Value = this.logmaps.Where(i => i.CardTime.ToString("yyyy/MM/dd")==day && i.StateDesc == "早餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 5].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "午餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 6].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "午餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 7].Value = this.logmaps.Where(i => i.CardTime.ToString("yyyy/MM/dd")==day && i.StateDesc == "午餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 8].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "晚餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 9].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "晚餐").Sum(i => i.DayCount);
                ws.Cells[RowStage, 10].Value = this.logmaps.Where(i => i.CardTime.ToString("yyyy/MM/dd") == day && i.StateDesc == "晚餐").Sum(i => i.DayCount);
                RowStage++;
            }
            ws.Cells[RowStage, 1].Value = "小計 (人數)";
            ws.Cells[RowStage, 2].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "早餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 3].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "早餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 4].Value = this.logmaps.Where(i => i.StateDesc == "早餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 5].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "午餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 6].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "午餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 7].Value = this.logmaps.Where(i => i.StateDesc == "午餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 8].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "晚餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 9].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "晚餐").Sum(i => i.DayCount);
            ws.Cells[RowStage, 10].Value = this.logmaps.Where(i => i.StateDesc == "晚餐").Sum(i => i.DayCount);
            RowStage++;
            ws.Cells[RowStage, 1].Value = "合計 (金額)";
            ws.Cells[RowStage, 2].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "早餐").Sum(i => i.DayCount*40);
            ws.Cells[RowStage, 3].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "早餐").Sum(i => i.DayCount*40);
            ws.Cells[RowStage, 4].Value = this.logmaps.Where(i => i.StateDesc == "早餐").Sum(i => i.DayCount*40);
            ws.Cells[RowStage, 5].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "午餐").Sum(i => i.DayCount*65);
            ws.Cells[RowStage, 6].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "午餐").Sum(i => i.DayCount*65);
            ws.Cells[RowStage, 7].Value = this.logmaps.Where(i => i.StateDesc == "午餐").Sum(i => i.DayCount*65);
            ws.Cells[RowStage, 8].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "晚餐").Sum(i => i.DayCount*65);
            ws.Cells[RowStage, 9].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "晚餐").Sum(i => i.DayCount*65);
            ws.Cells[RowStage, 10].Value = this.logmaps.Where(i => i.StateDesc == "晚餐").Sum(i => i.DayCount*65);
            RowStage++;
            ws.Cells[RowStage, 1].Value = "本次計價";
            ws.Cells[RowStage, 2].Value = MgaNames[0];
            ws.Cells[RowStage, 3].Value = this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "早餐").Sum(i => i.DayCount * 40) +
                this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "午餐").Sum(i => i.DayCount * 65) +
                this.logmaps.Where(i => i.MgaName == MgaNames[0] && i.StateDesc == "晚餐").Sum(i => i.DayCount * 65);
            ws.Cells[RowStage, 3, RowStage, 4].Merge = true;
            ws.Cells[RowStage, 5].Value = MgaNames[1];
            ws.Cells[RowStage, 6].Value = this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "早餐").Sum(i => i.DayCount * 40) +
                this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "午餐").Sum(i => i.DayCount * 65) +
                this.logmaps.Where(i => i.MgaName == MgaNames[1] && i.StateDesc == "晚餐").Sum(i => i.DayCount * 65);
            ws.Cells[RowStage, 6, RowStage, 7].Merge = true;
            ws.Cells[RowStage, 8].Value = "總計";
            ws.Cells[RowStage, 9].Value = this.logmaps.Where(i => i.StateDesc == "早餐").Sum(i => i.DayCount * 40) +
              this.logmaps.Where(i => i.StateDesc == "午餐").Sum(i => i.DayCount * 65) + this.logmaps.Where(i => i.StateDesc == "晚餐").Sum(i => i.DayCount * 65);
            ws.Cells[RowStage, 9, RowStage, 10].Merge = true;
            //格線設定

            for (int i = 3; i <= ws.Dimension.End.Row; i++)
            {
                for (int j = 1; j <= ws.Dimension.End.Column; j++)
                {
                    if(i>3)
                        ws.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    ws.Cells[i, j].Style.Font.Size = 14;
                }
            }
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=CardLogSpec1.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void SetQuery()
        {
            string cmd_str = @"SELECT COUNT(*) AS DayCount,		                                            
                                                 CASE SyncMark3 WHEN 1 THEN '早餐'
												 WHEN 2 THEN '午餐'
												 WHEN 3 THEN '晚餐'
												END AS StateDesc,CONVERT(VARCHAR,CardTime,111) AS CardTime,MgaName
                                            FROM 
	                                            LogTable CL
	                                            LEFT JOIN B01_Person P ON CL.PsnNo=P.PsnNo	                                            
                                                WHERE SyncMark3 IN (1,2,3) 
                                                AND CardTime BETWEEN @DateS AND @DateE
                                                GROUP BY CardTime,MgaName,SyncMark3                                                
                                                ORDER BY CardTime";
            var param = new { DateS = Request["DateS"], DateE = Request["DateE"],
                PsnName = Request["CardNo_PsnName"]+"%",PsnNo=Request["CardNo_PsnName"],CardNo=Request["CardNo_PsnName"] };
            var log1 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable","B01_CardLog"), param).ToList();
            var log2 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLogFill"), param).ToList();
            this.logmaps = log1.Union(log2).OrderBy(i=>i.CardTime).ToList();
            //if (Request["MgaName"] != "全區")
            //{
            //    this.logmaps = this.logmaps.Where(i => i.MgaName == Request["MgaName"]).ToList();
            //}
            var count = this.logmaps.Count();
        }


        private void SetQueryByReport()
        {
            string cmd_str = @"SELECT COUNT(*) AS DayCount,		                                            
                                                 CASE SyncMark3 WHEN 1 THEN '早餐'
												 WHEN 2 THEN '午餐'
												 WHEN 3 THEN '晚餐'
												END AS StateDesc,CONVERT(VARCHAR,CardTime,111) AS CardTime,MgaName
                                            FROM 
	                                            LogTable CL
	                                            LEFT JOIN B01_Person P ON CL.PsnNo=P.PsnNo	                                            
                                                WHERE SyncMark3 IN (1,2,3) 
                                                AND CardTime BETWEEN @DateS AND @DateE
                                                GROUP BY CardTime,MgaName,SyncMark3                                                
                                                ORDER BY CardTime";
            var param = new
            {
                DateS = this.Calendar_CardTimeSDate.DateValue,
                DateE = this.Calendar_CardTimeEDate.DateValue,
                PsnName = Request["CardNo_PsnName"] + "%",
                PsnNo = Request["CardNo_PsnName"],
                CardNo = Request["CardNo_PsnName"],                
            };
            var log1 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLog"), param).ToList();
            var log2 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLogFill"), param).ToList();
            this.logmaps = log1.Union(log2).OrderBy(i => i.CardTime).ToList();            
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace