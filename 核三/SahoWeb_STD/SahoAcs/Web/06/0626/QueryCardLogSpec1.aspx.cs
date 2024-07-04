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
    public partial class QueryCardLogSpec1 : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogMap> logmaps = new List<CardLogMap>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryCardLogSpec1", "QueryCardLogSpec1.js");        //加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null&&Request["PageEvent"]=="Query")
            {
                this.SetQuery();
            }
            else
            {
                if (!IsPostBack)
                {
                    var logs = this.odo.GetQueryResult<CardLogMap>("SELECT TOP 1 * FROM B01_CardLog WHERE EquName NOT LIKE '%滿意度%' ORDER BY RecordID DESC").ToList();
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
                    this.DropMgaList.Items.Add(new ListItem()
                    {
                        Text = "全區",Value="1"
                    });
                    foreach (var o in mga_list)
                    {
                        this.DropMgaList.Items.Add(new ListItem()
                        {
                            Text = Convert.ToString(o.MgaName),
                            Value = Convert.ToString(o.MgaID)
                        });
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
            ws.Cells[1, 1].Value = "用餐日期";
            ws.Cells[1, 2].Value = "餐別";
            ws.Cells[1, 3].Value = "班級";
            ws.Cells[1, 4].Value = "學號";
            ws.Cells[1, 5].Value = "姓名";
            ws.Cells[1, 6].Value = "卡號";
            ws.Cells[1, 7].Value = "讀卡機";
            ws.Cells[1, 8].Value = "廠商";
            ws.Cells[1, 9].Value = "資料來源";
            int int_state = 0;
            //Content
            for (int i = 0; i < this.logmaps.Count; i++)
            {
                ws.Cells[int_state + 2, 1].Value = string.Format("{0:yyyy/MM/dd}", logmaps[i].CardTime);
                ws.Cells[int_state + 2, 2].Value = logmaps[i].StateDesc;
                ws.Cells[int_state + 2, 3].Value = logmaps[i].OrgName;
                ws.Cells[int_state + 2, 4].Value = logmaps[i].PsnNo;
                ws.Cells[int_state + 2, 5].Value = logmaps[i].PsnName;
                ws.Cells[int_state + 2, 6].Value = logmaps[i].CardNo;
                ws.Cells[int_state + 2, 7].Value = logmaps[i].EquName;
                ws.Cells[int_state + 2, 8].Value = logmaps[i].MgaName;
                ws.Cells[int_state + 2, 9].Value = "";
                int_state++;
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
            string cmd_str = @"SELECT DISTINCT
	                                            CardNo,CL.PsnNo,EquNo,EquName ,OSA.OrgName,CL.PsnName,StateDesc,CONVERT(VARCHAR,CardTime,111) AS CardTime,MgaName
                                            FROM 
	                                            LogTable CL
	                                            LEFT JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	                                            INNER JOIN OrgStrucAllData('Unit') OSA ON CL.OrgStruc=OrgIDList 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code WHERE LogStatus IN (49,51,53) 
                                                AND CardTime BETWEEN @DateS AND @DateE AND (CL.PsnName LIKE @PsnName OR CL.PsnNo=@PsnNo OR CL.CardNo=@CardNo)
                                                AND EquName NOT LIKE '%滿意度%'
                                                ORDER BY CardNo,CardTime";
            var param = new { DateS = Request["DateS"], DateE = Request["DateE"],
                PsnName = Request["CardNo_PsnName"]+"%",PsnNo=Request["CardNo_PsnName"],CardNo=Request["CardNo_PsnName"] };
            var log1 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable","B01_CardLog"), param).ToList();
            var log2 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLogFill"), param).ToList();
            this.logmaps = log1.Union(log2).OrderBy(i=>i.CardTime).ToList();
            if (Request["MgaName"] != "全區")
            {
                this.logmaps = this.logmaps.Where(i => i.MgaName == Request["MgaName"]).ToList();
            }
            var count = this.logmaps.Count();
        }


        private void SetQueryByReport()
        {
            string cmd_str = @"SELECT DISTINCT
	                                            CardNo,CL.PsnNo,EquNo,EquName ,OSA.OrgName,CL.PsnName,StateDesc,CONVERT(VARCHAR,CardTime,111) AS CardTime,MgaName
                                            FROM 
	                                            LogTable CL
	                                            LEFT JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	                                            INNER JOIN OrgStrucAllData('Unit') OSA ON CL.OrgStruc=OrgIDList 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code 
                                                WHERE LogStatus IN (49,51,53) 
                                                AND CardTime BETWEEN @DateS AND @DateE AND (CL.PsnName LIKE @PsnName OR P.PsnNo=@PsnNo OR CL.CardNo=@CardNo)
                                                AND EquName NOT LIKE '%滿意度%' ";
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
            if (this.DropMgaList.Items[this.DropMgaList.SelectedIndex].Text != "全區")
            {
                this.logmaps = this.logmaps.Where(i => i.MgaName == this.DropMgaList.Items[this.DropMgaList.SelectedIndex].Text).ToList();
            }            
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace