using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;

namespace SahoAcs
{
    public partial class ResetCardAuth2 : System.Web.UI.Page
    {

        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        #endregion

        #region Event
        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            //oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.Input_CardType);
            oScriptManager.RegisterAsyncPostBackControl(this.ResetAuthButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ResetCardAuth", "ResetCardAuth.js?"+DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            Input_CardType.Attributes["onChange"] = "Block();";
            #endregion

            #region 語系切換
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                ViewState["PersonList"] = null;
                CreateDropList_CardType();
                this.lblPersonList.Text = string.Format(GetLocalResourceObject("lblCardList").ToString(), 0);
            }

        }
        #endregion

        #region Input_CardType_SelectedIndexChanged
        protected void Input_CardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryCardList();
        }
        #endregion

        #endregion

        #region Method

        #region CreateDropList_CardType
        private void CreateDropList_CardType()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();            
            Item.Text = "- "+GetGlobalResourceObject("Resource","ddlSelectDefault").ToString()+" -";
            Item.Value = "";
            this.Input_CardType.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT * FROM dbo.B00_ItemList
                     WHERE ItemClass = 'CardType' ";
            #endregion

            oAcsDB.GetDataTable("CardTypeItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["ItemName"].ToString();
                Item.Value = dr["ItemNo"].ToString();
                this.Input_CardType.Items.Add(Item);
            }
        }
        #endregion

        #region QueryCardList
        public void QueryCardList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            TableRow tr;
            TableCell td;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT Person.PsnName,Card.CardNo
                     FROM B01_OrgStruc AS OrgStruc
                     INNER JOIN B01_Person  AS Person ON Person.OrgStrucID = OrgStruc.OrgStrucID
                     INNER JOIN B01_Card AS Card ON Card.PsnID = Person.PsnID
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ?) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            if (!string.IsNullOrEmpty(this.Input_CardType.SelectedValue))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (Card.CardType = ?) ";
                liSqlPara.Add("S:" + this.Input_CardType.SelectedValue);
            }
            else
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " 1 = 0 ";
            }

            if (wheresql != "") wheresql += " AND ";
            wheresql += " CardAuthAllow = '1' ";

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY Card.CardNo ";
            #endregion

            oAcsDB.GetDataTable("CardListTable", sql, liSqlPara, out dt);

            ViewState["PersonList"] = dt;
            string hidden_card = @"<input type='hidden' name='card' value='{0}' />";
            if (dt.Rows.Count > 0)
            {
                tr = new TableRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i % 3 == 0)
                        tr = new TableRow();
                    td = new TableCell();
                    td.Width = 250;
                    td.Text = dt.Rows[i]["PsnName"] + " - [" + dt.Rows[i]["CardNo"] + "]"+string.Format(hidden_card,dt.Rows[i]["CardNo"].ToString());
                    td.Style.Add("white-space", "nowrap");
                    td.Style.Add("Padding", "3px");                    
                    tr.Controls.Add(td);
                    if (i == dt.Rows.Count - 1 && dt.Rows.Count % 3 != 0)
                    {
                        for (int k = 0; k < 3 - (dt.Rows.Count % 3); k++)
                        {
                            td = new TableCell();
                            td.Style.Add("white-space", "nowrap");
                            td.Width = 250;
                            td.Style.Add("Padding", "3px");
                            tr.Controls.Add(td);
                        }
                    }
                    this.PersonList.Controls.Add(tr);
                }
            }
            else
            {
                tr = new TableRow();
                td = new TableCell();
                //td.Text = "該設備無權限可進行複製";
                td.Width = 750;
                tr.Controls.Add(td);
                this.PersonList.Controls.Add(tr);
            }
            this.lblPersonList.Text = string.Format(GetLocalResourceObject("lblCardList").ToString(), dt.Rows.Count);
            PersonListUpdatePanel.Update();
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "msg", "ShowPersonListLabel('" +
            //    dt.Rows.Count + "');$.unblockUI();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "msg", "$.unblockUI();", true);
        }
        #endregion
        #endregion

        protected void ResetAuthButton_Click(object sender, EventArgs e)
        {
            int iCount = 0, iResult = 0, iSuccess = 0, iError = 0,iError2 = 0;
            DataTable PersonListDataTable;
            string ErrorMsg = "";
            string ErrorMsg2 = "";
            string sSql = "";
            List<string> liSqlPara = new List<string>();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            this.MsgResultTextBox.Text = "";
            PersonListDataTable = (DataTable)ViewState["PersonList"];
            foreach (DataRow dr in PersonListDataTable.Rows)
            {
                liSqlPara.Clear();
                sSql = "EXEC dbo.CardAuth_Update ?, ? ,'ResetCardAuth:ExcuteCardAuthReset','','卡片權限重整'";

                liSqlPara.Add("S:" + PersonListDataTable.Rows[iCount]["CardNo"].ToString());
                liSqlPara.Add("S:" + this.hideUserID.Value);
                try
                {
                    iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                    if (iResult > 0)
                    {
                        iSuccess++;
                    }
                    else
                    {
                        iError++;
                        if (!string.IsNullOrEmpty(ErrorMsg)) ErrorMsg += "、";
                        ErrorMsg += PersonListDataTable.Rows[iCount]["CardNo"].ToString();
                    }
                }
                catch(Exception ex)
                {
                    iError2++;
                    if (!string.IsNullOrEmpty(ErrorMsg2)) ErrorMsg2 += "、";
                    ErrorMsg2 += PersonListDataTable.Rows[iCount]["CardNo"].ToString();
                }                
                iCount++;
            }

            MsgResultTextBox.Text += "卡片重整完成！" + Environment.NewLine;
            MsgResultTextBox.Text += "重整成功筆數：" + iSuccess.ToString() + Environment.NewLine;
            MsgResultTextBox.Text += "重整失敗筆數：" + iError2.ToString() + Environment.NewLine;
            MsgResultTextBox.Text += "未設權限筆數：" + iError.ToString() + Environment.NewLine;            
            if (iError != 0)
            {
                MsgResultTextBox.Text += "未設權限卡號：" + Environment.NewLine;
                MsgResultTextBox.Text += ErrorMsg;
            }
            if (iError2 != 0)
            {
                MsgResultTextBox.Text += "重整失敗卡號：" + Environment.NewLine;
                MsgResultTextBox.Text += ErrorMsg2;
            }
            MsgResultUpdatePanel.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI()", true);
        }

    }
}