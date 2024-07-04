using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class SysParam : System.Web.UI.Page
    {
        #region Main Description        
        private int _pagesize = 13;     //100
        static string UserID = "";
        static string NonData = "";
        private string sMenuNo = "", sMenuName = "", sFunAuthSet = "";
        public string OrderName = "ParaClass", OrderType = "ASC";
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<SysParaData> ListParaData = new List<SysParaData>();
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess            
            //oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";
            js += "<script src='SysParam.js?"+Pub.GetNowTime+"'></script>";
            js += "<script type='text/javascript'>SetMode('');</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);            
            //ClientScript.RegisterClientScriptInclude("SysParam", "SysParam.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            //AddButton.Attributes["onClick"] = "CallAdd(); return false;";            
            //DeleteButton.Attributes["onClick"] = "CallDelete(); return false;";
            //AuthButton.Attributes["onClick"] = "CallAuth(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            
            #endregion
            
            
            #endregion //end LoadProcess
            NonData = this.GetLocalResourceObject("ttNonData").ToString();
            if (!IsPostBack)
            {                
                UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                if (Session["MenuNo"] != null) sMenuNo = Session["MenuNo"].ToString();
                if (Session["MenuName"] != null) sMenuName = Session["MenuName"].ToString();
                if (Session["FunAuthSet"] != null) sFunAuthSet = Session["FunAuthSet"].ToString();
                if (Request["DoAction"] == null)
                {
                    this.Query();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Query")
                {
                    this.Query();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Edit")
                {
                    this.LoadData();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Update")
                {
                    this.Update();   
                }

            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        
        #endregion

        #region Method
        #region LimitText
        public string LimitText(string str, int len)
        {
            if (str.Length > len)
            {
                return str.Substring(0, len) + "...";
            }
            else
            {
                return str;
            }
        }
        #endregion

        #region Query
        public void Query()
        {
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            
            #region Process String
            sql = " SELECT * FROM B00_SysParameter WHERE ParaClass <> @ParaClass";
            string para_class = "HideSystem";
            if (Request["ParaClass"] != null)
                para_class = Request["ParaClass"];
            this.ListParaData = this.odo.GetQueryResult<SysParaData>(sql, new { ParaClass = para_class }).ToList();
            if (Request["SortName"] != null)
                this.OrderName = Request["SortName"];
            if (Request["SortType"] != null)
                this.OrderType = Request["SortType"];
            this.ListParaData = this.ListParaData.OrderByField(this.OrderName, OrderType == "ASC").ToList();
            //this.ListParaData=this.ListParaData.
            //if (wheresql != "")            
            
            //sql += wheresql + " ORDER BY ParaClass, ParaNo ";
            //sql += wheresql + " ORDER BY " + this.OrderName + " " + SortDire;
            #endregion
        }
        #endregion  //end Query

        #region Query(string mode)
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B00_SysParameter  ";

            //if (wheresql != "")
            sql += " WHERE ParaClass <> ? ";
            liSqlPara.Add("S:HideSystem");

            //sql += wheresql + " ORDER BY ParaClass, ParaNo ";
            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("SysParamTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            
            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["ParaNo"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
        }
        #endregion  //end Query(string mode)

        #region LoadData        
        public void LoadData()
        {
            string RecordID = "";
            if (Request["ParaID"] != null)
            {
                RecordID = Request["ParaID"];
            }
            var DataResult = this.odo.GetQueryResult<SysParaData>("SELECT * FROM B00_SysParameter WHERE RecordID=@RecordID", new {RecordID=RecordID});
            SysParaData entity = null;
            bool IsOk = false;
            string message = "OK";            
            message = this.odo.DbExceptionMessage;
            if (DataResult.Count() > 0)
            {
                entity = DataResult.First();
                IsOk = true;
            }
            else
            {
                entity = new SysParaData();
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
            new { success = IsOk, resp = entity, message = message }));
            Response.End();
        }
        #endregion

        #region Update        
        public void Update()
        {            
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            
            objRet = Check_Input_DB(Request["ParaValue"], "Update");

            #region Update 系統參數 設定值
            if (objRet.result)
            {
                #region Process String - Updata SysParam
                sql = @" UPDATE B00_SysParameter SET
                         ParaValue = @ParaValue ,
                         UpdateUserID = @UserID , 
                         UpdateTime = GETDATE()
                         WHERE RecordID=@ParaID ";
                
                #endregion
                this.odo.Execute(sql, new {ParaID=Request["ParaID"],ParaValue=Request["ParaValue"],UserID=this.hUserId.Value});
                objRet.result = this.odo.isSuccess;
            }
            #endregion
            objRet.act = "Edit";
            objRet.message = Request["ParaValue"];
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objRet));
            Response.End();
        }
        #endregion  //end Update

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string P_Value, string mode)
        {
            //DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            //string sql = "";
            //List<string> liSqlPara = new List<string>();
            //Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(P_Value.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設定值 必須輸入";
            }
            //else if (P_Value.Trim().Count() > 20)
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
            //    objRet.result = false;
            //    objRet.message += "設定值 字數超過上限";
            //}
            #endregion

            return objRet;
        }
        #endregion  //end Check_Input_DB
                
        #endregion  //end Method
    }
}