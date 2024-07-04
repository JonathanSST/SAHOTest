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
using System.Web.Script.Serialization;


namespace SahoAcs
{
    public partial class CountDateDefine : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogTrt> ListLog = new List<CardLogTrt>();
        public List<dynamic> StoryLog = new List<dynamic>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
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

        public IPagedList<CardLogTrt> PagedList;

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
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "SetCountDate")
            {
                this.SetCountDate();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "GetCountData")
            {
                this.SetParaCountData();
            }
            else
            {
                this.SetInitData();
                this.SetQueryData();
            }
            if (!IsPostBack)
            {
               
            }
            ClientScript.RegisterClientScriptInclude("0639", "CountDateDefine.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            #region 給開始、結束時間預設值
            this.CalendarSet.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
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

      

        private void SetCountDate()
        {
            string sqlcmd = @"INSERT INTO B00_SysParameter (ParaClass, ParaNo,ParaName, ParaValue, ParaType, ParaDesc, CreateUserID, CreateTime) 
                                        VALUES ('CardPicShow', 'BCountDate', '開始計數日期', @ParaValue, 'String', '未回廠開始計算日期', 'UpdVer', GETDATE())";
            if (this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='BCountDate' AND ParaClass='CardPicShow' ").Count() > 0)
            {
                sqlcmd = "UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@UserID,UpdateTime=GETDATE() WHERE ParaNo='BCountDate' AND ParaClass='CardPicShow' ";
            }
            var ParaData = new { ParaValue = Request["CountDate"], UserID = Request["UserID"] };
            //設定啟始時間的參數
            this.odo.Execute(sqlcmd, ParaData);
            //設定異動紀錄            
            this.odo.Execute("INSERT INTO B03_CountDateStory (CountDate,CreateUserID,CreateTime) VALUES (@ParaValue,@UserID, GETDATE())", ParaData);
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                success = true, message=this.odo.DbExceptionMessage
            }));
            Response.End();
        }

        private void SetParaCountData()
        {
            string TotalCount = "0";
            string TodayCount = "0";
            var Result = this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('OutTodayCount','OutTotalCount') AND ParaClass='CardPicShow' ");
            foreach(var o in Result)
            {
                if (Convert.ToString(o.ParaNo) == "OutTodayCount")
                    TodayCount = Convert.ToString(o.ParaValue);
                if (Convert.ToString(o.ParaNo) == "OutTotalCount")
                {
                    TotalCount = Convert.ToString(o.ParaValue);
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                success = true,
                today_count = TodayCount,
                total_count=TotalCount,
                message = this.odo.DbExceptionMessage
            }));
            Response.End();
        }

        private void SetQueryData()
        {
            this.StoryLog = this.odo.GetQueryResult("SELECT * FROM B03_CountDateStory ORDER BY CreateTime DESC").ToList();
        }        

        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");            
            ws.Cells[1, 1].Value = "讀卡日期";
            ws.Cells[1, 2].Value = "人員編號";
            ws.Cells[1, 3].Value = "人員名稱";            
            ws.Cells[1, 4].Value = "部門";
            ws.Cells[1, 5].Value = "卡片號碼";
            ws.Cells[1, 6].Value = "出廠時間";
            ws.Cells[1, 7].Value = "入廠時間";
            //Content
            for (int i = 0; i < this.ListLog.Count; i++)
            {
                ws.Cells[i+2, 1].Value = string.Format("{0:yyyy/MM/dd}",this.ListLog[i].CardTime);
                ws.Cells[i + 2, 2].Value = this.ListLog[i].PsnNo;
                ws.Cells[i+2, 3].Value = this.ListLog[i].PsnName;                
                ws.Cells[i+2, 4].Value = this.ListLog[i].DepName;
                ws.Cells[i+2, 5].Value = this.ListLog[i].CardNo;
                ws.Cells[i + 2, 6].Value = this.ListLog[i].First;
                ws.Cells[i + 2, 7].Value = this.ListLog[i].Last;
                //i++;
            }
            ws.Cells[this.ListLog.Count + 2, 1].Value = "總筆數：";
            ws.Cells[this.ListLog.Count + 2, 2].Value = this.ListLog.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryInOutList.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        


        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                
            }
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }
        


    }//end page class
}//end namespace