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
    public partial class FloorNameList : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable UITable = new DataTable();
        DataRow UIRow;
        DataColumn UICol1, UICol2;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.popB_SetElevatorButton);
            oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("FloorNameList", "FloorNameList.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            popB_SetElevatorButton.Attributes["onClick"] = "SetElevatorRow(); return false;";
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

                popL_FunctionRemind.Text = "提醒：樓層名稱設定並不會直接影響權限開放與否，請於名稱設定完成後，進一步調整權限。";
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    ViewState["UITable"] = LoadRowToDatatable((DataTable)ViewState["UITable"], hideFloorName.Value);
                    UITable = (DataTable)ViewState["UITable"];
                    popInput_FloorCount.Text = UITable.Rows.Count.ToString();
                    Elevator_GridView.DataSource = (DataTable)ViewState["UITable"];
                    Elevator_GridView.DataBind();
                    Elevator_UpdatePanel.Update();
                }

                if (sFormTarget == this.popB_SetElevatorButton.ClientID)
                {
                    ViewState["UITable"] = SetRowToDatatable();
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
                    e.Row.Cells[0].Width = 100;
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
                    DataTable ProcessTable = (DataTable)ViewState["UITable"];

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 100;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 樓層名稱
                    TextBox tb = (TextBox)e.Row.FindControl("FloorName");
                    tb.Text = ProcessTable.Rows[e.Row.RowIndex]["FloorName"].ToString();
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
            Pub.MessageObject objRet = new Pub.MessageObject();
            GridViewToDatatable();
            string FloorNamestr = "";
            UITable = (DataTable)ViewState["UITable"];
            objRet = Check_Input_DB(hideEquID.Value, hideEquParaID.Value, UITable);
            if (objRet.result)
            {
                for (int i = 0; i < UITable.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(FloorNamestr)) FloorNamestr += ",";
                    FloorNamestr += UITable.Rows[i][0] + ":" + UITable.Rows[i][1];
                }

                Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + "FloorNameList/SahoFromURL" + FloorNamestr + "';");
                Sa.Web.Fun.RunJavaScript(this, "window.close();");
            }
            else
                Sa.Web.Fun.RunJavaScript(this, objRet.message);
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

        #region SetRowToDatatable
        protected DataTable SetRowToDatatable()
        {
            int tempint, MaxRow;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DataTable ProcessTable = (DataTable)ViewState["UITable"];
            GridViewToDatatable();

            tempint = int.Parse(this.popInput_FloorCount.Text.ToString());
            if (tempint < 0 || tempint > 48)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "樓層數量超出限制,最高為48層樓";
            }

            if (objRet.result)
            {
                MaxRow = Math.Max(tempint, ProcessTable.Rows.Count);

                for (int i = 0; i < MaxRow; i++)
                {
                    if (ProcessTable.Rows.Count < tempint)
                    {
                        UIRow = ProcessTable.NewRow();
                        UIRow["IOIndex"] = ProcessTable.Rows.Count.ToString();
                        ProcessTable.Rows.Add(UIRow);
                    }
                    else if (ProcessTable.Rows.Count > tempint)
                    {
                        ProcessTable.Rows.RemoveAt(ProcessTable.Rows.Count - 1);
                    }

                }
            }
            else
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + objRet.message.ToString() + "');");
            return ProcessTable;
        }
        #endregion

        #region GridViewToDatatable
        public void GridViewToDatatable()
        {
            if (ViewState["UITable"] != null)
            {
                DataTable ProcessTable = (DataTable)ViewState["UITable"];
                for (int i = 0; i < Elevator_GridView.Rows.Count; i++)
                {
                    TextBox tb = (TextBox)Elevator_GridView.Rows[i].FindControl("FloorName");
                    ProcessTable.Rows[i]["FloorName"] = tb.Text;
                }
                ViewState["UITable"] = ProcessTable;
            }
        }
        #endregion

        #region Check_Input_DB
        protected Pub.MessageObject Check_Input_DB(string EquID, string EquParaID, DataTable UITable)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(EquID))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備 必須指定";
            }

            if (string.IsNullOrEmpty(EquParaID))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "參數項目 必須指定";
            }

            for (int i = 0; i < UITable.Rows.Count; i++)
            {
                if (!int.TryParse(UITable.Rows[i][0].ToString(), out tempint))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "接點編號 必須為整數";
                    break;
                }
            }




            #endregion
            #region DB

            sql = @" SELECT EquClass FROM B01_EquData WHERE EquID = ? ";
            liSqlPara.Add("S:" + EquID.ToString());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (string.Compare(dr.DataReader["EquClass"].ToString(), "Elevator") != 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "設備需為電梯";
                }
            }
            #endregion

            return objRet;
        }
        #endregion

        #endregion
    }
}
