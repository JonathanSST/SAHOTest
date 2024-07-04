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
    public partial class QueryCardLogSpec2 : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogMap> logmaps = new List<CardLogMap>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryCardLogSpec2", "QueryCardLogSpec2.js");        //加入同一頁面所需的JavaScript檔案

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
            //DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "廠商";            
            ws.Cells[1, 2].Value = "日期";
            ws.Cells[1, 3].Value = "自助餐區";
            ws.Cells[1, 4].Value = "";
            ws.Cells[1, 5].Value = "";
            ws.Cells[1, 6].Value = "快餐區";
            ws.Cells[1, 7].Value = "";
            ws.Cells[1, 8].Value = "";            
            ws.Cells[1, 9].Value = "燴飯區";
            ws.Cells[1, 10].Value = "";
            ws.Cells[1, 11].Value = "";
            ws.Cells[1, 12].Value = "麵食區";
            ws.Cells[1, 13].Value = "";
            ws.Cells[1, 14].Value = "";
            ws.Cells[1, 15].Value = "輕食區";
            ws.Cells[1, 16].Value = "";
            ws.Cells[1, 17].Value = "";
            ws.Cells[1, 18].Value = "觸控輸入";
            ws.Cells[1, 19].Value = "";
            ws.Cells[1, 20].Value = "";
            ws.Cells[1, 21].Value = "合計";
            ws.Cells[1, 22].Value = "";
            ws.Cells[1, 23].Value = "";
            ws.Cells[1, 24].Value = "總計";
            ws.Cells[2, 1].Value = "";
            ws.Cells[2, 2].Value = "";
            ws.Cells[2, 3].Value = "早餐";
            ws.Cells[2, 4].Value = "午餐";
            ws.Cells[2, 5].Value = "晚餐";
            ws.Cells[2, 6].Value = "早餐";
            ws.Cells[2, 7].Value = "午餐";
            ws.Cells[2, 8].Value = "晚餐";
            ws.Cells[2, 9].Value = "早餐";
            ws.Cells[2, 10].Value = "午餐";
            ws.Cells[2, 11].Value = "晚餐";
            ws.Cells[2, 12].Value = "早餐";
            ws.Cells[2, 13].Value = "午餐";
            ws.Cells[2, 14].Value = "晚餐";
            ws.Cells[2, 15].Value = "早餐";
            ws.Cells[2, 16].Value = "午餐";
            ws.Cells[2, 17].Value = "晚餐";
            ws.Cells[2, 18].Value = "早餐";
            ws.Cells[2, 19].Value = "午餐";
            ws.Cells[2, 20].Value = "晚餐";
            ws.Cells[2, 21].Value = "早餐";
            ws.Cells[2, 22].Value = "午餐";
            ws.Cells[2, 23].Value = "晚餐";
            ws.Cells[2, 24].Value = "全天";
            ws.Cells[1, 1, 2, 1].Merge = true;
            ws.Cells[1, 2, 2, 2].Merge = true;
            ws.Cells[1, 3, 1, 5].Merge = true;
            ws.Cells[1, 6, 1, 8].Merge = true;
            ws.Cells[1, 9, 1, 11].Merge = true;
            ws.Cells[1, 12, 1, 14].Merge = true;
            ws.Cells[1, 15, 1, 17].Merge = true;
            ws.Cells[1, 18, 1, 20].Merge = true;
            ws.Cells[1, 21, 1, 23].Merge = true;
            int int_state = 3;
            var groupresult = from g in this.logmaps
                              group g by new { g.CardTime }
                                      into groups
                              select new
                              {
                                  CardDay = groups.Key.CardTime,
                                  MgaName = groups.First().MgaName,
                                  AmountB = groups.Count(i => i.LogStatus == "49"),
                                  AmountL = groups.Count(i => i.LogStatus == "51"),
                                  AmountD = groups.Count(i => i.LogStatus == "53"),
                                  Amount1 = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("自助餐區")),
                                  Amount2 = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("自助餐區")),
                                  Amount3 = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("自助餐區")),
                                  Amount4 = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("快餐區")),
                                  Amount5 = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("快餐區")),
                                  Amount6 = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("快餐區")),
                                  Amount7 = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("燴飯區")),
                                  Amount8 = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("燴飯區")),
                                  Amount9 = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("燴飯區")),
                                  Amount10 = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("麵食區")),
                                  Amount11 = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("麵食區")),
                                  Amount12 = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("麵食區")),
                                  Amount13 = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("觸控")),
                                  Amount14 = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("觸控")),
                                  Amount15 = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("觸控")),
                                  Amount16 = groups.Count(i => i.LogStatus == "49" && i.EquName.Contains("輕食區")),
                                  Amount17 = groups.Count(i => i.LogStatus == "51" && i.EquName.Contains("輕食區")),
                                  Amount18 = groups.Count(i => i.LogStatus == "53" && i.EquName.Contains("輕食區")),
                                  AmountAll = groups.Count()
                              };
            foreach (var o in groupresult)
            {
                ws.Cells[int_state, 1].Value = string.Format("{0:yyyy/MM/dd}", o.CardDay);
                ws.Cells[int_state, 2].Value = o.MgaName;
                ws.Cells[int_state, 3].Value = o.Amount1;
                ws.Cells[int_state, 4].Value = o.Amount2;
                ws.Cells[int_state, 5].Value = o.Amount3;
                ws.Cells[int_state, 6].Value = o.Amount4;
                ws.Cells[int_state, 7].Value = o.Amount5;
                ws.Cells[int_state, 8].Value = o.Amount6;
                ws.Cells[int_state, 9].Value = o.Amount7;
                ws.Cells[int_state, 10].Value = o.Amount8;
                ws.Cells[int_state, 11].Value = o.Amount9;
                ws.Cells[int_state, 12].Value = o.Amount10;
                ws.Cells[int_state, 13].Value = o.Amount11;
                ws.Cells[int_state, 14].Value = o.Amount12;
                ws.Cells[int_state, 15].Value = o.Amount16;
                ws.Cells[int_state, 16].Value = o.Amount17;
                ws.Cells[int_state, 17].Value = o.Amount18;
                ws.Cells[int_state, 18].Value = o.Amount13;
                ws.Cells[int_state, 19].Value = o.Amount14;
                ws.Cells[int_state, 20].Value = o.Amount15;
                ws.Cells[int_state, 21].Value = o.AmountB;
                ws.Cells[int_state, 22].Value = o.AmountL;
                ws.Cells[int_state, 23].Value = o.AmountD;
                ws.Cells[int_state, 24].Value = o.AmountAll;
                int_state++;
            }
            ws.Cells[int_state, 1].Value = "合計";
            ws.Cells[int_state, 2].Value = "";
            ws.Cells[int_state, 3].Value = this.logmaps.Where(i => i.EquName.Contains("自助餐區") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 4].Value = this.logmaps.Where(i => i.EquName.Contains("自助餐區") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 5].Value = this.logmaps.Where(i => i.EquName.Contains("自助餐區") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 6].Value = this.logmaps.Where(i => i.EquName.Contains("快餐區") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 7].Value = this.logmaps.Where(i => i.EquName.Contains("快餐區") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 8].Value = this.logmaps.Where(i => i.EquName.Contains("快餐區") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 9].Value = this.logmaps.Where(i => i.EquName.Contains("燴飯區") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 10].Value = this.logmaps.Where(i => i.EquName.Contains("燴飯區") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 11].Value = this.logmaps.Where(i => i.EquName.Contains("燴飯區") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 12].Value = this.logmaps.Where(i => i.EquName.Contains("麵食區") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 13].Value = this.logmaps.Where(i => i.EquName.Contains("麵食區") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 14].Value = this.logmaps.Where(i => i.EquName.Contains("麵食區") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 15].Value = this.logmaps.Where(i => i.EquName.Contains("輕食區") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 16].Value = this.logmaps.Where(i => i.EquName.Contains("輕食區") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 17].Value = this.logmaps.Where(i => i.EquName.Contains("輕食區") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 18].Value = this.logmaps.Where(i => i.EquName.Contains("觸控") && i.LogStatus == "49").Count();
            ws.Cells[int_state, 19].Value = this.logmaps.Where(i => i.EquName.Contains("觸控") && i.LogStatus == "51").Count();
            ws.Cells[int_state, 20].Value = this.logmaps.Where(i => i.EquName.Contains("觸控") && i.LogStatus == "53").Count();
            ws.Cells[int_state, 21].Value = this.logmaps.Where(i => i.LogStatus == "49").Count();
            ws.Cells[int_state, 22].Value = this.logmaps.Where(i =>  i.LogStatus == "51").Count();
            ws.Cells[int_state, 23].Value = this.logmaps.Where(i => i.LogStatus == "53").Count();
            ws.Cells[int_state, 24].Value = this.logmaps.Count();
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
                                                AND CardTime BETWEEN @DateS AND @DateE  AND EquName NOT LIKE '%滿意度%'
                                                ORDER BY CardTime";
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
                                                AND CardTime BETWEEN @DateS AND @DateE AND EquName NOT LIKE '%滿意度%'
                                                ORDER BY CardTime";
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