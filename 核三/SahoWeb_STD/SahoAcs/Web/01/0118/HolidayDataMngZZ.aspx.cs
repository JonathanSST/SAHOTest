using DapperDataObjectLib;
using PagedList;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.DBClass;


namespace SahoAcs.Web._01._0118
{
    public partial class HolidayDataMngZZ : System.Web.UI.Page
    {
        #region Global block
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public string SortName = "StartTime";
        public string SortType = "ASC";

        public List<B03PsnHoliday> ScheduleDatas = new List<B03PsnHoliday>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        #region 分頁參數
        public int PageIndex = 1;
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<B03PsnHoliday> PagedList;
        #endregion End 分頁參數


        #endregion end Global block


        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (this.GetFormEndValue("PageEvent").Equals("Query"))
            {
                //按下查詢時 query 結果
                //this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (this.GetFormEndValue("PageEvent").Equals("QueryPerson"))
            {
                this.SetQueryPerson();
            }

            if (!IsPostBack)
            {
                this.QueryTimeS.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                this.QueryTimeE.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
            }


            ClientScript.RegisterClientScriptInclude("JsInclude1", "HolidayDataMngZZ.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsInclude2", "HolidayEdit.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }// End Page_load

        private void SetQueryPerson()
        {
            if (!this.GetFormEndValue("PsnName").Equals(""))
            {
                string sqlcmd = "SELECT TOP 200 B01_Person.* FROM B01_Person INNER JOIN B00_ClassData ON Text5=CNo AND CName LIKE '工讀生%' WHERE PsnNo LIKE @Key ORDER BY PsnNo";
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = this.GetFormEndValue("PsnName") + "%" }).ToList();
                sqlcmd = "SELECT TOP 200 B01_Person.* FROM B01_Person INNER JOIN B00_ClassData ON Text5=CNo AND CName LIKE '工讀生%' WHERE PsnName LIKE @Key ORDER BY PsnName";
                this.PersonList = this.PersonList.Union(this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = this.GetFormEndValue("PsnName") + "%" })).ToList();
                this.PersonList = this.PersonList.GroupBy(i => i.PsnNo).Select(i => i.First()).ToList();
            }
            else
            {
                this.PersonList = this.odo.GetQueryResult<PersonEntity>("SELECT TOP 200 B01_Person.* FROM B01_Person INNER JOIN B00_ClassData ON Text5=CNo AND CName LIKE '工讀生%' WHERE PsnNo LIKE '%' ORDER BY PsnNo").ToList();
            }

        }

        private void SetQueryData()
        {
            
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            
            string QueryPsnsql = @"SELECT H.*,P.PsnName,VName AS HoliNo FROM B03_PsnHoliday H 
                                            INNER JOIN B01_Person P ON H.PsnNo=P.PsnNo 
                                            INNER JOIN B00_VacationData PH ON H.HoliNo=PH.VNo
                                            WHERE KeyIn<>'' AND ((StartTime BETWEEN @QryTimeS AND @QryTimeE) OR (EndTime BETWEEN @QryTimeS AND @QryTimeE))";
            bool boolSort = this.SortType.Equals("ASC");
            this.ScheduleDatas = this.odo.GetQueryResult<B03PsnHoliday>(QueryPsnsql, new {PsnNo=Sa.Web.Fun.GetSessionStr(this,"PsnNo"), QryTimeS=Request["QryTimeS"].ToString(), QryTimeE=Request["QryTimeE"].ToString()}).ToList();
            //this.ScheduleDatas.ForEach(i => i.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName"));
            this.PagedList = this.ScheduleDatas.ToPagedList(PageIndex, 100);
        }

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

        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion


            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "2道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 104, TitleWidth = 100, TitleName = "2道出廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "1道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 104, TitleWidth = 100, TitleName = "人員ID" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 124, TitleWidth = 120, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 124, TitleWidth = 120, TitleName = "廠商名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "廠商編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 124, TitleWidth = 120, TitleName = "工程編號" });
            foreach (var o in this.ListCols.Where(i => i.DataRealName == null || i.DataRealName == ""))
                o.DataRealName = o.ColName;

            #endregion            
        }

     
    } // End page class 
} // End namespace