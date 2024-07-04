using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class TimeTableSetting : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("TimeTableSetting", "TimeTableSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            popB_Save.Attributes["onClick"] = "__doPostBack(popB_Save.id, ''); return false;";
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_EquParaDef", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["RoleNo"].ToString();
            //Label_Name.Text = TableInfo["RoleName"].ToString();
            //Label_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_No.Text = TableInfo["RoleNo"].ToString();
            //popLabel_Name.Text = TableInfo["RoleName"].ToString();
            //popLabel_EName.Text = TableInfo["RoleEName"].ToString();
            //popLabel_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_Desc.Text = TableInfo["RoleDesc"].ToString();
            //popLabel_Remark.Text = TableInfo["Remark"].ToString();
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value =  Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                hideMode.Value =  Request.QueryString["Mode"].ToString();
                #endregion
                
                if (string.Compare(hideMode.Value, "MControl") == 0)
                    TimeBlock_Legend.InnerText = "管制模式時區設定";
                else if (string.Compare(hideMode.Value, "CardReader") == 0)
                    TimeBlock_Legend.InnerText = "刷卡模式時區設定";
               
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    CreateDropItem();
                    if (!string.IsNullOrEmpty(hideParaValue.Value))
                        this.Input_Mode.SelectedValue = hideParaValue.Value;
                }
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + this.Input_Mode.SelectedValue + "';");
            Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion

        #region Method

        #region CreateDropItem
        private void CreateDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            this.Input_Mode.Items.Clear();

            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- 請選擇 -";
            Item.Value = "";
            this.Input_Mode.Items.Add(Item);

            #region Process String
            sql = @" SELECT * FROM B01_TimeTableDef AS TimeTable
                     LEFT JOIN B01_EquData AS EquData ON EquData.EquModel = TimeTable.EquModel
                     WHERE EquData.EquID = ? AND TimeType = ? ";
            liSqlPara.Add("S:" + hideEquID.Value);
            liSqlPara.Add("S:" + hideMode.Value);
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = "(" + dr["TimeNo"].ToString() + ") " + dr["TimeName"].ToString();
                Item.Value = dr["TimeID"].ToString();
                this.Input_Mode.Items.Add(Item);
            }
        }
        #endregion

        #endregion
    }
}
