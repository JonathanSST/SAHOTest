using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml;
using System.Data;

using PagedList;
using DapperDataObjectLib;

using SahoAcs.DBModel;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class WorkHour24 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<B03WorkAbnormal> ListLog = new List<B03WorkAbnormal>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public string txt_CardNo_PsnName = "";

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
        public IPagedList<B03WorkAbnormal> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("jsfun", "WorkHour24.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            if (this.GetFormEndValue("PageEvent").Equals("Query"))
            {
                PageIndex = Convert.ToInt32(this.GetFormEndValue("PageIndex"));
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (this.GetFormEndValue("PageEvent").Equals("QueryOneLog"))
            {
                this.SetQueryOneLog();
            }
            else if(this.GetFormEndValue("PageEvent").Equals("Save"))
            {
                this.SetClearData();
            }
            else
            {
                this.SetQueryData();
                this.SetDoPage();
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

        public void SetQueryData()
        {
            this.ListLog = this.odo.GetQueryResult<B03WorkAbnormal>(@"SELECT WD.*,P.PsnName FROM B03_WorkAbnormal WD
                                   INNER JOIN B01_Person P ON WD.PsnNo=P.PsnNo
                                  INNER JOIN B01_MgnOrgStrucs B ON P.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                WHERE UserID=@UserID AND IsPass=0 ORDER BY WorkDate DESC,PsnNo", new { UserID = Sa.Web.Fun.GetSessionStr(this, "UserID") }).ToList();
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);

        }

        void SetClearData()
        {
            this.odo.Execute("UPDATE B03_WorkAbnormal SET IsPass=1,UpdateTime=GETDATE(),UpdateUserID=@UserID WHERE RecordID=@RecordID",
                new { RecordID = this.GetFormEqlValue("RecordID"), UserID = Sa.Web.Fun.GetSessionStr(this, "UserID") });
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new {success=this.odo.isSuccess, message=this.odo.DbExceptionMessage}));
            Response.End();
        }

        void SetQueryOneLog()
        {
            B03WorkAbnormal entity = new B03WorkAbnormal();
            foreach (var o in this.odo.GetQueryResult<B03WorkAbnormal>(@"SELECT WD.*,P.PsnName FROM B03_WorkAbnormal WD 
                                                                    INNER JOIN B01_Person P ON WD.PsnNo=P.PsnNo WHERE RecordID=@RecordID", new { RecordID = this.GetFormEndValue("RecordID") }))
            {
                entity = o;
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(entity));
            Response.End();
        }

        public virtual bool onlyAcceptHttpPost()
        {
            return true;
        }
    }//end page class
}//end namespace