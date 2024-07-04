using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using OfficeOpenXml;
using PagedList;


namespace SahoAcs.Web
{
    public partial class QueryStayLog4 : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> logmaps = new List<CardLogModel>();
        public List<CardLogModel> OutList = new List<CardLogModel>();
        public Dictionary<string, List<string>> Builders = new Dictionary<string, List<string>>();
        public int RowCount = 0;
        public string MainQueryLogStr = @"SELECT 
	                                                                A.* 
                                                                FROM 
	                                                                B01_CardLog A
	                                                                INNER JOIN 
	                                                                (SELECT 
		                                                                PsnNo,MAX(RecordID) AS LastDir
		                                                                FROM 
		                                                                B01_CardLog A
		                                                                WHERE PsnNo IN (SELECT PsnNo FROM B01_Person WHERE Text1='M' )
		                                                                AND DATEDIFF(DAY,CardTime,@CardTime) BETWEEN 0 AND @Range AND PsnNo!='' AND LogStatus=0
		                                                                GROUP BY PsnNo) AS F1 ON A.RecordID=F1.LastDir
		                                                                WHERE EquDir=@EquDir  AND A.EquNo IN (SELECT EquNo 
                                                FROM B01_EquData A
	                                                INNER JOIN B01_EquGroupData B ON A.EquID=B.EquID
	                                                INNER JOIN B01_EquGroup C ON B.EquGrpID=C.EquGrpID
                                                WHERE EquGrpNo IN @EgList) ";

        string SecQueryLogStr = @"SELECT PsnNo,LeaveDate AS CardTime FROM OGView.dbo.v_sahoMotor_Leave WHERE LeaveDate=@CardTime";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryStayLog4", "QueryStayLog4.js");        //加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null&&Request["PageEvent"]=="Query")
            {
                this.SetBuilderData();
                this.SetQuery();
            }
            else
            {
                List<EquGroupModel> PersonList = this.odo.GetQueryResult<EquGroupModel>("SELECT * FROM OGView.dbo.v_sahoMotor WHERE EquGrpNo IN ('EG006','EG007','EG008','EG009') ").ToList();
                var pagetotal = PersonList.Count / 500;
                for (int p = 1; p <= pagetotal+1; p++)
                {
                    this.odo.Execute("UPDATE B01_Person SET Text1='' WHERE PsnNo IN @PsnList", new { PsnList = PersonList.ToPagedList(p, 500).Select(i => i.PsnNo) });
                    this.odo.Execute("UPDATE B01_Person SET Text1='M' WHERE PsnNo IN @PsnList", new { PsnList = PersonList.ToPagedList(p,500).Select(i => i.PsnNo) });                    
                }
                    //PersonList.ToPagedList(0, 100);                
            }
            if (!IsPostBack)
            {
                this.CalendarFrm1.DateValue = DateTime.Now.ToString("yyyy/MM/dd");                
            }
            //this.SetQuery();
        }

        private void SetBuilderData()
        {
            Builders = new Dictionary<string, List<string>>();
            Builders.Add("1", new List<string>());
            Builders["1"].Add("EG006");
            Builders["1"].Add("EG007");
            Builders["1"].Add("EG010");
            Builders["1"].Add("EG011");
            Builders["1"].Add("EG008");
            Builders["1"].Add("EG009");
            Builders["1"].Add("EG012");
            Builders["1"].Add("EG013");
            Builders.Add("2", new List<string>());
            Builders["2"].Add("EG006");
            Builders["2"].Add("EG007");
            Builders["2"].Add("EG010");
            Builders["2"].Add("EG011");
            Builders.Add("3", new List<string>());
            Builders["3"].Add("EG008");
            Builders["3"].Add("EG009");
            Builders["3"].Add("EG012");
            Builders["3"].Add("EG013");
        }



        protected void ExportButton_Click(object sender,EventArgs e)
        {
            this.SetBuilderData();
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
            ws.Cells[1, 2].Value = "設備";
            ws.Cells[1, 3].Value = "單位";
            ws.Cells[1, 4].Value = "學號";
            ws.Cells[1, 5].Value = "姓名";
            ws.Cells[1, 6].Value = "卡號";
            ws.Cells[1, 7].Value = "備註";
            int int_state = 0;
            //Content
            for (int i = 0; i < this.logmaps.Count; i++)
            {
                ws.Cells[int_state + 2, 1].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", logmaps[i].CardTime);
                ws.Cells[int_state + 2, 2].Value = logmaps[i].EquName;
                ws.Cells[int_state + 2, 3].Value = logmaps[i].DepName;
                ws.Cells[int_state + 2, 4].Value = logmaps[i].PsnNo;
                ws.Cells[int_state + 2, 5].Value = logmaps[i].PsnName;
                ws.Cells[int_state + 2, 6].Value = logmaps[i].CardNo;
                ws.Cells[int_state + 2, 7].Value = this.OutList.Where(o => o.PsnNo.Trim() == logmaps[i].PsnNo).Count() > 0 ? "已申請請假" : "";
                int_state++;
            }
            ws.Cells[int_state + 2, 1].Value = "合計";
            ws.Cells[int_state + 2, 2].Value = logmaps.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryStayLog1.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    

        private void SetQuery()
        {            
            var param = new {PsnName = Request["CardNo_PsnName"]+"%",PsnNo=Request["CardNo_PsnName"],CardNo=Request["CardNo_PsnName"],
                EquDir = Request["EquDir"],
                Range = Request["Range"],
                CardTime = Request["Calendar1"],
                EgList = Builders[Request["Builds"]]
            };            
            var log1 = this.odo.GetQueryResult<CardLogModel>(this.MainQueryLogStr, param).ToList();
            this.OutList = this.odo.GetQueryResult<CardLogModel>(this.SecQueryLogStr, param).ToList();
            logmaps = log1;
            this.RowCount = log1.Count;
        }


        private void SetQueryByReport()
        {            
            var param = new
            {
                PsnName = Request["CardNo_PsnName"] + "%",
                PsnNo = Request["CardNo_PsnName"],
                CardNo = Request["CardNo_PsnName"],
                EquDir = Request["EquDir"],
                Range = Request["Range"],
                CardTime = this.CalendarFrm1.DateValue,EgList = Builders[this.ddlBuilder.SelectedValue]
            };
            var log1 = this.odo.GetQueryResult<CardLogModel>(this.MainQueryLogStr, param).ToList();
            this.OutList = this.odo.GetQueryResult<CardLogModel>(this.SecQueryLogStr, param).ToList();
            logmaps = log1;
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace