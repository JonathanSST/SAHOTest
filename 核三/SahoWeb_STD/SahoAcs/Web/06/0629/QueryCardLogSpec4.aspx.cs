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
    public partial class QueryCardLogSpec4 : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogMap> logmaps = new List<CardLogMap>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryCardLogSpec4", "QueryCardLogSpec4.js");        //加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQuery();
            }
            else
            {
                if (!IsPostBack)
                {
                    var logs = this.odo.GetQueryResult<CardLogMap>(@"SELECT TOP 1 * FROM (
                                                                                                                    SELECT  CardTime,RecordID FROM B01_CardLog
                                                                                                                    UNION 
                                                                                                                    SELECT  CardTime,RecordID FROM B01_CardLogFill) AS TR
                                                                                                                    ORDER BY CardTime DESC").ToList();
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
            ws.Cells[1, 1].Value = "廠商";            
            ws.Cells[1, 2].Value = "日期";
            ws.Cells[1, 3].Value = "早餐滿意";
            ws.Cells[1, 4].Value = "早餐不滿意";
            ws.Cells[1, 5].Value = "午餐滿意";
            ws.Cells[1, 6].Value = "午餐不滿意";
            ws.Cells[1, 7].Value = "晚餐滿意";
            ws.Cells[1, 8].Value = "晚餐不滿意";
            ws.Cells[1, 9].Value = "當日滿意";
            ws.Cells[1, 10].Value = "當日不滿意";
            ws.Cells[1, 11].Value = "當日滿意度(%)";
            ws.Cells[1, 12].Value = "當日投票人數";
            ws.Cells[1, 13].Value = "當日用餐人數";
            ws.Cells[1, 14].Value = "當日投票率(%)";
            int int_state = 2;
            var groupresult = from g in this.logmaps
                              group g by new { g.CardTime }
                                      into groups
                              select new
                              {
                                  CardDay = groups.Key.CardTime,
                                  MgaName = groups.First().MgaName,
                                  AmountAll = groups.Count(i => !i.EquName.Contains("滿意度")),
                                  AmountSel = groups.Count(i => i.EquName.Contains("滿意度")),
                                  AmountBY = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("滿意度調查(右)")),
                                  AmountBN = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("滿意度調查(左)")),
                                  AmountLY = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("滿意度調查(右)")),
                                  AmountLN = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("滿意度調查(左)")),
                                  AmountDY = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("滿意度調查(右)")),
                                  AmountDN = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("滿意度調查(左)")),
                                  AmountY = groups.Count(i => i.EquName.Contains("滿意度調查(右)")),
                                  AmountN = groups.Count(i => i.EquName.Contains("滿意度調查(左)"))
                              };
            foreach (var o in groupresult)
            {
                ws.Cells[int_state, 1].Value = string.Format("{0:yyyy/MM/dd}", o.CardDay);
                ws.Cells[int_state, 2].Value = o.MgaName;
                ws.Cells[int_state, 3].Value = o.AmountBY;
                ws.Cells[int_state, 4].Value = o.AmountBN;
                ws.Cells[int_state, 5].Value = o.AmountLY;
                ws.Cells[int_state, 6].Value = o.AmountLN;
                ws.Cells[int_state, 7].Value = o.AmountDY;
                ws.Cells[int_state, 8].Value = o.AmountDN;
                ws.Cells[int_state, 9].Value = o.AmountY;
                ws.Cells[int_state, 10].Value = o.AmountN;
                var baseInt = 1;
                if (o.AmountSel > 0)
                {
                    baseInt = o.AmountSel;
                }
                ws.Cells[int_state, 11].Value = string.Format("{0:0}",(o.AmountY/baseInt)*100);
                ws.Cells[int_state, 12].Value = o.AmountSel;
                ws.Cells[int_state, 13].Value = o.AmountAll;
                if (o.AmountAll > 0)
                {
                    baseInt = o.AmountAll;
                }
                ws.Cells[int_state, 14].Value = string.Format("{0:0.00}", (o.AmountSel / baseInt) * 100);
                int_state++;
            }
            ws.Cells[int_state, 1].Value = "合計";
            ws.Cells[int_state, 2].Value = "";
            ws.Cells[int_state, 3].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(右)") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 4].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(左)") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 5].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(右)") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 6].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(左)") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 7].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(右)") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 8].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(左)") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 9].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(右)")).Count();
            ws.Cells[int_state, 10].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查(左)")).Count();
            ws.Cells[int_state, 11].Value = 0;
            ws.Cells[int_state, 12].Value = this.logmaps.Where(i => i.EquName.Contains("滿意度調查")).Count();
            ws.Cells[int_state, 13].Value = this.logmaps.Where(i => !i.EquName.Contains("滿意度調查")).Count();
            ws.Cells[int_state, 14].Value = 0;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=CardLogSpec2.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private void SetQuery()
        {
            string cmd_str = @"SELECT DISTINCT
	                                            CardNo,CL.PsnNo,EquNo,EquName ,OSA.OrgName,CL.PsnName,StateDesc,Convert(varchar,CardTime,111) AS CardTime,MgaName,LogStatus
                                            FROM 
	                                            LogTable CL
	                                            LEFT JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	                                            INNER JOIN OrgStrucAllData('Unit') OSA ON CL.OrgStruc=OrgIDList 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code WHERE LogStatus IN (49,51,53) 
                                                AND CardTime BETWEEN @DateS AND @DateE ORDER BY CardTime";
            var param = new { DateS = Request["DateS"], DateE = Request["DateE"] };
            var log1 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable","B01_CardLog"), param).ToList();
            var log2 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLogFill"), param).ToList();
            this.logmaps = log1.Union(log2).OrderBy(i=>i.CardTime).ToList();
            this.logmaps = this.logmaps.Where(i => i.MgaName == Request["MgaName"]).ToList();
            var count = this.logmaps.Count();
        }


        private void SetQueryByReport()
        {
            string cmd_str = @"SELECT DISTINCT
	                                            CardNo,CL.PsnNo,EquNo,EquName ,OSA.OrgName,CL.PsnName,StateDesc,Convert(varchar,CardTime,111) AS CardTime,MgaName,LogStatus
                                            FROM 
	                                            LogTable CL
	                                            LEFT JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	                                            INNER JOIN OrgStrucAllData('Unit') OSA ON CL.OrgStruc=OrgIDList 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code WHERE LogStatus IN (49,51,53) 
                                                AND CardTime BETWEEN @DateS AND @DateE ORDER BY CardTime";
            var param = new
            {
                DateS = this.Calendar_CardTimeSDate.DateValue,
                DateE = this.Calendar_CardTimeEDate.DateValue
            };
            var log1 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLog"), param).ToList();
            var log2 = this.odo.GetQueryResult<CardLogMap>(cmd_str.Replace("LogTable", "B01_CardLogFill"), param).ToList();
            this.logmaps = log1.Union(log2).OrderBy(i => i.CardTime).ToList();
            this.logmaps = this.logmaps.Where(i => i.MgaName == this.DropMgaList.Items[this.DropMgaList.SelectedIndex].Text).ToList();
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace