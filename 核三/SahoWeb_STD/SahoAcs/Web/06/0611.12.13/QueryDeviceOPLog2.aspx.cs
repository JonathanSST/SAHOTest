using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using DapperDataObjectLib;
using OfficeOpenXml;

namespace SahoAcs
{
    public partial class QueryDeviceOPLog2 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
        string _sqlcommand;
        private static string _Equtype = "Door Access";
        Hashtable TableInfo;
        DataTable CardLogTable = null;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryDeviceOPLog", "QueryDeviceOPLog.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            DetailButton.Attributes["onClick"] = "CallMsgWin('" + this.GetLocalResourceObject("lblViewSysLog").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            Pub.SetModalPopup(ModalPopupExtender1, 1);

            this.MainGridView.PageSize = _pagesize;

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
                hideDeviceOPType.Value = Request.QueryString["Mode"];
                #endregion

                CreateDOPStateItem();
                ViewState["query_LogTimeBegin"] = "";
                ViewState["query_LogTimeEnd"] = "";
                ViewState["query_DOPState"] = "";
                ViewState["query_EquNo"] = "";
                ViewState["query_UserID"] = "";
                ViewState["query_CardLogTable"] = "";
                ViewState["SortExpression"] = "B01_EquData.EquNo,CreateTime";
                ViewState["SortDire"] = "ASC";

