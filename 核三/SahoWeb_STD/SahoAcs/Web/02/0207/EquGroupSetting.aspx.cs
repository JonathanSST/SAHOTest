using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.uc;

namespace SahoAcs
{
    public partial class EquGroupSetting : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable QueueTable = new DataTable();
        DataTable PendingTable = new DataTable();
        DataTable FilterTable = new DataTable();
        DataTable CardRuleTable = new DataTable();
        DataColumn QueueCol1, QueueCol2, QueueCol3, QueueCol4, QueueCol5, QueueCol6, QueueCol7, QueueCol8;
        DataColumn PendingCol1, PendingCol2, PendingCol3, PendingCol4, PendingCol5, PendingCol6, PendingCol7, PendingCol8;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.AddEquButton);
            oScriptManager.RegisterAsyncPostBackControl(this.RemoveEquButton);
            oScriptManager.RegisterAsyncPostBackControl(this.PendingButton);
            oScriptManager.RegisterAsyncPostBackControl(this.QueueButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EquGroupSetting", "EquGroupSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            AddEquButton.Attributes["onClick"] = "AddEqu(); return false;";
            RemoveEquButton.Attributes["onClick"] = "RemoveEqu(); return false;";
            PendingButton.Attributes["onClick"] = "PendingQuery(); return false;";
            QueueButton.Attributes["onClick"] = "QueueQuery(); return false;";
            //popB_Save.Attributes["onClick"] = "return false;";
            popB_Cancel.Attributes["onClick"] = "window.close(); return false;";
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
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                string sql = "";
                Sa.DB.DBReader dr;
                List<string> liSqlPara = new List<string>();

                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                hideEquGrpNo.Value = Request.QueryString["EquGrpNo"].ToString();
                
                #region 取得EquGrpID
                sql = @" SELECT * FROM B01_EquGroup AS EquGroup
                         WHERE EquGrpNo = ? ";
                liSqlPara.Add("S:" + Request.QueryString["EquGrpNo"].ToString());
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    hideEquGrpID.Value = dr.DataReader["EquGrpID"].ToString();
                    this.Title = dr.DataReader["EquGrpName"].ToString();
                }
                    
                #endregion
                
                #endregion

                #region  QueueTable
                QueueCol1 = new DataColumn("EquID", typeof(decimal));
                QueueCol2 = new DataColumn("EquNo", typeof(string));
                QueueCol3 = new DataColumn("EquName", typeof(string));
                QueueCol4 = new DataColumn("EquClass", typeof(string));
                QueueCol5 = new DataColumn("EquModel", typeof(string));
                QueueCol6 = new DataColumn("OptionValue", typeof(string));
                QueueCol7 = new DataColumn("CardRule", typeof(string));
                QueueCol8 = new DataColumn("CardExtData", typeof(string));


                if (ViewState["QueueTable"] == null)
                {
                    QueueTable.Columns.Add(QueueCol1);
                    QueueTable.Columns.Add(QueueCol2);
                    QueueTable.Columns.Add(QueueCol3);
                    QueueTable.Columns.Add(QueueCol4);
                    QueueTable.Columns.Add(QueueCol5);
                    QueueTable.Columns.Add(QueueCol6);
                    QueueTable.Columns.Add(QueueCol7);
                    QueueTable.Columns.Add(QueueCol8);
                }
                ViewState["QueueTable"] = QueueTable;
                #endregion

                #region  FilterTable               
                QueueCol1 = new DataColumn("EquID", typeof(decimal));
                QueueCol2 = new DataColumn("EquNo", typeof(string));
                QueueCol3 = new DataColumn("EquName", typeof(string));
                QueueCol4 = new DataColumn("EquClass", typeof(string));
                QueueCol5 = new DataColumn("EquModel", typeof(string));
                QueueCol6 = new DataColumn("OptionValue", typeof(string));
                QueueCol7 = new DataColumn("CardRule", typeof(string));
                QueueCol8 = new DataColumn("CardExtData", typeof(string));
                if (ViewState["FilterTable"] == null)
                {
                    FilterTable.Columns.Add(QueueCol1);
                    FilterTable.Columns.Add(QueueCol2);
                    FilterTable.Columns.Add(QueueCol3);
                    FilterTable.Columns.Add(QueueCol4);
                    FilterTable.Columns.Add(QueueCol5);
                    FilterTable.Columns.Add(QueueCol6);
                    FilterTable.Columns.Add(QueueCol7);
                    FilterTable.Columns.Add(QueueCol8);
                }
                ViewState["FilterTable"] = FilterTable;
                #endregion

                #region  PendingTable
                PendingCol1 = new DataColumn("EquID", typeof(decimal));
                PendingCol2 = new DataColumn("EquNo", typeof(string));
                PendingCol3 = new DataColumn("EquName", typeof(string));
                PendingCol4 = new DataColumn("EquClass", typeof(string));
                PendingCol5 = new DataColumn("EquModel", typeof(string));
                PendingCol6 = new DataColumn("OptionValue", typeof(string));
                PendingCol7 = new DataColumn("CardRule", typeof(string));
                PendingCol8 = new DataColumn("CardExtData", typeof(string));

                if (ViewState["PendingTable"] == null)
                {
                    PendingTable.Columns.Add(PendingCol1);
                    PendingTable.Columns.Add(PendingCol2);
                    PendingTable.Columns.Add(PendingCol3);
                    PendingTable.Columns.Add(PendingCol4);
                    PendingTable.Columns.Add(PendingCol5);
                    PendingTable.Columns.Add(PendingCol6);
                    PendingTable.Columns.Add(PendingCol7);
                    PendingTable.Columns.Add(PendingCol8);
                }
                ViewState["PendingTable"] = PendingTable;
                #endregion

                #region SetQueryValue
                ViewState["pending_EquNo"] = "";
                ViewState["queue_EquNo"] = "";
                #endregion

                #region Load Queue
                ViewState["QueueTable"] = LoadRowToQueueTable((DataTable)ViewState["QueueTable"]);
                ViewState["QueueTable"] = DataTableSort((DataTable)ViewState["QueueTable"], "EquNo");
                Queue_GridView.DataSource = (DataTable)ViewState["QueueTable"];
                Queue_GridView.DataBind();
                Queue_UpdatePanel.Update();
                #endregion

                #region Load Pending
                ViewState["PendingTable"] = LoadRowToPendingTable((DataTable)ViewState["PendingTable"]);
                Pending_GridView.DataSource = (DataTable)ViewState["PendingTable"];
                Pending_GridView.DataBind();
                Pending_UpdatePanel.Update();
                #endregion

                #region Pending minus Queue
                PendingTable = (DataTable)ViewState["PendingTable"];
                QueueTable = (DataTable)ViewState["QueueTable"];
                FilterTable = (DataTable)ViewState["FilterTable"];
                for (int i = PendingTable.Rows.Count - 1; i >= 0; i--)
                {
                    for (int k = 0; k < QueueTable.Rows.Count; k++)
                    {
                        if (string.Compare(QueueTable.Rows[k]["EquID"].ToString(), PendingTable.Rows[i]["EquID"].ToString()) == 0)
                        {
                            PendingTable.Rows.RemoveAt(i);
                            break;
                        }
                    }
                    for (int k = 0; k < FilterTable.Rows.Count; k++)
                    {
                        if (string.Compare(FilterTable.Rows[k]["EquID"].ToString(), PendingTable.Rows[i]["EquID"].ToString()) == 0)
                        {
                            PendingTable.Rows.RemoveAt(i);
                            break;
                        }
                    }
                }
                ViewState["PendingTable"] = PendingTable;
                Pending_GridView.DataSource = (DataTable)ViewState["PendingTable"];
                Pending_GridView.DataBind();
                Pending_UpdatePanel.Update();
                #endregion
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];
                DataTable ProcessTable;

                #region ←動作
                if (sFormTarget == this.AddEquButton.ClientID)
                {
                    PendingTable = (DataTable)ViewState["PendingTable"];
                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ProcessTable = (DataTable)ViewState["FilterTable"];
                    else
                        ProcessTable = (DataTable)ViewState["QueueTable"];

                    #region 轉換前對QueueTable進行儲存
                    for (int i = 0; i < Queue_GridView.Rows.Count; i++)
                    {
                        DropDownList DDL = (DropDownList)Queue_GridView.Rows[i].Cells[3].FindControl("QueueDropDownList");
                        ProcessTable.Rows[i]["CardRule"] = DDL.SelectedValue;
                    }
                    #endregion

                    for (int i = PendingTable.Rows.Count - 1; i >= 0; i--)
                    {
                        CheckBox CBox = (CheckBox)Pending_GridView.Rows[i].Cells[1].FindControl("PendingCheckBox");

                        if (CBox.Checked)
                        {
                            ProcessTable.Rows.Add(ProcessTable.NewRow());
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["EquID"] = PendingTable.Rows[i]["EquID"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["EquNo"] = PendingTable.Rows[i]["EquNo"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["EquName"] = PendingTable.Rows[i]["EquName"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["EquClass"] = PendingTable.Rows[i]["EquClass"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["EquModel"] = PendingTable.Rows[i]["EquModel"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["OptionValue"] = PendingTable.Rows[i]["OptionValue"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["CardExtData"] = PendingTable.Rows[i]["CardExtData"].ToString();
                            ProcessTable.Rows[ProcessTable.Rows.Count - 1]["CardRule"] = "";
                            PendingTable.Rows.RemoveAt(i);
                        }
                    }
                    ViewState["PendingTable"] = PendingTable;
                    Pending_GridView.DataSource = PendingTable;
                    Pending_GridView.DataBind();
                    Pending_UpdatePanel.Update();

                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ViewState["FilterTable"] = ProcessTable;
                    else
                        ViewState["QueueTable"] = ProcessTable;
                    Queue_GridView.DataSource = ProcessTable;
                    Queue_GridView.DataBind();
                    Queue_UpdatePanel.Update();
                }
                #endregion

                #region →動作
                if (sFormTarget == this.RemoveEquButton.ClientID)
                {
                    PendingTable = (DataTable)ViewState["PendingTable"];
                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ProcessTable = (DataTable)ViewState["FilterTable"];
                    else
                        ProcessTable = (DataTable)ViewState["QueueTable"];

                    #region 轉換前對QueueTable進行儲存
                    for (int i = 0; i < Queue_GridView.Rows.Count; i++)
                    {
                        DropDownList DDL = (DropDownList)Queue_GridView.Rows[i].Cells[3].FindControl("QueueDropDownList");
                        ProcessTable.Rows[i]["CardRule"] = DDL.SelectedValue;
                    }
                    #endregion

                    for (int i = ProcessTable.Rows.Count - 1; i >= 0; i--)
                    {
                        CheckBox CBox = (CheckBox)Queue_GridView.Rows[i].Cells[1].FindControl("QueueCheckBox");

                        if (CBox.Checked)
                        {
                            PendingTable.Rows.Add(PendingTable.NewRow());
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["EquID"] = ProcessTable.Rows[i]["EquID"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["EquNo"] = ProcessTable.Rows[i]["EquNo"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["EquName"] = ProcessTable.Rows[i]["EquName"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["EquClass"] = ProcessTable.Rows[i]["EquClass"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["EquModel"] = ProcessTable.Rows[i]["EquModel"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["OptionValue"] = ProcessTable.Rows[i]["OptionValue"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["CardRule"] = ProcessTable.Rows[i]["CardRule"].ToString();
                            PendingTable.Rows[PendingTable.Rows.Count - 1]["CardExtData"] = ProcessTable.Rows[i]["CardExtData"].ToString();
                            ProcessTable.Rows.RemoveAt(i);
                        }
                    }
                    ViewState["PendingTable"] = PendingTable;
                    Pending_GridView.DataSource = PendingTable;
                    Pending_GridView.DataBind();
                    Pending_UpdatePanel.Update();

                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ViewState["FilterTable"] = ProcessTable;
                    else
                        ViewState["QueueTable"] = ProcessTable;
                    Queue_GridView.DataSource = ProcessTable;
                    Queue_GridView.DataBind();
                    Queue_UpdatePanel.Update();
                }
                #endregion

                #region SettingCardExtData
                if (sFormTarget.IndexOf("Queue_GridView_QueueFloorButton", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ProcessTable = (DataTable)ViewState["FilterTable"];
                    else
                        ProcessTable = (DataTable)ViewState["QueueTable"];

                    #region 轉換前對QueueTable進行儲存
                    for (int i = 0; i < Queue_GridView.Rows.Count; i++)
                    {
                        DropDownList DDL = (DropDownList)Queue_GridView.Rows[i].Cells[3].FindControl("QueueDropDownList");
                        ProcessTable.Rows[i]["CardRule"] = DDL.SelectedValue;
                    }
                    #endregion

                    string[] Target = sFormArg.Split('&');
                    for (int i = 0; i < ProcessTable.Rows.Count; i++)
                    {
                        if (string.Compare(ProcessTable.Rows[i]["EquID"].ToString(), Target[0].ToString()) == 0)
                            ProcessTable.Rows[i]["CardExtData"] = Target[1];
                    }

                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ViewState["FilterTable"] = ProcessTable;
                    else
                        ViewState["QueueTable"] = ProcessTable;

                    Queue_GridView.DataSource = ProcessTable;
                    Queue_GridView.DataBind();
                    Queue_UpdatePanel.Update();
                }
                #endregion

                #region PendingQuery
                if (sFormTarget == this.PendingButton.ClientID)
                {
                    #region Set QueryValue
                    ViewState["pending_EquNo"] = this.PendingInput_EquNo.Text.Trim();
                    #endregion

                    ViewState["PendingTable"] = LoadRowToPendingTable((DataTable)ViewState["PendingTable"]);
                    PendingTable = (DataTable)ViewState["PendingTable"];
                    QueueTable = (DataTable)ViewState["QueueTable"];
                    FilterTable = (DataTable)ViewState["FilterTable"];
                    for (int i = PendingTable.Rows.Count - 1; i >= 0; i--)
                    {
                        for (int k = 0; k < QueueTable.Rows.Count; k++)
                        {
                            if (string.Compare(QueueTable.Rows[k]["EquID"].ToString(), PendingTable.Rows[i]["EquID"].ToString()) == 0)
                            {
                                PendingTable.Rows.RemoveAt(i);
                                break;
                            }
                        }
                        for (int k = 0; k < FilterTable.Rows.Count; k++)
                        {
                            if (string.Compare(FilterTable.Rows[k]["EquID"].ToString(), PendingTable.Rows[i]["EquID"].ToString()) == 0)
                            {
                                PendingTable.Rows.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    ViewState["PendingTable"] = PendingTable;
                    Pending_GridView.DataSource = (DataTable)ViewState["PendingTable"];
                    Pending_GridView.DataBind();
                    Pending_UpdatePanel.Update();
                }
                #endregion

                #region QueueQuery
                if (sFormTarget == this.QueueButton.ClientID)
                {
                    #region Set QueryValue
                    ViewState["queue_EquNo"] = this.PendingInput_EquNo.Text.Trim();
                    #endregion

                    #region 查詢前對ProcessTable進行儲存
                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ProcessTable = (DataTable)ViewState["FilterTable"];
                    else
                        ProcessTable = (DataTable)ViewState["QueueTable"];

                    for (int i = 0; i < Queue_GridView.Rows.Count; i++)
                    {
                        DropDownList DDL = (DropDownList)Queue_GridView.Rows[i].Cells[3].FindControl("QueueDropDownList");
                        ProcessTable.Rows[i]["CardRule"] = DDL.SelectedValue;
                    }

                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ViewState["FilterTable"] = ProcessTable;
                    else
                        ViewState["QueueTable"] = ProcessTable;
                    #endregion

                    ProcessTable = FilterRowToFilterTable();
                    Queue_GridView.DataSource = ProcessTable;
                    Queue_GridView.DataBind();
                    Queue_UpdatePanel.Update();
                }
                #endregion
            }
        }
        #endregion

        #region Queue_GridView_RowDataBound
        protected void Queue_GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 40, 100, 160, 150 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    Queueli_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;
                    DataTable ProcessTable;
                    if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                        ProcessTable = (DataTable)ViewState["FilterTable"];
                    else
                        ProcessTable = (DataTable)ViewState["QueueTable"];

                    #region 設定欄位寛度
                    int[] DataWidth = { 38, 100, 160, 150 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 選取
                    CheckBox ChkBox = (CheckBox)e.Row.Cells[0].FindControl("QueueCheckBox");
                    e.Row.Cells[0].Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    e.Row.Cells[1].Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    e.Row.Cells[2].Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    ChkBox.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    #endregion

                    #region 規則
                    DropDownList DDL = (DropDownList)e.Row.FindControl("QueueDropDownList");
                    if (!string.IsNullOrEmpty(ProcessTable.Rows[e.Row.RowIndex]["OptionValue"].ToString()))
                    {
                        string[] ItemList = ProcessTable.Rows[e.Row.RowIndex]["OptionValue"].ToString().Split(',');
                        string[] Itemarray;
                        LoadCardRule();
                        for (int i = 0; i < ItemList.Length; i++)
                        {
                            Itemarray = ItemList[i].Split(':');
                            ListItem Li = new ListItem();
                            for (int k = 0; k < CardRuleTable.Rows.Count; k++)
                            {
                                if (string.Compare(Itemarray[1].ToString(), CardRuleTable.Rows[k]["RuleID"].ToString()) == 0)
                                {
                                    Li.Text = Itemarray[0] + "-" + CardRuleTable.Rows[k]["RuleNo"].ToString() + "(" + CardRuleTable.Rows[k]["RuleName"].ToString() + ")";
                                    break;
                                }
                            }
                            Li.Value = Itemarray[0];
                            DDL.Items.Add(Li);
                        }

                        if (!string.IsNullOrEmpty(ProcessTable.Rows[e.Row.RowIndex]["CardRule"].ToString()))
                            DDL.SelectedValue = ProcessTable.Rows[e.Row.RowIndex]["CardRule"].ToString();
                    }
                    else
                    {
                        ListItem Li = new ListItem();
                        Li.Text = "未定義規則";
                        Li.Value = "";
                        DDL.Items.Add(Li);
                    }
                    #endregion

                    #region 樓層
                    Button SmallB = (Button)e.Row.FindControl("QueueFloorButton");
                    SmallB.Font.Size = 10;
                    SmallB.Style.Add("margin", "0px 2px 0px 2px");
                    SmallB.Style.Add("padding", "0px 15px");
                    if (string.Compare(ProcessTable.Rows[e.Row.RowIndex]["EquClass"].ToString(), "Elevator") == 0)
                    {
                        SmallB.Enabled = true;
                        SmallB.Attributes["onClick"] = "GetFloorNameList('EquGroupFloorList.aspx','" + ProcessTable.Rows[e.Row.RowIndex]["EquID"].ToString() + "&" + ProcessTable.Rows[e.Row.RowIndex]["CardExtData"].ToString() + "','dialogHeight:390px;dialogWidth:625px','" + SmallB.ClientID + "'); return false;";
                        oScriptManager.RegisterAsyncPostBackControl(SmallB);
                    }
                    #endregion

                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region Pending_GridView_RowDataBound
        protected void Pending_GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 40, 160 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    Pendingli_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;
                    DataTable ProcessTable = (DataTable)ViewState["PendingTable"];

                    #region 設定欄位寛度
                    int[] DataWidth = { 38, 160 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 選取
                    CheckBox ChkBox = (CheckBox)e.Row.Cells[0].FindControl("PendingCheckBox");
                    e.Row.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    ChkBox.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    #endregion

                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region QueueInput_EquClass_Init
        protected void QueueInput_EquClass_Init(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text =string.Format("- {0} -",GetGlobalResourceObject("Resource","ddlSelectDefault"));
            Item.Value = "";
            this.QueueInput_EquClass.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT ItemInfo1 FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' ";
            #endregion

            oAcsDB.GetDataTable("EquModelItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                switch (dr["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeDA").ToString();
                        break;
                    case "Elevator":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeElev").ToString();
                        break;
                    case "TRT":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeTRT").ToString();
                        break;
                    case "Meal":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeMeal").ToString();
                        break;
                }
                Item.Value = dr["ItemInfo1"].ToString();
                this.QueueInput_EquClass.Items.Add(Item);
            }
        }
        #endregion

        #region PendingInput_EquClass_Init
        protected void PendingInput_EquClass_Init(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = string.Format("- {0} -", GetGlobalResourceObject("Resource", "ddlSelectDefault"));
            Item.Value = "";
            this.PendingInput_EquClass.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT ItemInfo1 FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' ";
            #endregion

            oAcsDB.GetDataTable("EquModelItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                switch (dr["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeDA").ToString();
                        break;
                    case "Elevator":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeElev").ToString();
                        break;
                    case "TRT":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeTRT").ToString();
                        break;
                    case "Meal":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeMeal").ToString();
                        break;
                }
                Item.Value = dr["ItemInfo1"].ToString();
                this.PendingInput_EquClass.Items.Add(Item);
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            string actflag = "";
            DataTable ProcessTable;
            if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
                ProcessTable = (DataTable)ViewState["FilterTable"];
            else
                ProcessTable = (DataTable)ViewState["QueueTable"];

            #region 轉換前對QueueTable進行儲存
            for (int i = 0; i < Queue_GridView.Rows.Count; i++)
            {
                DropDownList DDL = (DropDownList)Queue_GridView.Rows[i].Cells[3].FindControl("QueueDropDownList");
                ProcessTable.Rows[i]["CardRule"] = DDL.SelectedValue;
            }
            #endregion

            #region 還原 FilterTable、QueueTable
            if (((DataTable)ViewState["FilterTable"]).Rows.Count > 0)
            {
                FilterTable = ProcessTable;
                QueueTable = (DataTable)ViewState["QueueTable"];
            }
            else
            {
                FilterTable = (DataTable)ViewState["FilterTable"];
                QueueTable = ProcessTable;
            }
            #endregion

            #region 合併Table
            for (int i = 0; i < FilterTable.Rows.Count; i++)
            {
                QueueTable.Rows.Add(QueueTable.NewRow());
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquID"] = FilterTable.Rows[i]["EquID"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquNo"] = FilterTable.Rows[i]["EquNo"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquName"] = FilterTable.Rows[i]["EquName"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquClass"] = FilterTable.Rows[i]["EquClass"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquModel"] = FilterTable.Rows[i]["EquModel"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["OptionValue"] = FilterTable.Rows[i]["OptionValue"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["CardRule"] = FilterTable.Rows[i]["CardRule"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["CardExtData"] = FilterTable.Rows[i]["CardExtData"].ToString();
            }
            #endregion

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable OrgEquGroupData;
            List<string> liSqlPara = new List<string>();
            DateTime Time = DateTime.Now;

            sql = @" SELECT * FROM B01_EquGroupData
                     WHERE EquGrpID = ?  ";
            liSqlPara.Add("I:" + hideEquGrpID.Value.ToString());
            oAcsDB.GetDataTable("OrgEquGroupData", sql, liSqlPara, out OrgEquGroupData);

            liSqlPara.Clear();
            liSqlPara.Add("D:"+Time.ToString());            
            liSqlPara.Add("S:"+this.hideUserID.Value);
            liSqlPara.Add("S:"+Session["UserName"].ToString());            
            liSqlPara.Add("S:"+Request.UserHostAddress);            
            //開始進行設備群組資料記錄
            oAcsDB.SqlCommandExecute("INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,LogFrom,LogIP,LogInfo) VALUES (?,'設備群組設定',?,?,'0207',?,'開始進行設備群組設定')",liSqlPara);

            sql = "";
            liSqlPara.Clear();           
            for (int i = 0; i < OrgEquGroupData.Rows.Count; i++)
            {
                actflag = "";
                for (int k = QueueTable.Rows.Count - 1; k >= 0; k--)
                {
                    if (string.Compare(QueueTable.Rows[k]["EquID"].ToString(), OrgEquGroupData.Rows[i]["EquID"].ToString()) == 0)
                    {
                        actflag = "Update";
                        #region Process String - Update EquGroupData
                        sql += @" UPDATE B01_EquGroupData SET
                                  CardRule = ?, CardExtData = ?,
                                  UpdateUserID = ?, UpdateTime = ?
                                  WHERE EquGrpID = ? AND EquID = ? ";

                        liSqlPara.Add("S:" + QueueTable.Rows[k]["CardRule"].ToString());
                        liSqlPara.Add("S:" + QueueTable.Rows[k]["CardExtData"].ToString());
                        liSqlPara.Add("S:" + hideUserID.Value.ToString());
                        liSqlPara.Add("D:" + Time.ToString());
                        liSqlPara.Add("I:" + hideEquGrpID.Value.ToString());
                        liSqlPara.Add("I:" + QueueTable.Rows[k]["EquID"].ToString());
                        #endregion
                        QueueTable.Rows.RemoveAt(k);
                    }
                }
                if (string.IsNullOrEmpty(actflag))
                    actflag = "Delete";
                #region Process String - Delete EquGroupData
                if (actflag == "Delete")
                {
                    sql += " DELETE FROM B01_EquGroupData WHERE EquGrpID = ? AND EquID = ? ";
                    liSqlPara.Add("I:" + hideEquGrpID.Value.ToString());
                    liSqlPara.Add("I:" + OrgEquGroupData.Rows[i]["EquID"].ToString());

                    sql += @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,LogFrom,LogIP,LogInfo) VALUES (?,'設備群組設定',?,?,'0207',?,?) ";
                    liSqlPara.Add("D:" + DateTime.Now.ToString());
                    liSqlPara.Add("S:" + this.hideUserID.Value);
                    liSqlPara.Add("S:" + Session["UserName"].ToString());
                    liSqlPara.Add("S:" + Request.UserHostAddress);
                    liSqlPara.Add("S:" + string.Format("{0}刪除設備{1}", hideEquGrpNo.Value, OrgEquGroupData.Rows[i]["EquID"].ToString()));
                }
                #endregion
            }
            for (int i = 0; i < QueueTable.Rows.Count; i++)
            {
                sql += @" INSERT INTO B01_EquGroupData (EquGrpID, EquID, CardRule, CardExtData, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                          VALUES (?, ?, ?, ?, ?, ?, ?, ?)  ";
                liSqlPara.Add("I:" + hideEquGrpID.Value.ToString());
                liSqlPara.Add("I:" + QueueTable.Rows[i]["EquID"].ToString());
                liSqlPara.Add("S:" + QueueTable.Rows[i]["CardRule"].ToString());
                
                liSqlPara.Add("S:" + QueueTable.Rows[i]["CardExtData"].ToString());

                liSqlPara.Add("S:" + hideUserID.Value.ToString());
                liSqlPara.Add("D:" + Time.ToString());
                liSqlPara.Add("S:" + hideUserID.Value.ToString());
                liSqlPara.Add("D:" + Time.ToString());

                sql += @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,LogFrom,LogIP,LogInfo) VALUES (?,'設備群組設定',?,?,'0207',?,?) ";
                liSqlPara.Add("D:" + DateTime.Now.ToString());
                liSqlPara.Add("S:" + this.hideUserID.Value);
                liSqlPara.Add("S:" + Session["UserName"].ToString());
                liSqlPara.Add("S:" + Request.UserHostAddress);
                liSqlPara.Add("S:" + string.Format("{0}加入設備設備{1}",hideEquGrpNo.Value,QueueTable.Rows[i]["EquID"].ToString()));
            }
            if (oAcsDB.SqlCommandExecute(sql, liSqlPara) != -1)
                Response.Write("<script>window.close();</script>");
        }
        #endregion

        #endregion

        #region Method

        #region LoadCardRule
        public void LoadCardRule()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            sql = @" SELECT * FROM B01_CardRuleDef ";
            oAcsDB.GetDataTable("CardRuleTable", sql, out CardRuleTable);
        }
        #endregion

        #region LoadRowToPendingTable
        public DataTable LoadRowToPendingTable(DataTable Process)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            List<string> liSqlPara = new List<string>();

            //            sql = @" SELECT * FROM (
            //	                                SELECT DISTINCT 
            //	                                EquData.*, EquParaData.ParaValue AS OptionValue, '' AS CardRule, '' AS CardExtData 
            //	                                FROM B00_SysUser AS SysUser 
            //	                                INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.UserID = SysUser.UserID
            //	                                INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.MgaID = SysUserMgns.MgaID
            //	                                INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquGrpID = MgnEquGroup.EquGrpID
            //	                                LEFT JOIN B01_EquData AS EquData ON EquData.EquID = EquGroupData.EquID
            //	                                LEFT JOIN B01_EquParaDef AS EquParaDef ON EquParaDef.EquModel = EquData.EquModel AND EquParaDef.ParaName = 'CardRule'
            //	                                LEFT JOIN B01_EquParaData AS EquParaData ON EquParaData.EquID = EquData.EquID AND EquParaData.EquParaID = EquParaDef.EquParaID
            //	                                WHERE (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) 
            //
            //	                                UNION 
            //
            //	                                SELECT DISTINCT 
            //	                                EquData.*, EquParaData.ParaValue AS OptionValue, '' AS CardRule, '' AS CardExtData 
            //	                                FROM B01_EquData AS EquData
            //	                                LEFT JOIN B01_EquParaDef AS EquParaDef ON EquParaDef.EquModel = EquData.EquModel AND EquParaDef.ParaName = 'CardRule'
            //	                                LEFT JOIN B01_EquParaData AS EquParaData ON EquParaData.EquID = EquData.EquID AND EquParaData.EquParaID = EquParaDef.EquParaID
            //	                                WHERE (EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) 
            //                     ) AS Base ";
            sql = @" SELECT * FROM ( 
                                    SELECT DISTINCT 
                                    EquData.*, EquParaData.ParaValue AS OptionValue, '' AS CardRule, CASE WHEN (EquClass='Elevator') THEN
									'000000000000' 
									ELSE ''
									END AS CardExtData 
                                    FROM B01_EquData AS EquData
                                    LEFT JOIN B01_EquParaDef AS EquParaDef ON EquParaDef.EquModel = EquData.EquModel AND EquParaDef.ParaName = 'CardRule'
                                    LEFT JOIN B01_EquParaData AS EquParaData ON EquParaData.EquID = EquData.EquID AND EquParaData.EquParaID = EquParaDef.EquParaID
                                    LEFT JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                                    LEFT JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                                    LEFT JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                                    LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
                                    WHERE (SysUser.UserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?) OR EquData.CreateUserID = ?) 
                                    ) AS Base ";

            #region DataAuth
            //liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + "%" + this.hideUserID.Value + "%");
            liSqlPara.Add("S:" + this.hideUserID.Value);
            //liSqlPara.Add("S:" + "%" + this.hideUserID.Value + "%");
            #endregion

            if (!string.IsNullOrEmpty(this.PendingInput_EquClass.SelectedValue.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " Base.EquClass = ? ";
                liSqlPara.Add("S:" + PendingInput_EquClass.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ViewState["pending_EquNo"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " Base.EquNo like ? ";
                liSqlPara.Add("S:" + "%" + ViewState["pending_EquNo"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquID ";

            oAcsDB.GetDataTable("EquDataTable", sql, liSqlPara, out Process);
            return Process;
        }
        #endregion

        #region LoadRowToQueueTable
        public DataTable LoadRowToQueueTable(DataTable Process)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();

            sql = @" SELECT EquData.*, EquParaData.ParaValue AS OptionValue, EquGroupData.CardRule, 	
                    CASE WHEN (EquClass='Elevator' AND EquGroupData.CardExtData='') THEN '000000000000' ELSE
	                EquGroupData.CardExtData 
	                END AS CardExtData
                     FROM B01_EquGroupData AS EquGroupData
                     INNER JOIN B01_EquData AS EquData ON EquData.EquID = EquGroupData.EquID
                     LEFT JOIN B01_EquParaDef AS EquParaDef ON EquParaDef.EquModel = EquData.EquModel AND EquParaDef.ParaName = 'CardRule'
                     LEFT JOIN B01_EquParaData AS EquParaData ON EquParaData.EquID = EquData.EquID AND EquParaData.EquParaID = EquParaDef.EquParaID
                     WHERE EquGroupData.EquGrpID = ? ";
            liSqlPara.Add("S:" + hideEquGrpID.Value.ToString());
            oAcsDB.GetDataTable("EquDataTable", sql, liSqlPara, out Process);
            return Process;
        }
        #endregion

        #region FilterRowToFilterTable
        public DataTable FilterRowToFilterTable()
        {
            bool copyflag = false;
            bool actflag = false;
            QueueTable = (DataTable)ViewState["QueueTable"];
            FilterTable = (DataTable)ViewState["FilterTable"];

            #region 合併Table
            for (int i = FilterTable.Rows.Count - 1; i >= 0; i--)
            {
                QueueTable.Rows.Add(QueueTable.NewRow());
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquID"] = FilterTable.Rows[i]["EquID"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquNo"] = FilterTable.Rows[i]["EquNo"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquName"] = FilterTable.Rows[i]["EquName"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquClass"] = FilterTable.Rows[i]["EquClass"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["EquModel"] = FilterTable.Rows[i]["EquModel"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["OptionValue"] = FilterTable.Rows[i]["OptionValue"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["CardRule"] = FilterTable.Rows[i]["CardRule"].ToString();
                QueueTable.Rows[QueueTable.Rows.Count - 1]["CardExtData"] = FilterTable.Rows[i]["CardExtData"].ToString();
            }
            #endregion

            FilterTable.Rows.Clear();

            #region 查詢過濾
            if (!string.IsNullOrEmpty(this.QueueInput_EquClass.SelectedValue.Trim()) || !string.IsNullOrEmpty(this.QueueInput_EquNo.Text.Trim()))
            {
                for (int i = QueueTable.Rows.Count - 1; i >= 0; i--)
                {
                    copyflag = false;
                    if (!string.IsNullOrEmpty(this.QueueInput_EquClass.SelectedValue.Trim()) && !string.IsNullOrEmpty(this.QueueInput_EquNo.Text.Trim()))
                    {
                        if (string.Compare(QueueTable.Rows[i]["EquClass"].ToString(), this.QueueInput_EquClass.SelectedValue.ToString()) == 0
                         && QueueTable.Rows[i]["EquNo"].ToString().IndexOf(QueueInput_EquNo.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                            copyflag = true;
                    }
                    else if (!string.IsNullOrEmpty(this.QueueInput_EquClass.SelectedValue.Trim()))
                    {
                        if (string.Compare(QueueTable.Rows[i]["EquClass"].ToString(), this.QueueInput_EquClass.SelectedValue.ToString()) == 0)
                            copyflag = true;
                    }
                    else if (!string.IsNullOrEmpty(this.QueueInput_EquNo.Text.Trim()))
                    {
                        if (QueueTable.Rows[i]["EquNo"].ToString().IndexOf(QueueInput_EquNo.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                            copyflag = true;
                    }

                    if (copyflag)
                    {
                        FilterTable.Rows.Add(FilterTable.NewRow());
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["EquID"] = QueueTable.Rows[i]["EquID"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["EquNo"] = QueueTable.Rows[i]["EquNo"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["EquName"] = QueueTable.Rows[i]["EquName"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["EquClass"] = QueueTable.Rows[i]["EquClass"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["EquModel"] = QueueTable.Rows[i]["EquModel"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["OptionValue"] = QueueTable.Rows[i]["OptionValue"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["CardRule"] = QueueTable.Rows[i]["CardRule"].ToString();
                        FilterTable.Rows[FilterTable.Rows.Count - 1]["CardExtData"] = QueueTable.Rows[i]["CardExtData"].ToString();
                        QueueTable.Rows.RemoveAt(i);
                    }
                }
                actflag = true;
            }
            #endregion

            FilterTable = DataTableSort(FilterTable, "EquNo");
            QueueTable = DataTableSort(QueueTable, "EquNo");

            ViewState["FilterTable"] = FilterTable;
            ViewState["QueueTable"] = QueueTable;
            if (actflag)
                return FilterTable;
            else
                return QueueTable;
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