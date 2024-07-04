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

namespace SahoAcs
{
    public partial class MessageList : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            ScriptManager1.EnablePageMethods = true;

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("MessageList", "MessageList.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊主頁Button動作

            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_DeviceConnInfo", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["DciNo"].ToString();
            //Label_Name.Text = TableInfo["DciName"].ToString();
            //Label_Ip.Text = TableInfo["IpAddress"].ToString();
            //Label_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            //popLabel_No.Text = TableInfo["DciNo"].ToString();
            //popLabel_Name.Text = TableInfo["DciName"].ToString();
            //popLabel_PassWD.Text = TableInfo["DciPassWD"].ToString();
            //popLabel_Ip.Text = TableInfo["IpAddress"].ToString();
            //popLabel_Port.Text = TableInfo["TcpPort"].ToString();
            //popLabel_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            #endregion

            #endregion

            if (!IsPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CreateTypeList();
                GetMsgList("");
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.TypeList.ClientID)
                {
                    CreateTypeList();
                    GetMsgList(sFormArg);
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

        #region Msg_GridView_Data_RowDataBound
        public void Msg_GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:
                    #region 設定欄位寛度
                    int[] HeaderWidth = { 180, 200, 300 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理


                    #region 時間
                    #endregion
                    #region 來源
                    #endregion
                    #region 內容
                    #endregion
                    #region 已讀時間
                    #endregion

                    //#region 編號
                    //e.Row.Cells[0].Text = TableInfo["RoleNo"].ToString();
                    //#endregion
                    //#region 名稱
                    //e.Row.Cells[1].Text = TableInfo["RoleName"].ToString();
                    //#endregion
                    //#region 英文名稱
                    //e.Row.Cells[2].Text = TableInfo["RoleEName"].ToString();
                    //#endregion
                    //#region 描述
                    //e.Row.Cells[3].Text = TableInfo["RoleDesc"].ToString();
                    //#endregion
                    //#region 狀態
                    //e.Row.Cells[4].Text = TableInfo["RoleState"].ToString();
                    //#endregion
                    //#region 備註
                    //e.Row.Cells[5].Text = TableInfo["Remark"].ToString();
                    //#endregion

                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion
                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["LogTime"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 183, 204, 304 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 時間
                    #endregion
                    #region 來源
                    #endregion
                    #region 內容
                    #endregion
                    #region 已讀時間
                    if (string.IsNullOrEmpty(oRow.Row["ReadTime"].ToString()))
                    {
                        e.Row.Cells[3].Text = "未讀";
                        e.Row.Attributes.Add("style", "font-weight:bold;background:#b6e0ff");
                    }
                    else
                        e.Row.Cells[3].Text = "已讀";

                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    //for (int x = 0; x < e.Row.Cells.Count; x++)
                    //{
                    //    if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                    //        e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    //}
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 27, true);
                    //e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 43, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["MsgID"].ToString() + "', '', '');LoadMsg();");
                    break;
                #endregion
            }
        }
        #endregion

        #region Msg_GridView_Data_DataBound
        protected void Msg_GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = Msg_GridView.Columns.Count.ToString();
        }
        #endregion

        #region Msg_GridView_PageIndexChanging
        protected void Msg_GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Msg_GridView.PageIndex = e.NewPageIndex;
            if (Msg_GridView.Rows[0].Cells.Count == Msg_GridView.HeaderRow.Cells.Count)
                Msg_GridView.DataBind();
        }
        #endregion

        #region TypeListTimer_Tick
        protected void TypeListTimer_Tick(object sender, EventArgs e)
        {
            CreateTypeList();
        }
        #endregion

        #endregion

        #region Method

        #region CreateTypeList
        private void CreateTypeList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            TableRow tr;
            TableCell td;
            string sql = "", wheresql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            #region Process String
            sql = @" SELECT MessageNoticeCount.MsgType,
                     SUM(CASE
	                     WHEN ReadTime IS NULL THEN 1
	                     ELSE 0
	                     END) AS MsgCount
                     FROM B00_MessageNotice AS MessageNoticeCount ";
            #endregion

            #region Base條件
            if (wheresql != "") wheresql += " AND ";
            wheresql += " ReceiveUserID = ?  ";
            liSqlPara.Add("S:" + hideUserID.Value);
            #endregion

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " GROUP BY (MsgType) ORDER BY MsgType ";

            oAcsDB.GetDataTable("TypeListTabler", sql, liSqlPara, out dt);
            oAcsDB.CloseConnect();

            foreach (DataRow dr in dt.Rows)
            {
                tr = new TableRow();
                td = new TableCell();
                Label TextItem = new Label();

                switch (dr["MsgType"].ToString())
                {
                    case "Type":
                        TextItem.Text = "類型訊息1";
                        break;
                    case "Type2":
                        TextItem.Text = "類型訊息2";
                        break;
                    case "Type3":
                        TextItem.Text = "類型訊息3";
                        break;
                    default:
                        TextItem.Text = dr["MsgType"].ToString();
                        break;
                }
                TextItem.Text += " (" + dr["MsgCount"].ToString() + ")";
                TextItem.Attributes.Add("OnClick", "CreateTypeList('" + dr["MsgType"].ToString() + "');");
                TextItem.Attributes.Add("style", "font-size:24px;padding:20px 35px;cursor:pointer;");
                TextItem.ForeColor = System.Drawing.Color.White;
                td.Controls.Add(TextItem);
                td.Attributes.Add("style", "Width:280px");
                tr.Attributes.Add("style", "height:80px");
                tr.Controls.Add(td);
                this.TypeList.Controls.Add(tr);
            }
            TypeList.Style.Add("word-break", "break-all");
            TypeList.Style.Add("border-collapse", "collapse");
        }
        #endregion

        #region GetMsgList
        public void GetMsgList(string Type)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT * FROM B00_MessageNotice ";

            #region Base條件
            if (wheresql != "") wheresql += " AND ";
            wheresql += " ReceiveUserID = ? ";
            liSqlPara.Add("S:" + hideUserID.Value);

            if (wheresql != "") wheresql += " AND ";
            if (string.IsNullOrEmpty(Type))
                wheresql += " MsgType = (SELECT TOP 1 MsgType FROM B00_MessageNotice ORDER BY MsgType) ";
            else
            {
                wheresql += " MsgType = ? ";
                liSqlPara.Add("S:" + Type);
            }
            #endregion

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY MsgType, LogTime ";
            #endregion

            oAcsDB.GetDataTable("EquTable", sql, liSqlPara, out dt);
            oAcsDB.CloseConnect();
            GirdViewDataBind(this.Msg_GridView, dt);
            Msg_UpdatePanel.Update();
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

        #region LoadMsg
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadMsg(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT MessageNotice.LogTime, MessageNotice.Source, MessageNotice.MsgContent 
                     FROM B00_MessageNotice AS MessageNotice
                     WHERE MsgID = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            sql = "";
            liSqlPara.Clear();

            #region Update ReadTime
            sql = @" UPDATE B00_MessageNotice
                     SET ReadTime = GETDATE()
                     WHERE MsgID = ? AND ReadTime IS NULL ";

            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.SqlCommandExecute(sql, liSqlPara);
            oAcsDB.CloseConnect();
            #endregion

            return EditData;
        }
        #endregion

        #endregion
    }
}