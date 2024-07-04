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

namespace SahoAcs
{
    public partial class QueryMasterLogZZ02 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<AliveState> MasterLog = new List<AliveState>();
        
        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        
        public IPagedList<AliveState> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                //this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "QueryOneLog")
            {
                //this.SetOneCardLog();
            }
            else
            {
                this.SetInitData();
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            if (!IsPostBack)
            {
               
            }
            ClientScript.RegisterClientScriptInclude("ZZ01", "QueryMasterLogZZ02.js?"+DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CreateTime) FROM B00_SysDeviceOpLog WHERE CreateTime <= GETDATE()");
            this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
            this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
            #endregion

            #region 設定欄位寬度、名稱內容預設
            //this.ListCols.Add(new ColNameObj() { ColName = "Company", DataWidth = 124, TitleWidth = 120, TitleName = "公司" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 124, TitleWidth = 120, TitleName = "寢室" });            
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 103, TitleWidth = 100,TitleName="姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 74, TitleWidth = 70,TitleName="卡號" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardDate", DataWidth = 123, TitleWidth = 120, TitleName = "刷卡日期" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 123, TitleWidth = 120, TitleName = "刷卡時間" });            
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
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");            
            string sql = "";
            sql = @"SELECT * FROM B00_SysDeviceOpLog WHERE DOPType='AliveState' AND CreateTime BETWEEN @DateS AND @DateE AND DOPActive IN ('MstAliveState','EquAliveState') ORDER BY CreateTime ";
            this.MasterLog = this.odo.GetQueryResult<AliveState>(sql, new {DateS=Request["CardDateS"],DateE=Request["CardDateE"]}).ToList();
            //this.ListLog = this.ListLog.Concat(this.ListLogOut).OrderByField(SortName, boolSort).ToList();            
            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                //this.DataResult = OrmDataObject.IEnumerableToTable(this.MasterLog.OrderByField(SortName,boolSort).ToList());
            }
            else
            {
                //轉datatable
                //this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                //this.DataResult = OrmDataObject.IEnumerableToTable(this.MasterLog.OrderByField(SortName,boolSort).ToList());
            }                   
        }


        private string GetConditionCmdStr()
        {
            string sql = "";                       
            //一般查詢的方法
            sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
            }
            if (Request["MgaName"] != null && Request["MgaName"] != "")
            {
                sql += " AND MgaName = @MgaName ";
            }
            if (Request["EquNo"] != null && Request["EquNo"] != "")
            {
                sql += " AND EquNo = @EquNo ";
            }
            if (Request["PsnID"] != "")
            {
                sql += " AND PsnNo IN (SELECT PsnNo FROM B01_Person WHERE PsnID=@PsnID)";
            }
            return sql;
        }



        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");


            ws.Cells[1, 1].Value = "斷連線類型";
            ws.Cells[1, 2].Value = "位置";
            ws.Cells[1, 3].Value = "時間";
            ws.Cells[1, 4].Value = "設備編號";
            ws.Cells[1, 5].Value = "斷連線資訊";
            //ws.Cells[1, this.ListCols.Count].Value = "進出類型";
            //Content
            for (int i = 0; i < this.MasterLog.Count; i++)
            {
                if (this.MasterLog[i].DOPActive.StartsWith("Mst"))
                {
                    var arr0 = MasterLog[i].ResultMsg.Split(',');
                    if (arr0.Length >= 2)
                    {
                        var arr1 = arr0[1].Split(';');
                        if (arr1.Length >= 2)
                        {
                            MasterLog[i].EquNo = arr1[0];
                            MasterLog[i].IPAddress = arr1[1];
                        }
                    }
                }
                ws.Cells[i + 2, 1].Value = this.MasterLog[i].DOPActive.StartsWith("Mst") ? "連線裝置" : "設備";
                ws.Cells[i + 2, 1].Value += this.MasterLog[i].DOPState == "0" ? "斷線" : "連線";
                ws.Cells[i + 2, 2].Value = this.MasterLog[i].IPAddress;
                ws.Cells[i + 2, 3].Value = this.MasterLog[i].CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                ws.Cells[i + 2, 4].Value = this.MasterLog[i].EquNo;
                ws.Cells[i + 2, 5].Value = this.MasterLog[i].ResultMsg;
            }
            //ws.Cells[this.DataResult.Rows.Count+2, 1, this.DataResult.Rows.Count+2, this.ListCols.Count].Merge = true;
            //ws.Cells[this.DataResult.Rows.Count + 2, 1].Value = string.Format("未出人數：{0}，未進人數：{1}",this.ListLog.Where(i=>i.EquDir=="進").Count(),this.ListLog.Where(i=>i.EquDir=="出").Count());
            ws.Cells[this.MasterLog.Count + 2, 1].Value = "總筆數";
            ws.Cells[this.MasterLog.Count + 2, 2].Value = this.MasterLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=DeviceOpLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {

            //轉datatable
            this.PagedList = this.MasterLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }
        


    }//end page class
}//end namespace