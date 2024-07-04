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
    public partial class FloorSetting : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable UITable = new DataTable();
        DataColumn UICol1, UICol2;
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
            ClientScript.RegisterClientScriptInclude("FloorSetting", "FloorSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
            ClientScript.RegisterClientScriptInclude("JQuery", Pub.JqueyNowVer);

            #region 註冊Button動作
            popB_Save.Attributes.Add("OnClick", " __doPostBack(popB_Save.id, ''); return false;");
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

                #region ElevatorTable
                UICol1 = new DataColumn("IOIndex", typeof(int));
                UICol2 = new DataColumn("FloorName", typeof(string));

                if (ViewState["UITable"] == null)
                {
                    UITable.Columns.Add(UICol1);
                    UITable.Columns.Add(UICol2);
                }
                ViewState["UITable"] = UITable;
                #endregion

                if (string.Compare(Request.QueryString["Mode"].ToString(), "Open", true) == 0)
                    popL_FunctionRemind.Text = "於開放模式下設定需管制的樓層";
                else if (string.Compare(Request.QueryString["Mode"].ToString(), "AccessControl", true) == 0)
                    popL_FunctionRemind.Text = "於管制模式下設定需常開放的樓層";
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == Elevator_UpdatePanel.ClientID && sFormArg == "StarePage")
                {
                    if (!string.IsNullOrEmpty(hideParaValue.Value))
                        ViewState["BinParaValue"] = Sa.Change.HexToBin(hideParaValue.Value, 48, true);

                    ViewState["UITable"] = LoadRowToDatatable((DataTable)ViewState["UITable"], hideFloorName.Value);
                    UITable = (DataTable)ViewState["UITable"];
                    Elevator_GridView.DataSource = (DataTable)ViewState["UITable"];
                    Elevator_GridView.DataBind();
                    Elevator_UpdatePanel.Update();
                }
            }
        }
        #endregion

        #region Elevator_GridView_RowDataBound
        protected void Elevator_GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 100, 400 };
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

                    #region 設定欄位寛度
                    int[] DataWidth = { 100, 400 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 控制
                    CheckBox ChkBox = (CheckBox)e.Row.FindControl("FloorControl");
                    if (ViewState["BinParaValue"] != null)
                    {
                        if (ViewState["BinParaValue"].ToString().Substring(e.Row.RowIndex, 1) == "1")
                        {
                            ChkBox.Checked = true;                       
                        }                      
                        if(Request.QueryString["Mode"].ToString() == "Open")
                        {
                            ChkBox.Checked = !ChkBox.Checked;
                        }
                    }
                    e.Row.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    ChkBox.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    #endregion

                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            string BinStr = "", HexStr = "";

            #region 依畫面設定轉換2進位值
            for (int i = 0; i < Elevator_GridView.Rows.Count; i++)
            {
                CheckBox ChkBox = (CheckBox)Elevator_GridView.Rows[i].FindControl("FloorControl");
                if (ChkBox.Checked)
                    BinStr = "1" + BinStr;
                else
                    BinStr = "0" + BinStr;
            }
            #endregion

            #region 不足位數補處理
            for (int i = BinStr.Length; i < 48; i++)
            {
                BinStr = "0" + BinStr;
            }
            #endregion

            #region 轉換16進位值
            //若為常開模式，則要把資料做互斥或
            if (Request["Mode"].ToString() == "Open")
            {
                BinStr=BinStr.Replace("0", "F").Replace("1","T");
                BinStr = BinStr.Replace("T", "0").Replace("F", "1");
            }
            HexStr = Sa.Change.BinToHex(BinStr, 12);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + "FloorSetting/SahoFromURL" + HexStr + "';");
            Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion

        #region Method

        #region LoadRowToDatatable
        public DataTable LoadRowToDatatable(DataTable Process, string FloorName)
        {
            if (!string.IsNullOrEmpty(FloorName) && string.Compare(FloorName, "undefined") != 0)
            {
                string[] FloorNameArray = FloorName.Split(',');

                for (int i = 0; i < FloorNameArray.Length; i++)
                {
                    string[] Item = FloorNameArray[i].Split(':');
                    Process.Rows.Add(Process.NewRow());
                    Process.Rows[Process.Rows.Count - 1]["IOIndex"] = Item[0].ToString();
                    Process.Rows[Process.Rows.Count - 1]["FloorName"] = Server.HtmlDecode(Item[1].ToString());
                }
            }
            return Process;
        }
        #endregion

        #region GirdViewDataBind
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)//Gridview中有資料
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else//Gridview中沒有資料
            {
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
            }
        }
        #endregion

        #endregion
    }
}