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
using PagedList;
using OfficeOpenXml;


namespace SahoAcs.Web._06._0601
{
    public partial class _0601Rpt : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogModel> ListLog = new List<CardLogModel>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");
            List<ColNameObj> collist = new List<ColNameObj>();
            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "CardTimeVal", DataWidth = 123, TitleWidth = 120, TitleName = "讀卡時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = "部門名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 84, TitleWidth = 80, TitleName = "人員編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "人員姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 74, TitleWidth = 70, TitleName = "卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "TempCardNo", DataWidth = 74, TitleWidth = 70, TitleName = "臨時卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardVer", DataWidth = 64, TitleWidth = 60, TitleName = "卡片版本" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "設備編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 104, TitleWidth = 100, TitleName = "設備名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 104, TitleWidth = 100, TitleName = "讀卡結果" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogTimeVal", DataWidth = 154, TitleWidth = 150, TitleName = "記錄時間" });
            #endregion
            foreach (var s in Request.QueryString.GetValues("ColName"))
            {
                //this.ListCols.Add(s);
                collist.Add(this.ListCols.Where(i => i.ColName == s).FirstOrDefault());
            }
            this.ListCols = collist;
            this.txt_CardNo_PsnName = Request["CardNoPsnNo"];

            string sql = "";
            string sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            var EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") });
            sql = @" SELECT * FROM B01_CardLog WHERE (EquNo IN @EquList)";
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            //一般查詢的方法
            sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
            }
            if (Request["LogStatus"] != null && Request["LogStatus"] != "")
            {
                sql += " AND LogStatus IN @LogStatus ";
            }
            this.ListLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                EquList = EquDatas.Select(i => i.EquNo),
                PsnName = "%" + this.txt_CardNo_PsnName + "%",
                CardTimeS = Request["CardTimeS"],
                CardTimeE = Request["CardTimeE"],
                LogStatus = Request["LogStatus"].Split(',')
            }).ToList();
            //轉datatable
            this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            this.DataResult.Columns.Add(new DataColumn("CardTimeVal"));
            this.DataResult.Columns.Add(new DataColumn("LogTimeVal"));

            foreach (DataRow r in this.DataResult.Rows)
            {
                r["CardTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["CardTime"]));
                r["LogTimeVal"] = string.Format("{0:yyyy/MM/dd HH:mm:ss}", Convert.ToDateTime(r["LogTime"]));
                r["LogStatus"] = logstatus.Where(i => Convert.ToInt32(i.Code) == Convert.ToInt32(r["LogStatus"])).FirstOrDefault().StateDesc;
            }

            for(int i = 0; i < this.ListCols.Count; i++)
            {
                ws.Cells[1, i+1].Value = this.ListCols[i].TitleName;
            }
           
            //Content
            for (int i = 0;i< this.DataResult.Rows.Count; i++)
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



    }
}