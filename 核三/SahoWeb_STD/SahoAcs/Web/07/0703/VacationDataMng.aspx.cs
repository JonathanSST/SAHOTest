using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using PagedList;


namespace SahoAcs._0703
{
    public partial class VacationDataMng : System.Web.UI.Page
    {
        #region Main Description
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        private int _pagesize = 50;        //DataGridView每頁顯示的列數
        private static string MsgDuplicate = "";
        public IPagedList<dynamic> PagedList;
        public string SortName = "VNo";
        public string SortType = "ASC";
        #region 分頁設定
        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        #endregion

        #endregion


        #region LoadProcess
        private void LoadProcess()
        {            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("SelfScript", "VacationDataMng.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "YYData();", true);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            //設定DataGridView每頁顯示的列數
            //this.MainGridView.PageSize = _pagesize;
        }
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            MsgDuplicate = this.GetLocalResourceObject("MsgDouble").ToString();
            hUserId.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            //hMenuNo.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuNo");
            //hMenuName.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuName");
            LoadProcess();
            RegisterObj();

            if (!IsPostBack)
            {
                if (this.GetFormEqlValue("PageEvent").Equals("Query"))
                {
                    //Query(true, this.SortName, this.SortType);
                }
                if (this.GetFormEqlValue("PageEvent").Equals("Add"))
                {
                    this.Insert();
                }
                if (this.GetFormEqlValue("PageEvent").Equals("Update"))
                {
                    this.Update();
                }
                if (this.GetFormEndValue("PageEvent").Equals("Delete"))
                {
                    this.Delete();
                }
                if (this.GetFormEndValue("PageEvent").Equals("Edit"))
                {
                    this.LoadData();
                }
                Query(true, this.SortName, this.SortType);
            }
            else
            {

            }
        }
        #endregion
        

        #region 其他方法


        


        #region 記載查詢條件的紀錄，防止頁數按鈕切換時查詢錯誤
        private void CatchSession(List<String> Data)
        {
            String datalist = "";
            for (int i = 0; i < Data.Count; i++)
                datalist += Data[i] + "|";
            Session["OldSearchList"] = datalist;
        }
        #endregion

        #region LimitText

        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);
            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis) len -= 3;

                string res = big5.GetString(b, 0, len);
                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);
                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion


        #endregion

        #region 查詢

        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            int iPageIndex = 1;
            string sql = "";            
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String NowCondition = "", NowYear = "", wheresql = "";

            
            #region Process String
            sql = " SELECT * FROM B00_VacationData ORDER BY " + SortExpression + " " + SortDire ;           
            #endregion
            var result = this.odo.GetQueryResult(sql);
            if (!this.GetFormEqlValue("Index").Equals(""))
            {
                PageIndex = int.Parse(this.GetFormEqlValue("Index"));
            }
            this.PagedList = result.ToPagedList(PageIndex, _pagesize);
            hDataRowCount.Value = result.Count().ToString();
            SetDoPage();
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

        #endregion

        #region Insert、Update、Delete 資料
        public void Insert()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            string sql = @" INSERT INTO B00_VacationData(VNo,VName,CreateUserID,CreateTime) VALUES 
                                    (@VNo, @VName, @User, GETDATE())";
            var effectRow = 0;            
            Pub.MessageObject sRet = new Pub.MessageObject();
            if (this.odo.GetQueryResult("SELECT * FROM B00_VacationData WHERE VNo=@VNo", new {VNo = this.GetFormEqlValue("VNo") }).Count() > 0)
            {
                sRet.result = false;
                sRet.message = "假別代碼重複使用";
            }
            if (sRet.result)
            {
                effectRow = this.odo.Execute(sql, new
                {
                    User = UserID,
                    VNo = this.GetFormEqlValue("VNo"),
                    VName = this.GetFormEqlValue("VName")
                });
                if (effectRow > 0)
                {
                    sRet.result = true;
                    sRet.message = "新增完成";
                }
                else
                {
                    sRet.result = false;
                    sRet.message = "新增失敗，資料庫異常！";
                }
            }          
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }

        public void Update()
        {            
            string UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            string sql = " UPDATE B00_VacationData SET VNo=@vNo, VName=@VName,UpdateUserID=@User, UpdateTime=GETDATE() WHERE VID=@VID";
            Pub.MessageObject sRet = new Pub.MessageObject();
            if (this.odo.GetQueryResult("SELECT * FROM B00_VacationData WHERE VNo=@VNo AND VID<>@VID", new {VID=this.GetFormEqlValue("VID"), VNo=this.GetFormEqlValue("VNo")}).Count() > 0)
            {
                sRet.result = false;
                sRet.message = "假別代碼重複使用";                
            }
            if (sRet.result)
            {
                var effectRow = this.odo.Execute(sql, new
                {
                    User = UserID,
                    VNo = this.GetFormEqlValue("VNo"),                    
                    VID = this.GetFormEqlValue("VID"),
                    VName = this.GetFormEqlValue("VName")
                });
                if (effectRow > 0)
                {
                    sRet.result = true;
                    sRet.message = "修改完成";
                }
                else
                {
                    sRet.result = false;
                    sRet.message = "修改失敗，資料庫異常！";
                }                
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }

        public void Delete()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            string sql = " Delete B00_VacationData WHERE VID=@VID";
            var effectRow = this.odo.Execute(sql, new {User = UserID, VID = this.GetFormEndValue("VID") });
            Pub.MessageObject sRet = new Pub.MessageObject();
            if (effectRow > 0)
            {
                sRet.result = true;
                sRet.message = "刪除完成";
            }
            else
            {
                sRet.result = false;
                sRet.message = "刪除失敗，資料庫異常！";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }
        #endregion

        #region 帶入資料進行編輯
        public void LoadData()
        {
            Response.Clear();
            Pub.MessageObject sRet = new Pub.MessageObject();
            sRet.result = false;
            var result = this.odo.GetQueryResult("SELECT * FROM B00_VacationData WHERE VID=@Vid", new { Vid = this.GetFormEndValue("VID") });            
            dynamic obj = null;
            if (result.Count() > 0)
            {
                sRet.result = true;
                obj = result.First();
            }
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { result = sRet.result, cdata = obj });
            Response.Write(json);
            Response.End();
        }
        #endregion

        #region GridView處理
       
        

        #endregion

        #region JavaScript及aspx共用方法
        

        #endregion


    }//end page class
}//end namespace