                Input_LogTimeBegin.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                Input_LogTimeEnd.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                //Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_LogTimeBegin"] = Input_LogTimeBegin.DateValue;
                    ViewState["query_LogTimeEnd"] = Input_LogTimeEnd.DateValue;
                    ViewState["query_DOPState"] = Input_DOPState.SelectedValue;
                    ViewState["query_EquNo"] = Input_EquNo.Text;
                    ViewState["query_UserID"] = Input_UserID.Text;
                }

                string[] arrControl = sFormTarget.Split('$');

                if (arrControl.Length == 5)
                {
                    _pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                }
                else
                {
                    _pageindex = 0;
                }
                Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
        }
        #endregion

        #region GridView_Sorting
        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            String SortField = e.SortExpression, SortDire = "DESC";

            if (ViewState["SortExpression"] != null)
            {
                if (ViewState["SortExpression"].ToString().Equals(SortField))
                {
                    if (!ViewState["SortDire"].Equals("ASC"))
                    {
                        SortDire = "ASC";
                    }
                    else
                    {
                        SortDire = "DESC";
                    }
                }
            }

            ViewState["SortExpression"] = SortField;
            ViewState["SortDire"] = SortDire;
            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region GridView_Data_RowDataBound
        public void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:
                    #region 設定欄位寛度
                    int[] HeaderWidth = { 10, 160, 70, 100, 80, 110 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    e.Row.Cells[0].Visible = false;
                    #region 針對各欄位做所需處理

                    #endregion

                    #region 排序條件Header加工
                    foreach (DataControlField dataControlField in MainGridView.Columns)
                    {
                        if (dataControlField.SortExpression.Equals(this.SortExpression))
                        {
                            Label label = new Label();
                            label.Text = this.SortDire.Equals("ASC") ? "▲" : "▼";
                            e.Row.Cells[MainGridView.Columns.IndexOf(dataControlField)].Controls.Add(label);
                        }
                    }
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
                    GrRow.ID = "GV_Row" + oRow.Row["RecordID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 10, 163, 74, 104, 84, 114 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region Log記錄時間
                    #endregion
                    #region 設備作業狀態
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 操作者帳號
                    #endregion
                    #region 操作者IP
                    #endregion
                    #region 結果訊息
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 14, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 11, true);
                    e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 14, true);
                    e.Row.Cells[6].Text = LimitText(e.Row.Cells[6].Text, 46, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["RecordID"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallMsgWin('" + "記錄詳細資料" + "')");

                    break;
                #endregion

                #region Pager
                case DataControlRowType.Pager:

                    #region 取得控制項
                    GridView gv = sender as GridView;
                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region 顯示頁數及[上下頁、首頁、末頁]處理
                    int showRange = 5; //顯示快捷頁數
                    //int pageCount = gv.PageCount;
                    //int pageIndex = gv.PageIndex;
                    _pagecount = (_datacount % _pagesize) == 0 ? (_datacount / _pagesize) : (_datacount / _pagesize) + 1;
                    int pageCount = _pagecount;
                    int pageIndex = _pageindex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    #region 指定頁數及改變文字風格
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();
                        lbtnPage.ID = "Pages_" + (i).ToString();
                        lbtnPage.Text = (i + 1).ToString();
                        lbtnPage.CommandName = "Pages";
                        lbtnPage.CommandArgument = (i + 1).ToString();
                        lbtnPage.Font.Overline = false;

                        if (i == pageIndex)
                        {
                            lbtnPage.Font.Bold = true;
                            lbtnPage.ForeColor = System.Drawing.Color.White;
                            lbtnPage.OnClientClick = "return false;";
                        }
                        else
                            lbtnPage.Font.Bold = false;

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #endregion

                    #region 上下頁
                    //lbtnPrev.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex > 0)
                    //    {
                    //        gv.PageIndex = gv.PageIndex - 1;
                    //        Query();
                    //    }
                    //};
                    if (_pageindex == 0)
                    {
                        lbtnPrev.Enabled = false;
                    }
                    else
                    {
                        lbtnPrev.Enabled = true;
                        lbtnPrev.ID = "lbtnPrev_" + (_pageindex - 1);
                    }

                    //lbtnNext.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex < gv.PageCount)
                    //    {
                    //        gv.PageIndex = gv.PageIndex + 1;
                    //        Query();
                    //    }
                    //};
                    if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    {
                        lbtnNext.Enabled = false;
                    }
                    else
                    {
                        lbtnNext.Enabled = true;
                        lbtnNext.ID = "lbtnNext_" + (_pageindex + 1);
                    }
                    #endregion

                    #region 首末頁
                    //lbtnFirst.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = 0;
                    //    Query();
                    //};
                    if (_pageindex == 0 || _pagecount == 0)
                    {
                        lbtnFirst.Enabled = false;
                    }
                    else
                    {
                        lbtnFirst.Enabled = true;
                        lbtnFirst.ID = "lbtnFirst_" + 0;
                    }

                    //lbtnLast.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = MainGridView.PageCount;
                    //    Query();
                    //};
                    if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    {
                        lbtnLast.Enabled = false;
                    }
                    else
                    {
                        lbtnLast.Enabled = true;
                        lbtnLast.ID = "lbtnLast_" + (_pagecount - 1);
                    }
                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region 顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region 寫入Literal_Pager
                    StringWriter Pager_sw = new StringWriter();
                    HtmlTextWriter Pager_writer = new HtmlTextWriter(Pager_sw);
                    e.Row.CssClass = "GVStylePgr";

                    e.Row.RenderControl(Pager_writer);
                    e.Row.Visible = false;
                    li_Pager.Text = Pager_sw.ToString();
                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region GridView_Data_DataBound
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }
        #endregion

        #region GridView_PageIndexChanging
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        #region ExcelButton_Click
        protected void ExcelButton_Click(object sender, EventArgs e)
        {
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #endregion

        #region Method

        #region CreateDOPStateItem
        private void CreateDOPStateItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            Input_DOPState.Items.Clear();

            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            this.Input_DOPState.Items.Add(Item);

            switch (hideDeviceOPType.Value)
            {
                case "Card":
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "OK";
                    Item.Value = "1";
                    this.Input_DOPState.Items.Add(Item);
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "NG";
                    Item.Value = "0";
                    this.Input_DOPState.Items.Add(Item);
                    break;
                case "Parameter":
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "OK";
                    Item.Value = "1";
                    this.Input_DOPState.Items.Add(Item);
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "NG";
                    Item.Value = "0";
                    this.Input_DOPState.Items.Add(Item);
                    break;
                case "Device":
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "Conn";
                    Item.Value = "1";
                    this.Input_DOPState.Items.Add(Item);
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "DisConn";
                    Item.Value = "0";
                    this.Input_DOPState.Items.Add(Item);
                    break;
                default:
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = "尚無資料";
                    Item.Value = "";
                    this.Input_DOPState.Items.Add(Item);
                    break;
            }
        }
        #endregion

        #region LimitText
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);
            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis) len -= 3;

                string res = big5.GetString(b, 0, len);
                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);
                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion

        #region Query
        public void Query(bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
//            sql = @" SELECT DISTINCT DeviceOPLog.RecordID,
//                     DeviceOPLog.CreateTime AS LogTime,
//                     StateList.ItemName AS DOPState, DeviceOPLog.EquNO,
//                     DeviceOPLog.CreateUserID AS UserID, DeviceOPLog.IPAddress AS UserIP, DeviceOPLog.ResultMsg
//                     FROM B00_SysDeviceOPLog AS DeviceOPLog 
//                     LEFT JOIN B00_ItemList AS StateList ON StateList.ItemClass = 'DOPState' AND StateList.ItemNo = DeviceOPLog.DOPState AND StateList.ItemInfo1 = DeviceOPLog.DOPType
//                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = DeviceOPLog.EquNo
//                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
//                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
//                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
//                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY " + SortExpression + " " + SortDire + @") AS NewIDNum,
                     B00_SysDeviceOPLog.RecordID, B00_SysDeviceOPLog.CreateTime AS LogTime,
                     CASE DOPActive WHEN 'SetOnceCardData' THEN '單筆設碼'+B00_ItemList.ItemName
					 WHEN 'CancleOnceCardData' THEN '消碼'+B00_ItemList.ItemName
					 WHEN 'SetAllCardDataStart' THEN '多筆設碼'+B00_ItemList.ItemName
					 ELSE
						B00_ItemList.ItemName
					 END AS DOPState , B00_SysDeviceOPLog.EquNO,
                     B00_SysDeviceOPLog.CreateUserID AS UserID, B00_SysDeviceOPLog.IPAddress AS UserIP, B00_SysDeviceOPLog.ResultMsg
                     FROM B00_SysDeviceOPLog
                     LEFT JOIN B00_ItemList ON B00_ItemList.ItemClass = 'DOPState' AND B00_ItemList.ItemNo = B00_SysDeviceOPLog.DOPState AND B00_ItemList.ItemInfo1 = B00_SysDeviceOPLog.DOPType
                     INNER JOIN (SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                     INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                     INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser ON B00_SysUser.UserID = B00_SysUserMgns.UserID
                     WHERE B00_SysUser.UserID = ? GROUP BY B01_EquData.EquNo, B01_EquData.EquName)
                     AS B01_EquData ON B01_EquData.EquNo = B00_SysDeviceOPLog.EquNo ";

            #region DataAuth
            //if (wheresql != "") wheresql += " AND ";
            //wheresql += " (SysUser.UserID = ? ) ";
            liSqlPara.Add("S:" + Session["UserID"].ToString());
            #endregion

            if (wheresql != "") wheresql += " AND ";
            wheresql += " (B00_SysDeviceOPLog.DOPType =  ?) ";
            liSqlPara.Add("S:" + hideDeviceOPType.Value);

            if (!string.IsNullOrEmpty(ViewState["query_LogTimeBegin"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (? <= B00_SysDeviceOPLog.CreateTime) ";
                liSqlPara.Add("D:" + ViewState["query_LogTimeBegin"].ToString().Trim());
            }

            if (!string.IsNullOrEmpty(ViewState["query_LogTimeEnd"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B00_SysDeviceOPLog.CreateTime <= ?) ";
                liSqlPara.Add("D:" + ViewState["query_LogTimeEnd"].ToString().Trim());
            }

            if (!string.IsNullOrEmpty(ViewState["query_DOPState"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B00_SysDeviceOPLog.DOPState = ?) ";
                liSqlPara.Add("I:" + ViewState["query_DOPState"].ToString().Trim());
            }

            if (!string.IsNullOrEmpty(ViewState["query_EquNo"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B00_SysDeviceOPLog.EquNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_EquNo"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_UserID"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B00_SysDeviceOPLog.CreateUserID LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_UserID"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            //sql += wheresql + " ORDER BY LogTime ";
            _sqlcommand = sql += wheresql;
            _datacount = oAcsDB.DataCount(_sqlcommand, liSqlPara);
            hDataRowCount.Value = _datacount.ToString();

            if (bMode == true)
            {
                oAcsDB.GetDataTable("DeviceOPLog", _sqlcommand, liSqlPara, out CardLogTable);
            }
            else
            {
                CardLogTable = oAcsDB.PageData(_sqlcommand, liSqlPara, "NewIDNum", _pageindex + 1, _pagesize);
            }
            #endregion

            //oAcsDB.GetDataTable("EquTable", sql, liSqlPara, out dt);
            if (bMode == true)
            {
                if (CardLogTable.Rows.Count != 0)
                {
                    ExportExcel(CardLogTable);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryCardLog.aspx';", true);
                }
            }
            else
            {
                GirdViewDataBind(this.MainGridView, CardLogTable);
                UpdatePanel1.Update();
            }
        }
        #endregion

        #region LoadData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            
            string sql = "";
            string[] TableData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT
                     DeviceOPLog.CreateTime AS LogTime,
                     StateList.ItemName AS DOPState, DeviceOPLog.EquNO,
                     DeviceOPLog.CreateUserID AS UserID, DeviceOPLog.IPAddress AS UserIP, DeviceOPLog.ResultMsg,DOPActive,DOPType
                     FROM B00_SysDeviceOPLog AS DeviceOPLog 
                     LEFT JOIN B00_ItemList AS StateList ON StateList.ItemClass = 'DOPState' AND StateList.ItemNo = DeviceOPLog.DOPState AND StateList.ItemInfo1 = DeviceOPLog.DOPType
                     WHERE DeviceOPLog.RecordID = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                TableData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    string dopstate = dr.DataReader["DOPState"].ToString() == "OK" ? " 成功 " : "失敗";
                    if (i == 1)
                    {
                        TableData[i] = dopstate;
                    }
                    else if (i == dr.DataReader.FieldCount - 3)
                    {
                        string resultmsg=dr.DataReader["ResultMsg"].ToString();
                        string dopactive = dr.DataReader["DOPActive"].ToString();
                        string doptype = dr.DataReader["DOPType"].ToString();
                        if (doptype == "Card")
                        {
                             dopactive=dr.DataReader["DOPActive"].ToString()== "SetOnceCardData" ? "設碼" : "消碼";
                        }
                        else if (doptype == "Parameter")
                        {
                            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                            var paraNames = odo.GetQueryResult("SELECT * FROM B01_EquParaDef WHERE ParaName=@ParaName", new {ParaName=dopactive.Replace("Set","") });
                            if (paraNames.Count() > 0)
                            {
                                dopactive = Convert.ToString(paraNames.First().ParaDesc);
                            }
                        }
                        if (resultmsg.Contains("]") && resultmsg.Contains("["))
                        {
                            TableData[i] = "卡號："+resultmsg.Substring(resultmsg.IndexOf('[') + 1, resultmsg.IndexOf(']') - resultmsg.IndexOf('[') - 1) +dopactive+dopstate
                                +" 在設備編號"+dr.DataReader["EquNo"].ToString();
                        }
                        else if(doptype=="Parameter")
                        {
                            TableData[i] ="設備 "+ dr.DataReader["EquNo"].ToString()+" 處理 "+dopactive+ " 設碼 " + dopstate;
                        }
                        else
                        {
                            TableData[i] = dr.DataReader[i].ToString() +" on 設備編號 "+ dr.DataReader["EquNo"].ToString(); ;
                        }
                    }
                    else
                    {
                        TableData[i] = dr.DataReader[i].ToString();
                    }                    
                }                    
            }
            else
            {
                TableData = new string[2];
                TableData[0] = "Saho_SysErrorMassage";
                TableData[1] = "系統中無此資料！";
            }
            #endregion

            return TableData;
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
                ProcessGridView.Rows[0].Cells[0].Text = this.GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }
        #endregion

        #region ExportExcel
        public void ExportExcel(DataTable ProcDt)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CardLog");
            DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "Log記錄";
            ws.Cells[1, 2].Value = "作業狀態";
            ws.Cells[1, 3].Value = "設備編號";
            ws.Cells[1, 4].Value = "操作者帳號";
            ws.Cells[1, 5].Value = "操作者IP";
            ws.Cells[1, 6].Value = "結果訊息";
            //Content
            for (int i = 0, iCount = dtCardLog.Rows.Count; i < iCount; i++)
            {
                for (int j = 2, jCount = dtCardLog.Rows[i].ItemArray.Length; j < jCount; j++)
                {
                    ws.Cells[i + 2, j - 1].Value = dtCardLog.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                }
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=DeviceOPCardLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

            //for (int x = 0; x < MainGridView.Rows.Count; x++)
            //{
            //    for (int i = 0; i < MainGridView.Rows[x].Cells.Count; i++)
            //    {
            //        MainGridView.Rows[x].Cells[i].Text = MainGridView.Rows[x].Cells[i].ToolTip;
            //    }
            //}

            //Response.ClearContent();
            //string excelFileName = "ToExcel.xls";
            //Response.AddHeader("content-disposition", "attachment;filename=" + Server.UrlEncode(excelFileName));
            //Response.ContentType = "application/excel";
            //System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            //li_header.Text = "<table border='1'>" + li_header.Text.ToString().Replace("th", "td");
            //li_header.RenderControl(htmlWrite);
            //MainGridView.Attributes.Clear();
            //for (int i = 0; i < MainGridView.Rows.Count; i++)
            //    MainGridView.Rows[i].Attributes.Clear();
            //MainGridView.RenderControl(htmlWrite);
            //Response.Write(stringWrite.ToString());
            //Response.End();
        }
        #endregion

        #region 排序欄位及條件
        public string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                {
                    ViewState["SortExpression"] = "SortCode";
                }
                return ViewState["SortExpression"].ToString();
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        public string SortDire
        {
            get
            {
                if (ViewState["SortDire"] == null)
                {
                    ViewState["SortDire"] = " DESC ";
                }
                return ViewState["SortDire"].ToString();
            }
            set
            {
                ViewState["SortDire"] = value;
            }
        }
        #endregion

        #endregion
    }
}