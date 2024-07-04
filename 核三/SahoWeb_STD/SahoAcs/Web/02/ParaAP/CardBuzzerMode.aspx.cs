using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class CardBuzzerMode : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        #endregion

        #region Events

        protected void Page_InitComplete(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);

            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardBuzzerMode", "CardBuzzerMode.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
            //ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "LoadData", "<script type='text/javascript'>LoadData();</script>");
            //Sa.Web.Fun.RunJavaScript(this.Page, "LoadData();");

            #region 註冊Button動作
            popB_Save.Attributes["onClick"] = "SaveBuzzerCard(); return false;";
            this.popInput_CardFormat0.Attributes["onClick"] = "RadioSelected('" + popInput_CardFormat0.ClientID + "')";
            this.popInput_CardFormat1.Attributes["onClick"] = "RadioSelected('" + popInput_CardFormat1.ClientID + "')";
            this.popInput_CardFormat2.Attributes["onClick"] = "RadioSelected('" + popInput_CardFormat2.ClientID + "')";
            this.popInput_CardFormat3.Attributes["onClick"] = "RadioSelected('" + popInput_CardFormat3.ClientID + "')";
            this.popInput_Buzzer0.Attributes["onClick"] = "RadioSelected('" + popInput_Buzzer0.ClientID + "')";
            this.popInput_Buzzer1.Attributes["onClick"] = "RadioSelected('" + popInput_Buzzer1.ClientID + "')";
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
        }

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    LoadData();
                }
            }
        }
        #endregion

        #endregion

        #region Method

        #region LoadData
        public void LoadData()
        {
            string CardFormatstr, Buzzerstr;
          
            if (!string.IsNullOrEmpty(hideParaValue.Value))
            {
                CardFormatstr = hideParaValue.Value.ToString().Substring(0, 2);
                Buzzerstr = hideParaValue.Value.ToString().Substring(2, 2);
                #region SetRadio
                switch (CardFormatstr)
                {
                    case "00":
                        this.popInput_CardFormat0.Checked = true;
                        break;
                    case "01":
                        this.popInput_CardFormat1.Checked = true;
                        break;
                    case "02":
                        this.popInput_CardFormat2.Checked = true;
                        break;
                    case "03":
                        this.popInput_CardFormat3.Checked = true;
                        break;
                }

                switch (Buzzerstr)
                {
                    case "00":
                        this.popInput_Buzzer0.Checked = true;
                        break;
                    case "AA":
                        this.popInput_Buzzer1.Checked = true;
                        break;
                }
                #endregion
                UpdatePanel1.Update();
            }
        }
        #endregion

        #region SaveData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SaveData(string UserID, string EquID, string EquParaID, string CardFormatValue, string BuzzerValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            string CardBuzzerStr = "";
            int Decide;
            List<string> liSqlPara = new List<string>();
            DateTime Time = DateTime.Now;
            Pub.MessageObject objRet = new Pub.MessageObject();

            objRet = Check_Input(CardFormatValue, BuzzerValue);

            if (objRet.result)
            {
                CardBuzzerStr = CardFormatValue + BuzzerValue;
                //                #region 判斷動作
                //                sql = @" SELECT COUNT(*) FROM B01_EquParaData ";

                //                if (!string.IsNullOrEmpty(EquID.Trim()))
                //                {
                //                    if (wheresql != "") wheresql += " AND ";
                //                    wheresql += " EquID = ? ";
                //                    liSqlPara.Add("S:" + EquID.Trim());
                //                }

                //                if (!string.IsNullOrEmpty(EquParaID.Trim()))
                //                {
                //                    if (wheresql != "") wheresql += " AND ";
                //                    wheresql += " EquParaID = ? ";
                //                    liSqlPara.Add("S:" + EquParaID.Trim());
                //                }

                //                if (wheresql != "")
                //                    sql += " WHERE ";
                //                sql += wheresql;

                //                Decide = oAcsDB.GetIntScalar(sql, liSqlPara);
                //                #endregion

                //                #region Process ParaData
                //                if (Decide <= 0)
                //                {
                //                    #region Insert
                //                    liSqlPara.Clear();

                //                    sql = @" INSERT INTO B01_EquParaData(EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime)
                //                             VALUES (?, ?, ?, ?, ?) ";
                //                    liSqlPara.Add("I:" + EquID.Trim());
                //                    liSqlPara.Add("I:" + EquParaID.Trim());
                //                    liSqlPara.Add("S:" + CardBuzzerStr.ToString());
                //                    liSqlPara.Add("S:" + UserID.Trim());
                //                    liSqlPara.Add("D:" + Time.ToString());

                //                    oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //                    #endregion
                //                }
                //                else
                //                {
                //                    #region Update
                //                    liSqlPara.Clear();

                //                    sql = @" UPDATE B01_EquParaData SET
                //                             ParaValue = ?,
                //                             UpdateUserID = ?,
                //                             UpdateTime = ?
                //                             WHERE EquID = ? AND EquParaID = ? ";
                //                    liSqlPara.Add("S:" + CardBuzzerStr.ToString());
                //                    liSqlPara.Add("S:" + UserID.Trim());
                //                    liSqlPara.Add("D:" + Time.ToString());
                //                    liSqlPara.Add("I:" + EquID.Trim());
                //                    liSqlPara.Add("I:" + EquParaID.Trim());

                //                    oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //                    #endregion
                //                }
                //                #endregion
                objRet.act = "SaveData";
                objRet.message = CardBuzzerStr;
            }

            return objRet;
        }
        #endregion

        #region Check_Input
        protected static Pub.MessageObject Check_Input(string CardFormat, string Buzzer)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();

            if (string.IsNullOrEmpty(CardFormat))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "卡片格式 必需指定";
            }
            if (string.IsNullOrEmpty(Buzzer))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "蜂鳴器模式 必需指定";
            }

            objRet.act = "CheckData";
            return objRet;
        }
        #endregion

        #endregion
    }
}
