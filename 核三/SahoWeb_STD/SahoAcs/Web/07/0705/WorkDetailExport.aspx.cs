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
using System.Configuration;
using CPCWorkDetail;
using System.Text;
using System.IO;

namespace SahoAcs.Web._07._0705
{
    public partial class WorkDetailExport : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<B03WorkDetail> ListLog = new List<B03WorkDetail>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public string txt_CardNo_PsnName = "";
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public string SortName = "WorkDate";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;
        public string PsnNo = "", PsnID = "", PsnName = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string AuthList = "";
        public IPagedList<B03WorkDetail> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
               
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Calc")
            {
                this.SetCalcWork();
            }            
            else
            {
                this.SetInitData();
                if (Request.UrlReferrer != null && (Request.UrlReferrer.PathAndQuery.Contains("Default.aspx") || Request.UrlReferrer.PathAndQuery.Equals("/")))
                {
                    this.SetQueryData();
                }
            }
            
            ClientScript.RegisterClientScriptInclude("WorkDetailExport", "WorkDetailExport.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            //ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案

        }

        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID = this.hideUserID.Value }).OrderBy(i => i.PsnName).ToList();
            //查詢組織相關資料
            this.OrgDataInit = this.odo.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>("SELECT * FROM OrgStrucAllData('Unit') WHERE OrgNo<>''").ToList();
            #endregion

            #region 給開始、結束時間預設值
            this.CalcDayS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            this.CalcDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            #endregion
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
        }


        public void SetCalcWork()
        {
            string WorkDate = (Request["WorkDate"] ?? this.CalcDayS.DateValue);
            string WorkDateE = this.GetFormEqlValue("WorkDateE") ?? this.CalcDayE.DateValue;
            string Message = "";
            bool result = false;
            if (Convert.ToDateTime(WorkDateE) > DateTime.Now)
            {
                Message = "匯出日期只能小於或等於當天日期!!";
                return;
            }
            if (WorkDate.CompareTo(WorkDateE) > 0)
            {
                Message = "起始日期不得大於結束日期";
                return;
            }
            try
            {
                CPCWorkDetail.CalcFunc func1 = new CPCWorkDetail.CalcFunc();
                //設定相關的參數
                func1.UseMode = "1";
                func1.WaitMin = int.Parse(ConfigurationManager.AppSettings["WaitMin"]);
                func1.OverMin = int.Parse(ConfigurationManager.AppSettings["OverMin"]);
                func1.CustomType = 1;
                func1.SWTag = "ABCR";
                func1.SWTagList = "1,04:00,06:00;2,12:00,13:00;3,18:00,19:00;0,00:00,00:00";
                func1.LogFileRoot = ConfigurationManager.AppSettings["LogFileRoot"];
                func1.LogKeepDay = 100;
                func1.DbUserID = ConfigurationManager.AppSettings["DbUserID"];
                func1.DbUserPW = ConfigurationManager.AppSettings["DbUserPW"];
                func1.DbDataBase = ConfigurationManager.AppSettings["DbDataBase"];
                func1.DbDataSource = ConfigurationManager.AppSettings["DbDataSource"];
                func1.Start();
                result = func1.CalcCPCWorkDetail(WorkDate, WorkDateE);
                func1.Stop();
                func1.Close();
                func1 = null;
            }
            catch (Exception ex)
            {
                result = false;
                Message = ex.Message;
            }
            finally
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = Message, success = result }));
                Response.End();
            }


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
            
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            sql = @"SELECT DISTINCT WD.*,P.PsnName FROM B03_WorkDetail WD
                                   INNER JOIN B01_Person P ON WD.PsnNo=P.PsnNo
                                  INNER JOIN B01_MgnOrgStrucs B ON P.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                WHERE WorkDate BETWEEN @DateS AND @DateE  AND UserID=@UserID ";
            sql += " ORDER BY PsnNo,WorkDate ";
            /* DATEDIFF(HOUR,InTime2nd,ISNULL(OutTime2nd,GETDATE()))>1 */
            this.ListLog = this.odo.GetQueryResult<B03WorkDetail>(sql, new
            {
                DateS = this.GetFormEqlValue("DateS"),
                DateE = this.GetFormEqlValue("DateE"), UserID=Sa.Web.Fun.GetSessionStr(this,"UserID")
            }).OrderByField("WorkDate", true).ToList();


            //this.ListLog = TempData.Where(i=>i.InTime2nd.;            
            if (Request["PageEvent"] == "Print")
            {
                //this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }
        

        private void EmptyCondition()
        {
            //轉datatable
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
        }
    }
}