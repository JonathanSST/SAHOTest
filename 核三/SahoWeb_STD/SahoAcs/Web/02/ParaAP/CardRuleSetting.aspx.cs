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
    public partial class CardRuleSetting : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable ProcessTable = new DataTable();
        DataRow ProcessRow;
        DataColumn UICol1, UICol2;
        DataTable CardRuleTable = new DataTable();

        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.popB_Add);
            oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardRuleSetting", "CardRuleSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            popB_Add.Attributes["onClick"] = "AddRule(); return false;";
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
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    CreateDropDownCardRule();

                    #region CardRuleTable
                    UICol1 = new DataColumn("CardRuleIndex", typeof(string));
                    UICol2 = new DataColumn("CardRule", typeof(string));

                    if (ViewState["CardRuleTable"] == null)
                    {
                        ProcessTable.Columns.Add(UICol1);
                        ProcessTable.Columns.Add(UICol2);
                    }
                    ViewState["CardRuleTable"] = ProcessTable;
                    #endregion

                    popL_FunctionRemind.Text = "提醒：時區規則為0~99，且時區編號不得重覆。";
                    ViewState["CardRuleTable"] = LoadRowToDatatable(hideParaValue.Value, ProcessTable);
                    popInput_CardRuleIndex.Text = ProcessTable.Rows.Count.ToString();
                    CardRule_GridView.DataSource = (DataTable)ViewState["CardRuleTable"];
                    CardRule_GridView.DataBind();
                    CardRule_UpdatePanel.Update();
                }

                if (sFormTarget == this.popB_Add.ClientID)
                {
                    ViewState["CardRuleTable"] = AddRowToDatatable();
                    CardRule_GridView.DataSource = (DataTable)ViewState["CardRuleTable"];
                    CardRule_GridView.DataBind();
                    CardRule_UpdatePanel.Update();
                    popInput_CardRuleIndex.Text = ((DataTable)ViewState["CardRuleTable"]).Rows.Count.ToString();
                    UpdatePanel1.Update();
                }
            }
        }
        #endregion

        #region CardRule_GridView_RowDataBound
        protected void CardRule_GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 40, 200 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    popli_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;
                    DataTable ProcessTable = (DataTable)ViewState["CardRuleTable"];

                    #region 設定欄位寛度
                    int[] DataWidth = { 42, 202 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 時區規則
                    LoadCardRule();
                    for (int k = 0; k < CardRuleTable.Rows.Count; k++)
                    {
                        if (string.Compare(e.Row.Cells[1].Text.ToString(), CardRuleTable.Rows[k]["RuleID"].ToString()) == 0)
                        {
                            e.Row.Cells[1].Text = CardRuleTable.Rows[k]["RuleNo"].ToString() + " ( " + CardRuleTable.Rows[k]["RuleName"].ToString() + " )";

                            break;
                        }
                    }
                    #endregion

                    #region 動作按鈕
                    Button SmallB = (Button)e.Row.FindControl("DeleteCardRule");
                    SmallB.Font.Size = 10;
                    SmallB.Style.Add("margin", "0px 2px 0px 2px");
                    SmallB.Style.Add("padding", "0px 15px");
                    oScriptManager.RegisterAsyncPostBackControl(SmallB);
                    #endregion

                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region CardRule_GridView_DataBound
        protected void CardRule_GridView_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = CardRule_GridView.Columns.Count.ToString();
        }
        #endregion

        #region CardRule_GridView_RowDeleting
        protected void CardRule_GridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ProcessTable = (DataTable)ViewState["CardRuleTable"];
            ProcessTable.Rows.RemoveAt(e.RowIndex);

            ViewState["CardRuleTable"] = ProcessTable;
            CardRule_GridView.DataSource = ProcessTable;
            CardRule_GridView.DataBind();
            CardRule_UpdatePanel.Update();
            popInput_CardRuleIndex.Text = ((DataTable)ViewState["CardRuleTable"]).Rows.Count.ToString();
            UpdatePanel1.Update();
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            string CardRuleStr = "";

            ProcessTable = (DataTable)ViewState["CardRuleTable"];

            for (int i = 0; i < ProcessTable.Rows.Count; i++)
            {
                if (CardRuleStr != "") CardRuleStr += ",";
                    CardRuleStr += int.Parse(ProcessTable.Rows[i]["CardRuleIndex"].ToString()).ToString() + ":" + ProcessTable.Rows[i]["CardRule"].ToString();
            }

            Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + CardRuleStr + "';");
            Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion

        #region Method

        #region CreateDropDownCardRule
        protected void CreateDropDownCardRule()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            sql = @" SELECT CardRuleDef.RuleNo + ' ( ' + CardRuleDef.RuleName + ' )' AS CardRuleName, CardRuleDef.RuleID AS CardRuleValue
                     FROM B01_EquData AS EquData
                     LEFT JOIN B01_CardRuleDef AS CardRuleDef ON CardRuleDef.EquModel = EquData.EquModel
                     WHERE EquData.EquID = ? ";
            liSqlPara.Add("S:" + hideEquID.Value);

            oAcsDB.GetDataTable("CardRuleDef", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["CardRuleName"].ToString();
                Item.Value = dr["CardRuleValue"].ToString();
                this.popInput_CardRule.Items.Add(Item);
            }
        }
        #endregion

        #region LoadCardRule
        public void LoadCardRule()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            sql = @" SELECT * FROM B01_CardRuleDef ";
            oAcsDB.GetDataTable("CardRuleTable", sql, out CardRuleTable);
        }
        #endregion

        #region LoadRowToDatatable
        public DataTable LoadRowToDatatable(string ParaValue, DataTable ProcessTable)
        {
            string[] ItemList, Itemarray; ;
            if (!string.IsNullOrEmpty(ParaValue))
            {
                ItemList = ParaValue.Split(',');
                for (int i = 0; i < ItemList.Length; i++)
                {
                    Itemarray = ItemList[i].Split(':');
                    ProcessRow = ProcessTable.NewRow();
                    ProcessRow["CardRuleIndex"] = Itemarray[0].PadLeft(2, '0');
                    ProcessRow["CardRule"] = Itemarray[1];
                    ProcessTable.Rows.Add(ProcessRow);
                }
            }
            return ProcessTable;
        }
        #endregion

        #region AddRowToDatatable
        protected DataTable AddRowToDatatable()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            DataTable ProcessTable = (DataTable)ViewState["CardRuleTable"];
            if (ProcessTable.Rows.Count < 100)
            {
                for (int i = 0; i < ProcessTable.Rows.Count; i++)
                {
                    if (ProcessTable.Rows[i]["CardRuleIndex"].ToString() == this.popInput_CardRuleIndex.Text.ToString().PadLeft(2, '0'))
                    {
                        objRet.result = false;
                        objRet.message += "規則編號重復！";
                    }
                }
            }
            else
            {
                objRet.result = false;
                objRet.message += "規則已滿！";
            }

            if (objRet.result)
            {
                ProcessRow = ProcessTable.NewRow();
                ProcessRow["CardRuleIndex"] = this.popInput_CardRuleIndex.Text.Trim().PadLeft(2, '0');
                ProcessRow["CardRule"] = this.popInput_CardRule.SelectedValue;
                ProcessTable.Rows.Add(ProcessRow);
            }
            else
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + objRet.message.ToString() + "');");
            ProcessTable = DataTableSort(ProcessTable, "CardRuleIndex ASC");
            ViewState["CardRuleTable"] = ProcessTable;
            return ProcessTable;
        }
        #endregion

        #region DataTableSort
        public DataTable DataTableSort(DataTable BeSortTable, string SortRule)
        {
            DataTable SortTable;

            SortTable = BeSortTable.Clone();
            foreach (DataRow dr in BeSortTable.Select(" 1 = 1 ", SortRule))
            {
                SortTable.ImportRow(dr);
            }
            BeSortTable.Clear();
            return SortTable;
        }
        #endregion

        #endregion
    }
}
