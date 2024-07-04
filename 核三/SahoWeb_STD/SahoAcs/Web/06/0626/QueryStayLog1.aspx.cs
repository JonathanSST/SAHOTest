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
    public partial class QueryStayLog1 : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> logmaps = new List<CardLogModel>();


        string MainQueryLogStr = @"SELECT 
	                                                *
                                                FROM 
	                                                B01_CardLog A
	                                                WHERE CardTime BETWEEN @DateS AND @DateE AND LogStatus=0 
                                                    AND (PsnName LIKE @PsnName OR PsnNo=@PsnNo OR CardNo=@CardNo)
                                                    AND A.EquNo IN ('Equ010','Equ011','Equ012','Equ013','Equ016','Equ017','Equ018','Equ019')
                                                ORDER BY 
	                                                PsnNo,CardTime";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryStayLog1", "QueryStayLog1.js");        //加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null&&Request["PageEvent"]=="Query")
            {
                this.SetQuery();
            }
            else
            {
                if (!IsPostBack)
                {
                    var logs = this.odo.GetQueryResult<CardLogModel>(@"SELECT 
	                                                                                                            TOP 100 *
                                                                                                            FROM 
	                                                                                                            B01_CardLog A	                                                                                                            
                                                                                                            ORDER BY 
	                                                                                                            CardTime DESC").ToList();
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
            ws.Cells[1, 1].Value = "讀卡時間";
            ws.Cells[1, 2].Value = "前次讀卡時間";
            ws.Cells[1, 3].Value = "設備名稱";
            ws.Cells[1, 4].Value = "單位";
            ws.Cells[1, 5].Value = "學號";
            ws.Cells[1, 6].Value = "姓名";
            ws.Cells[1, 7].Value = "卡號";
            ws.Cells[1, 8].Value = "進出";
            ws.Cells[1, 9].Value = "前次進出";            
            int int_state = 0;
            //Content
            for (int i = 0; i < this.logmaps.Count; i++)
            {
                ws.Cells[int_state + 2, 1].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", logmaps[i].CardTime);
                ws.Cells[int_state + 2, 2].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}",logmaps[i].PreCardTime);
                ws.Cells[int_state + 2, 3].Value = logmaps[i].EquName;
                ws.Cells[int_state + 2, 4].Value = logmaps[i].DepName;
                ws.Cells[int_state + 2, 5].Value = logmaps[i].PsnNo;
                ws.Cells[int_state + 2, 6].Value = logmaps[i].PsnName;
                ws.Cells[int_state + 2, 7].Value = logmaps[i].CardNo;
                ws.Cells[int_state + 2, 8].Value = logmaps[i].EquDir;
                ws.Cells[int_state + 2, 9].Value = logmaps[i].PreEquDir;                
                int_state++;
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryStayLog1.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }


        private List<CardLogModel> GetErrorLog(List<CardLogModel> LogList)
        {
            string PsnNo = "";
            DateTime? OldTime = null;
            string EquDir = "";
            int count = 0;
            TimeSpan ts;
            List<CardLogModel> ErrorLogs = new List<CardLogModel>();
            foreach (var o in LogList)
            {
                if (PsnNo != "" && PsnNo != o.PsnNo)
                {
                    count = 0;
                }
                if (count > 0)
                {
                    o.PreEquDir = EquDir;
                    o.PreCardTime = (DateTime)OldTime;                    
                    ts = o.CardTime - o.PreCardTime;
                    o.TimeMin = (int)ts.TotalSeconds;
                    if (o.TimeMin <= 1 && o.EquDir == o.PreEquDir)
                    {
                        ErrorLogs.Add(o);
                    }
                }
                count++;
                OldTime = o.CardTime;
                PsnNo = o.PsnNo;
                EquDir = o.EquDir;                
            }

            return ErrorLogs;
        }



        private void SetQuery()
        {            
            var param = new { DateS = Request["DateS"], DateE = Request["DateE"],
                PsnName = Request["CardNo_PsnName"]+"%",PsnNo=Request["CardNo_PsnName"],CardNo=Request["CardNo_PsnName"] };
            var log1 = this.odo.GetQueryResult<CardLogModel>(this.MainQueryLogStr, param).ToList();
            
            //var count = this.logmaps.Count();
            this.logmaps = this.GetErrorLog(log1);
            if (Request["EquDir"] != "0")
            {
                logmaps = logmaps.Where(i => i.EquDir == Request["EquDir"]).ToList();
            }
        }


        private void SetQueryByReport()
        {            
            var param = new
            {
                DateS = this.Calendar_CardTimeSDate.DateValue,
                DateE = this.Calendar_CardTimeEDate.DateValue,
                PsnName = Request["CardNo_PsnName"] + "%",
                PsnNo = Request["CardNo_PsnName"],
                CardNo = Request["CardNo_PsnName"],                
            };
            var log1 = this.odo.GetQueryResult<CardLogModel>(this.MainQueryLogStr, param).ToList();
            this.logmaps = this.GetErrorLog(log1);
            if (this.ddlEquDir.SelectedValue != "0")
            {
                logmaps = logmaps.Where(i => i.EquDir == this.ddlEquDir.SelectedValue).ToList();
            }
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace