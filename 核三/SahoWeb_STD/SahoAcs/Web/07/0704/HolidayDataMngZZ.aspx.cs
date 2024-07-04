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


namespace SahoAcs.Web._07._0704
{
    public partial class HolidayDataMngZZ : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public string SortName = "StartTime";
        public string SortType = "ASC";

        public List<B03PsnHoliday> ScheduleDatas = new List<B03PsnHoliday>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<PersonEntity> PersonList = new List<PersonEntity>();

        public int PageIndex = 1;
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<B03PsnHoliday> PagedList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.GetFormEndValue("PageEvent").Equals("Query"))
            {
                //按下查詢時 query 結果
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
                this.QueryTimeE.DateValue = DateTime.Now.AddDays(14).ToString("yyyy/MM/dd 23:59:59");
                this.PagedList = this.ScheduleDatas.ToPagedList(PageIndex, 1);
            }


            ClientScript.RegisterClientScriptInclude("JsInclude1", "HolidayDataMngZZ.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsInclude2", "HolidayEdit.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        private void SetQueryPerson()
        {
            if (!this.GetFormEndValue("PsnName").Equals(""))
            {
                string sqlcmd = "SELECT TOP 200 B01_Person.* FROM B01_Person Left JOIN B00_ClassData ON Text5=CNo Where PsnAuthAllow = 1 And PsnNo LIKE @Key ORDER BY PsnNo";
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = this.GetFormEndValue("PsnName") + "%" }).ToList();
                sqlcmd = "SELECT TOP 200 B01_Person.* FROM B01_Person Left JOIN B00_ClassData ON Text5=CNo Where PsnAuthAllow = 1 And PsnName LIKE @Key ORDER BY PsnName";
                this.PersonList = this.PersonList.Union(this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = this.GetFormEndValue("PsnName") + "%" })).ToList();
                this.PersonList = this.PersonList.GroupBy(i => i.PsnNo).Select(i => i.First()).ToList();
            }
            else
            {
                this.PersonList = this.odo.GetQueryResult<PersonEntity>("SELECT TOP 200 B01_Person.* FROM B01_Person Left JOIN B00_ClassData ON Text5=CNo Where PsnAuthAllow = 1 And PsnNo LIKE '%' ORDER BY PsnNo").ToList();
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
            if (!string.IsNullOrEmpty(Sa.Web.Fun.GetSessionStr(this, "PsnNo")))
            {
                QueryPsnsql += " And H.PsnNo=@PsnNo";
            }
            bool boolSort = this.SortType.Equals("ASC");
            this.ScheduleDatas = this.odo.GetQueryResult<B03PsnHoliday>(QueryPsnsql, 
                new {
                    PsnNo = Sa.Web.Fun.GetSessionStr(this, "PsnNo"),
                    QryTimeS = Request["QryTimeS"].ToString(),
                    QryTimeE = Request["QryTimeE"].ToString()
                }).ToList();
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
    }
}