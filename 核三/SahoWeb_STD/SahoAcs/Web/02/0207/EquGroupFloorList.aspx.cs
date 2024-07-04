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
    public partial class EquGroupFloorList : System.Web.UI.Page
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

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EquGroupFloorList", "EquGroupFloorList.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            //popB_SetElevatorButton.Attributes["onClick"] = "SetElevatorRow(); return false;";
            popB_Save.Attributes["onClick"] = "ProcessFloorSetting();  return false;";
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
                hideEquID.Value = Request.QueryString["EquID"].ToString();
                if (!string.IsNullOrEmpty( Request.QueryString["CardExtData"].ToString()))
                    hideCardExtData.Value = Request.QueryString["CardExtData"].ToString();
                #endregion

                #region Give Hex Value
                if (!string.IsNullOrEmpty(hideCardExtData.Value))
                    ViewState["BinParaValue"] = Sa.Change.HexToBin(hideCardExtData.Value, 48, true);
                #endregion

                Query();
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
                            ChkBox.Checked = true;
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

        #endregion

        #region Method

        #region Query
        public void Query()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT ElevatorFloor.*
                     FROM B01_ElevatorFloor AS ElevatorFloor
                     WHERE ElevatorFloor.EquID = ? ";
            liSqlPara.Add("S:" + hideEquID.Value);
            #endregion

            oAcsDB.GetDataTable("FloorNameTable", sql, liSqlPara, out dt);

            GirdViewDataBind(this.Elevator_GridView, dt);
            Elevator_UpdatePanel.Update();
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

        #region ProcessFloorSetting
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ProcessFloorSetting(string[] TableItem)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string BinStr = "", HexStr = "";

            #region 依畫面設定轉換2進位值
            for (int i = 0; i < TableItem.Length; i++)
            {
                if (bool.Parse(TableItem[i].ToString()))
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
            HexStr = Sa.Change.BinToHex(BinStr, 12);
            #endregion



            //            #region 判斷動作
            //            sql = @" SELECT COUNT(*) FROM B01_EquParaData ";

            //            if (!string.IsNullOrEmpty(EquID.ToString()))
            //            {
            //                if (wheresql != "") wheresql += " AND ";
            //                wheresql += " EquID = ? ";
            //                liSqlPara.Add("S:" + EquID.ToString());
            //            }

            //            if (!string.IsNullOrEmpty(EquParaID.ToString()))
            //            {
            //                if (wheresql != "") wheresql += " AND ";
            //                wheresql += " EquParaID = ? ";
            //                liSqlPara.Add("S:" + EquParaID.ToString());
            //            }

            //            if (wheresql != "")
            //                sql += " WHERE ";
            //            sql += wheresql;

            //            Decide = oAcsDB.GetIntScalar(sql, liSqlPara);
            //            #endregion

            //            #region Process ParaData
            //            if (Decide <= 0)
            //            {
            //                #region Insert
            //                liSqlPara.Clear();

            //                sql = @" INSERT INTO B01_EquParaData(EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime)
            //                         VALUES (?, ?, ?, ?, ?) ";
            //                liSqlPara.Add("I:" + EquID.ToString());
            //                liSqlPara.Add("I:" + EquParaID.ToString());
            //                liSqlPara.Add("S:" + HexStr.ToString());
            //                liSqlPara.Add("S:" + UserID.ToString());
            //                liSqlPara.Add("D:" + Time.ToString());

            //                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            //                #endregion
            //            }
            //            else
            //            {
            //                #region Update
            //                liSqlPara.Clear();

            //                sql = @" UPDATE B01_EquParaData SET
            //                         ParaValue = ?,
            //                         UpdateUserID = ?,
            //                         UpdateTime = ?
            //                         WHERE EquID = ? AND EquParaID = ? ";
            //                liSqlPara.Add("S:" + HexStr.ToString());
            //                liSqlPara.Add("S:" + UserID.ToString());
            //                liSqlPara.Add("D:" + Time.ToString());
            //                liSqlPara.Add("I:" + EquID.ToString());
            //                liSqlPara.Add("I:" + EquParaID.ToString());

            //                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            //                #endregion
            //            }
            //            #endregion
            objRet.act = "ProcessFloorSetting";
            objRet.message = HexStr.ToString();
            return objRet;
        }
        #endregion

        #endregion
    }
}