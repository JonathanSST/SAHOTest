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
    public partial class QueryInOutReport : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> logmaps = new List<CardLogModel>();
        public List<CardLogModel> OutList = new List<CardLogModel>();
        
        string MainQueryLogStr = @"SELECT A.*,C.OrgStrucID FROM B01_CardLog A
                                                                LEFT JOIN V_PsnCard C ON A.PsnNo=C.PsnNo 
                                                                WHERE LogStatus=0
	                                                                AND CardTime BETWEEN @DateS AND @DateE AND (A.PsnNo LIKE @PsnName OR A.PsnName LIKE @PsnName OR A.CardNo LIKE @PsnName) ORDER BY PsnNo,CardTime ";

        
        string QueryPerson = @"SELECT 
	                                                    * 	
                                                    FROM 
	                                                    B01_CardLog A
                                                    WHERE 
	                                                    CardTime BETWEEN @DateS AND @DateE
	                                                    AND PsnNo IS NOT NULL";
        string LogStateCmdStr = "SELECT * FROM B00_CardLogState ";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryInOutReport", "QueryInOutReport.js");        //加入同一頁面所需的JavaScript檔案



            if (Request["PageEvent"] != null&&Request["PageEvent"]=="Query")
            {
                this.SetQuery();                
            }
            else
            {
                
            }
            if (!IsPostBack)
            {
                this.Calendar1.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
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
            ws.Cells[1, 2].Value = "公司";
            ws.Cells[1, 3].Value = "工號";
            ws.Cells[1, 4].Value = "姓名";
            ws.Cells[1, 5].Value = "卡號";
            ws.Cells[1, 6].Value = "設備編號";
            ws.Cells[1, 7].Value = "設備名稱";
            ws.Cells[1, 8].Value = "進出";
            ws.Cells[1, 9].Value = "讀卡結果";
            int int_state = 0;
            //Content
            for (int i = 0; i < this.logmaps.Count; i++)
            {
                ws.Cells[int_state + 2, 1].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", logmaps[i].CardTime);
                ws.Cells[int_state + 2, 2].Value = logmaps[i].DepName;
                ws.Cells[int_state + 2, 3].Value = logmaps[i].PsnNo;
                ws.Cells[int_state + 2, 4].Value = logmaps[i].PsnName;
                ws.Cells[int_state + 2, 5].Value = logmaps[i].CardNo;
                ws.Cells[int_state + 2, 6].Value = logmaps[i].EquNo;
                ws.Cells[int_state + 2, 7].Value = logmaps[i].EquName;
                ws.Cells[int_state + 2, 8].Value = logmaps[i].EquDir;
                ws.Cells[int_state + 2, 9].Value = logmaps[i].LogStatus;
                int_state++;
            }               
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryStayLog1.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }


        private void SetQuery()
        {            
            var param = new {
                DateS = Request["Calendar1"]+" 00:00:00", DateE= Request["Calendar1"] + " 23:59:59", PsnName=Request["PsnNo"].ToString() + "%"    
            };
            
            var log1 = this.odo.GetQueryResult<CardLogModel>(this.MainQueryLogStr, param).ToList();
            var statelist = this.odo.GetQueryResult(this.LogStateCmdStr);
            var OrgCompany = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM OrgStrucAllData('Company')");
            log1.ForEach(i =>
            {
                i.LogStatus = statelist.Where(state => Convert.ToInt32(state.Code) == int.Parse(i.LogStatus)).FirstOrDefault().StateDesc;
                i.DepName = OrgCompany.Where(o => int.Parse(o.OrgStrucID) == i.OrgStrucID).Count() > 0 ? OrgCompany.Where(o => int.Parse(o.OrgStrucID) == i.OrgStrucID).FirstOrDefault().OrgName : "";
            });
            logmaps = log1;

        }

        private void SetQueryByReport()
        {            
            var param = new
            {
                DateS=this.Calendar1.DateValue + " 00:00:00", DateE=this.Calendar1.DateValue+" 23:59:59",
                PsnName = Request["PsnNo"].ToString() + "%"
            };
            var log1 = this.odo.GetQueryResult<CardLogModel>(this.MainQueryLogStr, param).ToList();
            var statelist = this.odo.GetQueryResult(this.LogStateCmdStr);
            var OrgCompany = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM OrgStrucAllData('Company')");
            log1.ForEach(i =>
            {
                i.LogStatus = statelist.Where(state => Convert.ToInt32(state.Code) == int.Parse(i.LogStatus)).FirstOrDefault().StateDesc;
                i.DepName = OrgCompany.Where(o => int.Parse(o.OrgStrucID) == i.OrgStrucID).Count() > 0 ? OrgCompany.Where(o => int.Parse(o.OrgStrucID) == i.OrgStrucID).FirstOrDefault().OrgName : "";
            });
            logmaps = log1;
            //logmaps = logmaps.Where(i => i.EquDir == this.ddlEquDir.SelectedValue).ToList();
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace