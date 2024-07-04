using SMS_Communication;
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
    public partial class Elevator : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        private static string _Equtype = "Elevator";
        Hashtable TableInfo;
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
            //js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("Elevator", "Elevator.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            ParaButton.Attributes["onClick"] = "CallParaSetting('" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "');return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" + "電梯設備資料新增" + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + "電梯設備資料編輯" + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + "電梯設備資料刪除" + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
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
                DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                string sql = "";
                Sa.DB.DBReader dr;
                #region Process DefaultCardLen
                sql = @" SELECT ParaValue FROM B00_SysParameter WHERE ParaNo = 'CardLen' ";
                oAcsDB.GetDataReader(sql, out dr);
                if (dr.Read())
                    DefaultCardLen.Value = dr.DataReader["ParaValue"].ToString();
                #endregion
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CreateDropItem();
                ViewState["query_Building"] = this.Input_Building.SelectedValue;
                ViewState["query_EquNo"] = "";
                ViewState["query_EquName"] = "";
                ViewState["SortExpression"] = "EquNo";
                ViewState["SortDire"] = "ASC";
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_Building"] = this.Input_Building.SelectedValue;
                    ViewState["query_EquNo"] = this.Input_EquNo.Text.Trim();
                    ViewState["query_EquName"] = this.Input_EquName.Text.Trim();
                }
                else
                {
                    CreateDropItem();
                    UpdatePanel2.Update();
                }

                if (!string.IsNullOrEmpty(sFormArg))
                {
                    if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                    {
                        ViewState["query_Building"] = this.BuildingValue.Value;
                        int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                    }
                    else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                    {
                        Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                    }
                }
                else
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
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
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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
                    int[] HeaderWidth = { 100, 250, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

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
                    GrRow.ID = "GV_Row" + oRow.Row["EquNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 103, 254, 104 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備型號
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱
                    #endregion
                    #region 設備連線
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text.Trim(), 20, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text.Trim(), 34, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text.Trim(), 32, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["EquNo"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + "電梯設備資料編輯" + "')");
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
                    int pageCount = gv.PageCount;
                    int pageIndex = gv.PageIndex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;
                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #region 指定頁數及改變文字風格
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();
                        lbtnPage.Text = (i + 1).ToString();
                        lbtnPage.CommandName = "Page";
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
                    lbtnPrev.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };
                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount)));
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

        #region popInput_EquModel_Init
        protected void popInput_EquModel_Init(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- 請選擇 -";
            Item.Value = "";
            this.popInput_EquModel.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT * FROM B00_ItemList
                     WHERE ItemInfo1 = '" + _Equtype + "' ";
            #endregion

            oAcsDB.GetDataTable("EquModelItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["ItemName"].ToString();
                Item.Value = dr["ItemNo"].ToString();
                this.popInput_EquModel.Items.Add(Item);
            }
        }
        #endregion

        #region popInput_Dci_Init
        protected void popInput_Dci_Init(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- 請選擇 -";
            Item.Value = "";
            this.popInput_Dci.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT * FROM B01_DeviceConnInfo ";
            #endregion

            oAcsDB.GetDataTable("DciItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["DciNo"].ToString() + " (" + dr["DciName"].ToString() + ")";
                Item.Value = dr["DciID"].ToString();
                this.popInput_Dci.Items.Add(Item);
            }
        }
        #endregion

        #region SetButton_Click
        protected void SetButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectValue.Value))
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + this.GetGlobalResourceObject("ResourceEquData", "MsgNonSelect") + "!');");
            else
            {
                SendAppCmdStrList("0" + "@" + SelectValue.Value + "@" + "SetAllCardDataStart" + "@");
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + this.GetGlobalResourceObject("ResourceEquData", "MsgProcess") + "!');");
            }
        }
        #endregion

        #endregion

        #region Method

        #region CreateDropItem
        private void CreateDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            Input_Building.Items.Clear();

            #region Process String
            sql = @" SELECT DISTINCT EquData.Building
                     FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? )";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            if (wheresql != "") wheresql += " AND ";
            wheresql += " (EquData.EquClass = ?) ";
            liSqlPara.Add("S:" + _Equtype);

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquData.Building ";
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["Building"].ToString();
                    Item.Value = dr["Building"].ToString();
                    if (Item.Value == this.BuildingValue.Value)
                        Item.Selected = true;
                    this.Input_Building.Items.Add(Item);
                }
            else
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = "尚無資料";
                Item.Value = "";
                this.Input_Building.Items.Add(Item);
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
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT DISTINCT EquData.*,
                     DeviceConnInfo.DciNo + ' (' + DeviceConnInfo.DciName+ ')' AS Dci,
                     InCtrlArea.CtrlAreaName AS InCtrlAreaName,
                     OutCtrlArea.CtrlAreaName AS OutCtrlAreaName
                     FROM B01_EquData AS EquData
                     LEFT JOIN B01_DeviceConnInfo AS DeviceConnInfo ON DeviceConnInfo.DciID = EquData.DciID
                     LEFT JOIN B01_CtrlArea AS InCtrlArea ON InCtrlArea.CtrlAreaID = EquData.InToCtrlAreaID
                     LEFT JOIN B01_CtrlArea AS OutCtrlArea ON OutCtrlArea.CtrlAreaID = EquData.OutToCtrlAreaID
                     LEFT JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     LEFT JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     LEFT JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            //wheresql += " (SysUser.UserID = ? )";
            //liSqlPara.Add("S:" + this.hideUserID.Value);
            wheresql += " (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + '%' + this.hideUserID.Value + '%');
            #endregion

            if (wheresql != "") wheresql += " AND ";
            wheresql += " (EquData.EquClass = ?) ";
            liSqlPara.Add("S:" + _Equtype);

            if (!string.IsNullOrEmpty(ViewState["query_Building"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.Building = ?) ";
                liSqlPara.Add("S:" + ViewState["query_Building"].ToString().Trim());
            }

            if (!string.IsNullOrEmpty(ViewState["query_EquNo"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquNo like ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_EquNo"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_EquName"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquName like ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_EquName"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("EquTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
        }
        #endregion

        #region Query(string mode)
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT DISTINCT EquData.*,
                     DeviceConnInfo.DciNo + ' (' + DeviceConnInfo.DciName+ ')' AS Dci,
                     InCtrlArea.CtrlAreaName AS InCtrlAreaName,
                     OutCtrlArea.CtrlAreaName AS OutCtrlAreaName
                     FROM B01_EquData AS EquData
                     LEFT JOIN B01_DeviceConnInfo AS DeviceConnInfo ON DeviceConnInfo.DciID = EquData.DciID
                     LEFT JOIN B01_CtrlArea AS InCtrlArea ON InCtrlArea.CtrlAreaID = EquData.InToCtrlAreaID
                     LEFT JOIN B01_CtrlArea AS OutCtrlArea ON OutCtrlArea.CtrlAreaID = EquData.OutToCtrlAreaID
                     LEFT JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     LEFT JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     LEFT JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + '%' + this.hideUserID.Value + '%');
            #endregion

            if (wheresql != "") wheresql += " AND ";
            wheresql += " (EquData.EquClass = ?) ";
            liSqlPara.Add("S:" + _Equtype);

            if (!string.IsNullOrEmpty(ViewState["query_Building"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.Building = ?) ";
                liSqlPara.Add("S:" + ViewState["query_Building"].ToString().Trim());
            }


            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("EquTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["EquNo"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
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
                     EquData.Building,
                     EquData.EquModel, EquData.EquNo, EquData.EquName, EquData.EquEName,
                     EquData.DciID, EquData.CardNoLen
                     FROM B01_EquData AS EquData
                     WHERE EquNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                TableData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    TableData[i] = dr.DataReader[i].ToString();
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

        #region Insert
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string UserID, string popInput_Building, string popInput_EquModel, string popInput_EquNo, string popInput_EquName, string popInput_EquEName, string popInput_Dci, string popInput_CardNoLen)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_EquNo;

            objRet = Check_Input_DB(NoArray, popInput_Building, popInput_EquModel, popInput_EquNo, popInput_EquName, popInput_EquEName, popInput_Dci, popInput_CardNoLen, "Insert");
            if (objRet.result)
            {
                #region Process String - Add EquData
                sql = @" INSERT INTO B01_EquData(EquNo, EquName, EquEName, EquClass, EquModel, CardNoLen, DciID, Building, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";
                liSqlPara.Add("S:" + popInput_EquNo.Trim());
                liSqlPara.Add("S:" + popInput_EquName.Trim());
                liSqlPara.Add("S:" + popInput_EquEName.Trim());
                liSqlPara.Add("S:" + _Equtype);
                liSqlPara.Add("S:" + popInput_EquModel.Trim());
                liSqlPara.Add("I:" + popInput_CardNoLen.Trim());
                liSqlPara.Add("I:" + popInput_Dci.Trim());
                liSqlPara.Add("S:" + popInput_Building.Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }

            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region Update
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string UserID, string SelectValue, string popInput_Building, string popInput_EquModel, string popInput_EquNo, string popInput_EquName, string popInput_EquEName, string popInput_Dci, string popInput_CardNoLen)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_EquNo;
            NoArray[1] = SelectValue;

            objRet = Check_Input_DB(NoArray, popInput_Building, popInput_EquModel, popInput_EquNo, popInput_EquName, popInput_EquEName, popInput_Dci, popInput_CardNoLen, "Update");
            if (objRet.result)
            {
                #region Process String - Updata EquData
                sql = @" UPDATE B01_EquData
                         SET EquNo = ?, EquName = ?, EquEName = ?,
                         EquModel = ?, CardNoLen = ?, DciID = ?,
                         Building = ?,
                         UpdateUserID = ?, UpdateTime = ?
                         WHERE EquNo = ? ";

                liSqlPara.Add("S:" + popInput_EquNo.Trim());
                liSqlPara.Add("S:" + popInput_EquName.Trim());
                liSqlPara.Add("S:" + popInput_EquEName.Trim());
                liSqlPara.Add("S:" + popInput_EquModel.Trim());
                liSqlPara.Add("I:" + popInput_CardNoLen.Trim());
                liSqlPara.Add("I:" + popInput_Dci.Trim());
                liSqlPara.Add("S:" + popInput_Building.Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }

            objRet.act = "Edit";
            return objRet;
        }
        #endregion

        #region Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", eid = "";
            List<string> liSqlPara = new List<string>();
            int iRet = 0;

            oAcsDB.BeginTransaction();

            #region 取得EID
            sql = "SELECT EquID FROM B01_EquData WHERE EquNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            Sa.DB.DBReader dr;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                eid = dr.DataReader["EquID"].ToString();
            }
            sql = "";
            liSqlPara.Clear();
            #endregion

            if (objRet.result)
            {
                if (iRet > -1)
                {
                    #region Process String - Delete ElevatorFloor
                    sql = @" DELETE FROM B01_ElevatorFloor WHERE EquID = ? ";
                    liSqlPara.Add("S:" + eid);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String - Delete EquGroupData
                    sql = @" DELETE FROM B01_EquGroupData WHERE EquID = ? ";
                    liSqlPara.Add("S:" + eid);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String - Delete EquParaData
                    sql = @" DELETE FROM B01_EquParaData WHERE EquID = ? ";
                    liSqlPara.Add("S:" + eid);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String - Delete EquDataExt
                    sql = @" DELETE FROM B01_EquDataExt WHERE EquID = ? ";
                    liSqlPara.Add("S:" + eid);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String - Delete EquData
                    sql = " DELETE FROM B01_EquData WHERE EquID = ? ";
                    liSqlPara.Add("S:" + eid);
                    #endregion
                    oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                if (iRet > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }

            objRet.act = "Delete";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string Building, string EquModel, string EquNo, string EquName, string EquEName, string Dci, string CardNoLen, string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;
            int tempint;

            #region Input
            if (string.IsNullOrEmpty(Building.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "建築物名稱 必須輸入";
            }
            else if (Building.Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "建築物名稱 字數超過上限";
            }

            if (string.IsNullOrEmpty(EquModel.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備型號 必須指定";
            }

            if (string.IsNullOrEmpty(NoArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備編號 必須輸入";
            }
            else if (NoArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備編號 字數超過上限";
            }

            if (string.IsNullOrEmpty(EquName.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備名稱 必須輸入";
            }
            else if (EquName.Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備名稱 字數超過上限";
            }

            if (EquEName.Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備英文名稱 字數超過上限";
            }

            if (string.IsNullOrEmpty(Dci.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備連線 必須指定";
            }

            if (string.IsNullOrEmpty(CardNoLen.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "卡號長度 必須輸入";
            }
            else if (!int.TryParse(CardNoLen.Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "卡號長度 必需為數字";
            }

            if (int.TryParse(CardNoLen.Trim(), out tempint))
            {
                if (tempint < 4 || tempint > 16)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "卡號長度 需介於 04 ~ 16 之間。";
                }
            }

            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_EquData WHERE EquNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_EquData WHERE EquNo = ? AND EquNo != ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
            }

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "此代碼已存在於系統中";
            }
            #endregion

            return objRet;
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
                ProcessGridView.Rows[0].Cells[0].Text = this.GetGlobalResourceObject("Resource","NonData").ToString();
            }
        }
        #endregion

        #region SendAppCmdStrList
        /// <summary>
        /// 傳送APP指令字串清單至SahoWebSocket
        /// </summary>
        /// <param name="sCmdStrList">指令字串清單</param>
        private void SendAppCmdStrList(string sCmdStrList)
        {
            string[] sIPArray = null, sAppCmdStrArray = null;
            SahoWebSocket oSWSocket = null;

            try
            {
                if (!string.IsNullOrEmpty(sCmdStrList))
                {
                    #region 建立與設定SahoWebSocket
                    if (Session["SahoWebSocket"] != null)
                    {
                        if ((!((SahoWebSocket)Session["SahoWebSocket"]).IsWorking) || ((SahoWebSocket)Session["SahoWebSocket"]).IsGameOver)
                        {
                            ((SahoWebSocket)Session["SahoWebSocket"]).Stop();
                            ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        }
                    }
                    else
                    {
                        #region 取得APP的IP位址
                        string sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                        if (!string.IsNullOrEmpty(sIPAddress)) { sIPArray = sIPAddress.Split(new char[] { ',' }); } else { sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }
                        if (sIPAddress == "::1") { sIPAddress = "127.0.0.1"; }
                        #endregion

                        #region 建立與設定SahoWebSocket物件及基本資料
                        Session["SahoWebSocket"] = new SahoWebSocket();

                        ((SahoWebSocket)Session["SahoWebSocket"]).UserID = hideUserID.Value;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SourceIP = sIPAddress;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                        //((SahoWebSocket)Session["SahoWebSocket"]).SelectRecordIDAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        //((SahoWebSocket)Session["SahoWebSocket"]).InsertAppCmdObjAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        //((SahoWebSocket)Session["SahoWebSocket"]).UpdateDriverCmdResultStrAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        #endregion
                    }
                    #endregion

                    #region 傳送APP指令字串
                    oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];

                    oSWSocket.ClearCmdResult();

                    sAppCmdStrArray = sCmdStrList.Split(';');
                    for (int i = 0; i < sAppCmdStrArray.Length; i++) { oSWSocket.SendAppCmdStr(sAppCmdStrArray[i]); }
                    #endregion
                }
            }
            finally
            {
                sIPArray = null;
                oSWSocket = null;
                sAppCmdStrArray = null;
            }
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